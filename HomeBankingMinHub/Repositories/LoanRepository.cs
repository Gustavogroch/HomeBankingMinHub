using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.Repositories
{
    public class LoanRepository : RepositoryBase<Loan>, ILoanRepository
    {
        public LoanRepository (HomeBankingContext repositorycontext) : base (repositorycontext) { }

        public IEnumerable<Loan> GetAllLoans() 
        {
            return FindAll().ToList();
        }

        public Loan FindById(long id)
        {
            return FindByCondition(loan => loan.Id == id)
                .FirstOrDefault();
        }
    }
}
