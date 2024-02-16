using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.Repositories
{
    public interface IClientLoanRepository
    {
        void Save(ClientLoan clientLoan);
    }
}
