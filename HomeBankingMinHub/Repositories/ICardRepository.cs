using HomeBankingMinHub.Models;

namespace HomeBankingMinHub.Repositories
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetAllCards();
        Card FindById(long id);
        void Save(Card card);
        IEnumerable<Card> GetCardsByClient(long clientId, string type);

    }
}
