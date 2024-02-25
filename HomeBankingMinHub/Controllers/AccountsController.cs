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
                // Obtener la información del cliente autenticado
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }

            return _AccountService.CreateAccountForCurrentUser(email);

        }


    }
}
         