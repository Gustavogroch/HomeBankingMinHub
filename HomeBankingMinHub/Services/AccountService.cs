using HomeBankingMinHub.DTOs;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMinHub.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IClientRepository _clientRepository;
        private readonly GenerateAccountNumber _accountNumberGenerator;

        public AccountService(IAccountRepository accountRepository, IClientRepository clientRepository)
        {
            _accountRepository = accountRepository;
            _clientRepository = clientRepository;
            _accountNumberGenerator = new GenerateAccountNumber(accountRepository);
        }



        public IEnumerable<AccountDTO> Get()
        {
            var accounts = _accountRepository.GetAllAcounts();
            if (accounts == null) { return null; }
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
            return AccountDTO;

        }

        public AccountDTO GetAccountById(long id)
        {
            var account = _accountRepository.FindById(id);
            if (account == null)
            {
                return null;
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
            return accountDTO;

        }

        public IActionResult CreateAccountForCurrentUser(string email)
        {
            try
            {
                // Buscar el cliente
                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                {
                    return new BadRequestObjectResult("El cliente no se encontró.");
                }

                // Verificar si el cliente ya tiene 3 cuentas registradas
                if (client.Accounts.Count >= 3)
                {
                    return new StatusCodeResult(403);
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
                _accountRepository.Save(newAccount);

                return new OkObjectResult("Cuenta creada exitosamente");

            }

            catch (Exception)

            { return new StatusCodeResult(500); }
        }

    }
}
