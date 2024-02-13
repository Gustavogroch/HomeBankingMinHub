using HomeBankingMinHub.Repositories;

namespace HomeBankingMinHub.Services
{
    public class AccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public string GenerateUniqueAccountNumber()
        {
            string accountNumber;
            do
            {
                accountNumber = GenerateAccountNumber();
            }
            while (_accountRepository.Exists(accountNumber));

            return accountNumber;
        }

        private string GenerateAccountNumber()
        {
            Random random = new Random();
            return "VIN-" + random.Next(100000, 999999);
        }
    }


}
