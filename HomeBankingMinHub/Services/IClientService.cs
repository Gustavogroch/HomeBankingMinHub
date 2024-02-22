using HomeBankingMinHub.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HomeBankingMinHub.Services
{
    public interface IClientService
    {
        public IEnumerable<ClientDTO> GetAllClients();
        public ClientDTO GetClientById(long id);

        public IActionResult CreateClient(ClientCreateDTO model);
        public ClientDTO GetCurrent(string email);
        public IEnumerable<AccountDTO> GetCurrentAccount(string email);
    }
}
