using HomeBankingMinHub.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMinHub.Services
{
    public interface IAccountService
    {
        public IEnumerable<AccountDTO> Get();
        public AccountDTO GetAccountById(long id);
        public IActionResult CreateAccountForCurrentUser(string email);


    }  
}
