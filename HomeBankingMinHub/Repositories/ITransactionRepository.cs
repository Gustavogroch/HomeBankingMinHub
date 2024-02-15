using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.Repositories
{
    public interface ITransactionRepository
    {
        void Save(Transaction transaction);
        Transaction FindByNumber(long id);
    }
}