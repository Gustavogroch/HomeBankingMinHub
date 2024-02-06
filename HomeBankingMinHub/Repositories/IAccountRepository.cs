using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAcounts();
        Account FindById(long id);
    }
}
