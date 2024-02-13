namespace HomeBankingMinHub.Services
{
    public class CardService
    {
        public string GenerateCardNumber()
        {
            Random random = new Random();
            return $"{random.Next(1000, 9999)}-{random.Next(1000, 9999)}-{random.Next(1000, 9999)}-{random.Next(1000, 9999)}";
        }

        public int GenerateCvv()
        {
            Random random = new Random();
            return random.Next(100, 999);
        }


    }
}
