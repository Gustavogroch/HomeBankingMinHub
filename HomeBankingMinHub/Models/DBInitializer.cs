namespace HomeBankingMinHub.Models
{
    public class DBInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client { Email = "vcoronado@gmail.com", FirstName="Victor", LastName="Coronado", Password="123246"},
                    


                };
                context.AddRange(clients);
                context.SaveChanges();
            }

        }
    }
}
