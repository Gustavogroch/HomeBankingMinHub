using HomeBankingMinHub.DTOs;

namespace HomeBankingMinHub.Services
{
    public interface IClientService
    {
        public IEnumerable<ClientDTO> GetAllClients();
    }
}
