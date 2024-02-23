using HomeBankingMinHub.DTOs;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace HomeBankingMinHub.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly AccountService _accountService;
        private readonly GenerateAccountNumber _accountNumberGenerator;

        public ClientService(IClientRepository clientRepository, IAccountRepository accountRepository, AccountService accountService)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _accountService = accountService;
            _accountService = new AccountService(accountRepository);
            _accountNumberGenerator = new GenerateAccountNumber(accountRepository);
        }
        public IEnumerable<ClientDTO> GetAllClients()
        {
            var clients = _clientRepository.GetAllClients();
            if (clients == null) { return null; }
            var clientsDTO = new List<ClientDTO>();
            foreach (Client client in clients)
            {
                var newClientDTO = new ClientDTO
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
                    }).ToList()
                };
                clientsDTO.Add(newClientDTO);
            }
            return clientsDTO;
        }

        public ClientDTO GetClientById(long id)
        {
            var client = _clientRepository.FindById(id);
            if (client == null) { return null; }
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
            return clientDTO;
        }

        public IActionResult CreateClient(ClientCreateDTO model)
        {
            try
            {
                if (model.Email.IsNullOrEmpty() || model.FirstName.IsNullOrEmpty() || model.LastName.IsNullOrEmpty())
                {
                    return new BadRequestObjectResult("Se requieren todos los campos");
                }
                if (_clientRepository.FindByEmail(model.Email) != null)
                {
                    return new BadRequestObjectResult("El correo electrónico ya está en uso.");
                }

                var client = new Client();
                client.Email = model.Email;
                client.FirstName = model.FirstName;
                client.LastName = model.LastName;
                client.Password = HashPassword.Hash(model.Password);
                _clientRepository.Save(client);

                var existingAccounts = _accountRepository.GetAccountsByClient(client.Id);
                if (existingAccounts.Count() == 0)

                {
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
                    _accountRepository.Save(newAccount);
                    return new OkObjectResult("Cuenta creada exitosamente");
                }
                else
                { return new OkObjectResult("El usuario ya cuenta con una cuenta");}
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }

        }

        public ClientDTO GetCurrent(string email)
        {
            Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                { return null;}
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
                return clientDTO;
        }

        public IEnumerable<AccountDTO> GetCurrentAccount(string email)
        {
           
            Client client = _clientRepository.FindByEmail(email);
            if (client == null)
            {
                return null;
            }
            var accountDTOs = client.Accounts.Select(ac => new AccountDTO
            {
                Id = ac.Id,
                Balance = ac.Balance,
                CreationDate = ac.CreationDate,
                Number = ac.Number
            }).ToList();
            return accountDTOs;
        }

    }
}
    
