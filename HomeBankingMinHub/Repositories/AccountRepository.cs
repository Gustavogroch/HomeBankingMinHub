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
        public void Save(Account account)

        {
            if (account.Id == 0)
            {
                Create(account);
            }
            else
            {
                Update(account);
            }

            SaveChanges();
        }

        public IEnumerable<Account> GetAccountsByClient(long clientId)
        {
            return FindByCondition(account => account.ClientId == clientId)
            .Include(account => account.Transactions)
            .ToList();
        }

        public bool Exists(string accountNumber)
        {
            var account = FindByCondition(a => a.Number == accountNumber).FirstOrDefault();
            return account != null;
        }

        public Account FinByNumber(string number)
        {
            return FindByCondition(account => account.Number.ToUpper() == number.ToUpper())
            .Include(account => account.Transactions)
            .FirstOrDefault();
        }
    }
}
