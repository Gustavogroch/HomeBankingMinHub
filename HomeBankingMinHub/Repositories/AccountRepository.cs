using HomeBankingMinHub.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingMinHub.Repositories
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }
        public IEnumerable<Account> GetAllAcounts()
        {
            return FindAll()
                .Include(Account => Account.Transactions)
                .ToList();
        }

        public Account FindById(long id)
        {
            return FindByCondition(Account => Account.Id == id)
                .Include(Account => Account.Transactions)
                .FirstOrDefault();
        }
    }
}
