using HomeBankingMinHub.DTOs;
using HomeBankingMinHub.Models;
using HomeBankingMinHub.Repositories;
using HomeBankingMinHub.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        public IClientRepository _clientRepository;
        private IClientService _clientService;
        public ClientsController (IClientRepository clientRepository, IClientService clientService)
        {
            _clientRepository = clientRepository;
            _clientService = clientService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var clients = _clientService.GetAllClients();
            if (clients == null) 
            { return NotFound(); }
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
                var client = _clientRepository.FindById(id);
                if (client == null)
                {
                    return NotFound();
                }
                return Ok(client);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ClientCreateDTO model)
        {
            return _clientService.CreateClient(model);
        }

        [HttpGet("/api/clients/current")]
        public IActionResult GetCurrent()
        {
            string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
            if (email == string.Empty)
            { return Forbid("usuario invalido"); }
            var client = _clientService.GetCurrent(email);
            if (client == null)
            { return NotFound("Cliente no encontrado"); }
            return Ok(client);
        }

        [HttpGet("/api/clients/current/accounts")]
        public IActionResult GetCurrentAccounts()
        {
            string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
            if (email == string.Empty)
            {return Forbid("usuario invalido");}
            var account = _clientService.GetCurrentAccount(email);
            if(account == null)
            {
                return NotFound("no se encontraron cuentas");
            }
            return Ok(account);
        }
    }
}



    


