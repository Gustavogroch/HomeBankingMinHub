using HomeBankingMinHub.DTOs;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private IAccountRepository _AccountRepository;

        public AccountsController(IAccountRepository AccountRepository)
        {
            _AccountRepository = AccountRepository;

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

        


    }
}
         