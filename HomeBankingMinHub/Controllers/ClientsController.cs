using HomeBankingMinHub.DTOs;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        public IClientRepository _clientRepository;
        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var clients = _clientRepository.GetAllClients();
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
                return Ok(clientsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        [HttpGet("{id}")]

        public IActionResult Get(long id)
        {
            try
            {
                var client = _clientRepository.FindById(id);
                if (client == null)
                {
                    return Forbid();
                }
                var clientDTO = new ClientDTO
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
                    }).ToList(),

                     Loans = client.ClientLoans.Select(cl => new ClientLoanDTO
                     {
                         Id = cl.Id,
                         LoanId = cl.LoanId,
                         Name = cl.Loan.Name,
                         Amount = cl.Amount,
                         Payments = int.Parse(cl.Payments)
                     }).ToList()
                };
                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] ClientCreateDTO model)
        {
            if (model.Email.IsNullOrEmpty() || model.FirstName.IsNullOrEmpty() || model.LastName.IsNullOrEmpty())
            {
                return BadRequest("Se requieren todos los campos");
            }
            try
            {
                var client = new Client();
                client.Email = model.Email;
                client.FirstName = model.FirstName;
                client.LastName = model.LastName;
                client.Password = "141516";
                _clientRepository.Save(client);
                return Created();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

