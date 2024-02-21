using HomeBankingMinHub.DTOs;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HomeBankingMinHub.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly AccountService _accountService;

        public ClientService(IClientRepository clientRepository, IAccountRepository accountRepository, AccountService accountService)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _accountService = accountService;
        }

        public IEnumerable<ClientDTO> GetAllClients()
        {        
                var clients = _clientRepository.GetAllClients();
            if (clients == null) { return null; }
                var clientsDTO = new List<ClientDTO>();
                foreach (Client client in clients)
                {
                    var newClientDTO = new ClientDTO
                    {
                        Id = client.Id,
                        Email = client.Email,
                        FirstName = client.FirstName,
                        LastName = client.LastName,
                        Accounts = client.Accounts.Select(ac => new AccountDTO
                        {
                            Id = ac.Id,
                            Balance = ac.Balance,
                            CreationDate = ac.CreationDate,
                            Number = ac.Number
                        }).ToList()
                    };
                    clientsDTO.Add(newClientDTO);
                }
                return clientsDTO;
        }
    
    }

}
    
