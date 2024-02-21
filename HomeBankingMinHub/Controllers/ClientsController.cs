using HomeBankingMinHub.DTOs;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using HomeBankingMinHub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        public IClientRepository _clientRepository;
        private IAccountRepository _AccountRepository;
        private AccountService _AccountService;
        private IClientService _clientService;

        public ClientsController(IAccountRepository AccountRepository, IClientRepository clientRepository, AccountService accountService, IClientService clientService)
        {
            _clientRepository = clientRepository;
            _AccountRepository = AccountRepository;
            _AccountService = accountService;
            _clientService = clientService;
        }

        [HttpGet]
        public IActionResult Get()
        {

            var clients = _clientService.GetAllClients();
            if (clients == null) 
            { return NotFound(); }

            return Ok(clients);
        }

        [HttpGet("{id}")]

        public IActionResult Get(long id)
        {
            try
            {
                var client = _clientRepository.FindById(id);
                if (client == null)
                {
                    return Forbid();
                }
                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),

                     Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                     {
                         Id = cl.Id,
                         LoanId = cl.LoanId,
                         Name = cl.Loan.Name,
                         Amount = cl.Amount,
                         Payments = int.Parse(cl.Payments)
                     }).ToList(),

                     Cards = client.Cards.Select(c => new CardDTO
                     {
                         Id = c.Id,
                         CardHolder = c.NameCardHolder,
                         Color = c.Color,
                         Cvv = c.Cvv,
                         FromDate = c.FromDate,
                         Number = c.Number,
                         ThruDate = c.ThruDate,
                         Type = c.Type
                     }).ToList()
                };
                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] ClientCreateDTO model)
        {
            try
            {
                if (model.Email.IsNullOrEmpty() || model.FirstName.IsNullOrEmpty() || model.LastName.IsNullOrEmpty())
                {
                    return BadRequest("Se requieren todos los campos");
                }
                if (_clientRepository.FindByEmail(model.Email) != null)
                {
                    return BadRequest("El correo electrónico ya está en uso.");
                }

                var client = new Client();
                client.Email = model.Email;
                client.FirstName = model.FirstName;
                client.LastName = model.LastName;
                client.Password = HashPassword.Hash(model.Password);
                _clientRepository.Save(client);

                var existingAccounts = _AccountRepository.GetAccountsByClient(client.Id);
                if (existingAccounts.Count() == 0)

                {
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
                { return Created(); 
                }
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("/api/clients/current")]
        public IActionResult GetCurrent()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }
                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                {
                    return Forbid();
                }
                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.NameCardHolder,
                        Color = c.Color,
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type
                    }).ToList()
                };
                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("/api/clients/current/accounts")]
        public IActionResult GetCurrentAccounts()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return Forbid();
                }
                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                {
                    return Forbid();
                }
                var accountDTOs = client.Accounts.Select(ac => new AccountDTO
                {
                    Id = ac.Id,
                    Balance = ac.Balance,
                    CreationDate = ac.CreationDate,
                    Number = ac.Number
                }).ToList();
                return Ok(accountDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}



    


