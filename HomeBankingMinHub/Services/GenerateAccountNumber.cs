
using HomeBankingMinHub.Repositories;

namespace HomeBankingMinHub.Services
{
    public class GenerateAccountNumber
    {
        private readonly IAccountRepository _accountRepository;

        public GenerateAccountNumber(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public string GenerateUniqueAccountNumber()
        {
            string accountNumber;
            do
            {
                accountNumber = CreateAccountNumber();
            }
            while (_accountRepository.Exists(accountNumber));

            return accountNumber;
        }

        private string CreateAccountNumber()
        {
            Random random = new Random();
            return "VIN-" + random.Next(100000, 999999);
        }
    }
}
