using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.Repositories
{
    public interface ILoanRepository
    {
        IEnumerable<Loan> GetAllLoans();

        Loan FindById(long id);
    }
}
