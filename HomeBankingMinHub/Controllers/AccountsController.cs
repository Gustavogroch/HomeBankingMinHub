using HomeBankingMinHub.DTOs;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using HomeBankingMinHub.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountRepository _AccountRepository;
        private AccountService _AccountService;
        private IClientRepository _ClientRepository;

        public AccountsController(IAccountRepository AccountRepository, IClientRepository clientRepository, AccountService accountService)
        {
            _AccountRepository = AccountRepository;
            _AccountService = accountService;
            _ClientRepository = clientRepository;

        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var accounts = _AccountRepository.GetAllAcounts();
                var AccountDTO = new List<AccountDTO>();

                foreach (Account account in accounts)
                {

                    var newAccountDTO = new AccountDTO
                    {
                        Id = account.Id,
                        Number = account.Number,
                        CreationDate = account.CreationDate,
                        Balance = account.Balance,
                        Transactions = account.Transactions.Select(transaction => new TransactionDTO
                        {
                            Id = transaction.Id,
                            Type = transaction.Type,
                            Amount = transaction.Amount,
                            Description = transaction.Description,
                            Date = transaction.Date,


                        }).ToList()
                    };
                    AccountDTO.Add(newAccountDTO);

                }
                return Ok(AccountDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetAccountById(long id)
        {
            try
            {
                var account = _AccountRepository.FindById(id);
                if (account == null)
                {
                    return NotFound();
                }
                var accountDTO = new AccountDTO
                {
                    Id = account.Id,
                    Number = account.Number,
                    CreationDate = account.CreationDate,
                    Balance = account.Balance,
                    Transactions = account.Transactions.Select(transaction => new TransactionDTO
                    {
                        Id = transaction.Id,
                        Amount = transaction.Amount,
                        Date = transaction.Date,
                        Type = transaction.Type,
                        Description = transaction.Description,

                    }).ToList()
                };
                return Ok(accountDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("/api/clients/current/accounts")]
        public IActionResult CreateAccountForCurrentUser()
        {
            try
            {
                // Obtener la información del cliente autenticado
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }

                // Buscar el cliente
                Client client = _ClientRepository.FindByEmail(email);
                if (client == null)
                {
                    return Forbid();
                }

                // Verificar si el cliente ya tiene 3 cuentas registradas
                if (client.Accounts.Count >= 3)
                {
                    return StatusCode(403, "El cliente ya tiene 3 cuentas registradas");
                }

                // Generar un número de cuenta aleatorio y único usando el servicio
                string accountNumber = _AccountService.GenerateUniqueAccountNumber();

                // Crear la nueva cuenta
                Account newAccount = new Account
                {
                    Number = accountNumber,
                    CreationDate = DateTime.Now,
                    Balance = 0,
                    ClientId = client.Id,
                    
                };

                // Guardar la cuenta en el repositorio
                _AccountRepository.Save(newAccount);

                return StatusCode(201, "Cuenta creada exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }





    }
}
         