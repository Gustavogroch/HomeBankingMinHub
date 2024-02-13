using HomeBankingMinHub.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeBankingMinHub.Repositories
{
    public class CardRepository : RepositoryBase<Card>, ICardRepository
    {
        public CardRepository(HomeBankingContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<Card> GetAllCards()
        {
            return FindAll().ToList();
        }

        public Card FindById(long id)
        {
            return FindByCondition(card => card.Id == id)
                .FirstOrDefault();
        }

        public IEnumerable<Card> GetCardsByClient(long clientId, string type)
        {
            return FindByCondition(card => card.ClientId == clientId)
            .ToList();
        }


        public void Save(Card card)
        {
            Create(card);
            SaveChanges();
        }
    }
}
