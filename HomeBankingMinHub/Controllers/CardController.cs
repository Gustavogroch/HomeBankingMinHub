using HomeBankingMinHub.Repositories;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HomeBankingMinHub.Services;
using HomeBankingMinHub.DTOs;
using HomeBankingMinHub.Models;
using System.Linq;

namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly ICardRepository _cardRepository;
        private readonly IClientRepository _clientRepository;
        private readonly CardService _cardService;

        public CardController(ICardRepository cardRepository, IClientRepository clientRepository, CardService cardService)
        {
            _cardRepository = cardRepository;
            _clientRepository = clientRepository;
            _cardService = cardService;
        }

        [HttpPost ("/api/clients/current/cards")]
        public IActionResult CreateCard([FromBody] CardDTO cardDTO)
        {
            try
            {
                string email = User.FindFirst("Client")?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Forbid();
                }

                // Buscar el cliente
                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                {
                    return Forbid();
                }


                int numberOfCards = _cardRepository.GetCardsByClient(client.Id, cardDTO.Type).Count();
                if (numberOfCards >= 3)
                {
                    return StatusCode(403, $"El cliente ya tiene 3 tarjetas de tipo {cardDTO.Type}");
                }
                string cardNumber = _cardService.GenerateCardNumber();
                int cvv = _cardService.GenerateCvv();
                var newCard = new Card
                {
                    Type = cardDTO.Type,
                    Color = cardDTO.Color,
                    NameCardHolder = client.FirstName,
                    Number = cardNumber,
                    Cvv = cvv,
                    FromDate = DateTime.Now,
                    ThruDate = DateTime.Now.AddYears(5),
                    ClientId = client.Id,
                };

                _cardRepository.Save(newCard);
                return StatusCode(201, "Tarjeta creada exitosamente");
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
