using HomeBankingMinHub.DTOs;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Principal;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private ILoanRepository _loanRepository;
        private IAccountRepository _accountRepository;
        private IClientLoanRepository _clientLoanRepository;
        private ITransactionRepository _transactionRepository;
        private readonly IClientRepository _clientRepository;

        public LoansController (ILoanRepository loanRepository, IAccountRepository accountRepository, IClientLoanRepository clientLoanRepository,ITransactionRepository transactionRepository, IClientRepository clientRepository)
        {
            _loanRepository = loanRepository;
            _accountRepository = accountRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
            _clientRepository = clientRepository;
        }

        public IActionResult Post(LoanApplicationDTO loanAppDto)
        {
            try
            {  //usuario autentificado
                string email = User.FindFirst("Client")?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Forbid();
                }

                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                {
                    return Forbid();
                }

                // Verificar que el préstamo exista
                Loan loan = _loanRepository.FindById(loanAppDto.LoanId);
                if (loan == null)
                {
                    return NotFound("El préstamo especificado no existe.");
                }
                // Validar el monto del préstamo
                if (loanAppDto.Amount <= 0 || loanAppDto.Amount > loan.MaxAmount)
                {
                    return BadRequest("El monto del préstamo es inválido.");
                }
                // Validar que los payments no estén vacíos
                if (loanAppDto.Payments == null || !loanAppDto.Payments.Any())
                {
                    return BadRequest("Los pagos del préstamo no pueden estar vacíos.");
                }
                // Verificar que la cuenta de destino exista
                Account account = _accountRepository.FindByNumber(loanAppDto.ToAccountNumber);
                if (account == null)
                {
                    return NotFound("La cuenta de destino especificada no existe.");
                }
                // Verificar que la cuenta de destino pertenezca al cliente autenticado
                if (account.ClientId != int.Parse(User.Identity.Name))
                {
                    return Forbid("La cuenta de destino no pertenece al cliente autenticado.");
                }

                double totalamount = loanAppDto.Amount * 0.20;

                _clientLoanRepository.Save(new ClientLoan
                {
                    ClientId = account.ClientId,
                    Amount = totalamount,
                    Payments= loanAppDto.Payments,
                    LoanId=loanAppDto.LoanId,
                });

                _transactionRepository.Save(new Transaction
                {

                    Type = TransactionType.CREDIT.ToString(),
                    Amount = loanAppDto.Amount,
                    Description = loan.Name,
                    AccountId = account.Id,
                    Date = DateTime.Now,
                });


                account.Balance += loanAppDto.Amount;
                _accountRepository.Save(account);

                return Ok("Préstamo solicitado y procesado correctamente.");

            }

            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        
        }





    }
}
