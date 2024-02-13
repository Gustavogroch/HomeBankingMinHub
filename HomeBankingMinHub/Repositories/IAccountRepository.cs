using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAcounts();
        Account FindById(long id);
        void Save(Account account);
        IEnumerable<Account> GetAccountsByClient(long clientId);
        bool Exists(string accountNumber);
    }
}
