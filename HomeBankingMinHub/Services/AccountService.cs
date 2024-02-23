using HomeBankingMinHub.DTOs;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMinHub.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
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
    }


}
