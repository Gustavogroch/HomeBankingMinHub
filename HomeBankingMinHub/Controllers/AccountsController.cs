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
        private IAccountService _AccountService;
        private IClientRepository _ClientRepository;
        private readonly GenerateAccountNumber _accountNumberGenerator;


        public AccountsController(IAccountRepository AccountRepository, IClientRepository clientRepository, IAccountService accountService)
        {
            _AccountRepository = AccountRepository;
            _AccountService = accountService;
            _ClientRepository = clientRepository;
            _accountNumberGenerator = new GenerateAccountNumber(AccountRepository);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var account = _AccountService.Get();
            if (account == null) 
            { return NotFound("No se encontraron cuentas"); }
            return Ok(account);

        }

        [HttpGet("{id}")]
        public IActionResult GetAccountById(long id)
        {
            var account = _AccountService.GetAccountById(id);
            if (account == null) { return NotFound("no se encontro la cuenta"); }
            return Ok(account);
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
                string accountNumber = _accountNumberGenerator.GenerateUniqueAccountNumber();

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
         