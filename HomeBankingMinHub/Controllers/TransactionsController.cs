using HomeBankingMinHub.DTOs;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private IAccountRepository _accountRepository;
        private ITransactionRepository _transactionRepository;

        public TransactionsController(IClientRepository clientRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }
        [HttpPost]
        public IActionResult Post([FromBody] TransferDTO transferDTO)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid("Email vacio");
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid("No existe el cliente");
                }
                if (transferDTO.FromAccountNumber == string.Empty || transferDTO.ToAccountNumber == string.Empty)
                {
                    return Forbid("Cuenta de origen o cuenta de destino no proporcionada.");
                }
                if (transferDTO.FromAccountNumber == transferDTO.ToAccountNumber)
                {
                    return Forbid("No se permite la transferencia a la misma cuenta.");
                }
                if (transferDTO.Amount == 0 || transferDTO.Description == string.Empty)
                {
                    return Forbid("Monto o descripcion no proporcionados.");
                }
                Account fromAccount = _accountRepository.FinByNumber(transferDTO.FromAccountNumber);
                if (fromAccount == null)
                {
                    return Forbid("Cuenta de origen no existe");
                }
                if (fromAccount.Balance < transferDTO.Amount)
                {
                    return Forbid("Fondos insuficientes");
                }
                Account toAccount = _accountRepository.FinByNumber(transferDTO.ToAccountNumber);
                if (toAccount == null)
                {
                    return Forbid("Cuenta de destino no existe");
                }
                _transactionRepository.Save(new Transaction
                {
                    Type = TransactionType.DEBIT.ToString(),
                    Amount = transferDTO.Amount * -1,
                    Description = transferDTO.Description + " " + toAccount.Number,
                    AccountId = fromAccount.Id,
                    Date = DateTime.Now,
                });

                _transactionRepository.Save(new Transaction
                {
                    Type = TransactionType.CREDIT.ToString(),
                    Amount = transferDTO.Amount,
                    Description = transferDTO.Description + " " + fromAccount.Number,
                    AccountId = toAccount.Id,
                    Date = DateTime.Now,
                });

                fromAccount.Balance = fromAccount.Balance - transferDTO.Amount;
                _accountRepository.Save(fromAccount);

                toAccount.Balance = toAccount.Balance + transferDTO.Amount;
                _accountRepository.Save(toAccount);
                return Created("Creado con exito", fromAccount);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);

            }
        }
    }
}
