using System;
using System.Threading.Tasks;
using DeckService.Models;
using DeckService.Repository;
using DeckService.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DeckService.Controllers
{
    [Route("v1.0/[controller]/[action]")]
    public class DeckController : Controller
    {
        private IStorageRepository<Deck> repository;

        public DeckController(IStorageRepository<Deck> repository)
        {
            this.repository = repository;
        }

        // GET deck/cut
        [HttpGet("{id}")]
        public async Task<DeckResponse> Cut(Guid id)
        {
            var deck = await this.repository.GetItemAsync(id.ToString());
            deck.Cut();
            await this.repository.UpdateItemAsync(id.ToString(), deck);

            return new DeckResponse() {
                Id = id,
                Message = "The deck has been cut"
            };
        }

        // GET deck/deal
        [HttpGet("{id}")]
        public async Task<DealCardResponse> Deal(Guid id)
        {
            var deck = await this.repository.GetItemAsync(id.ToString());
            var card = deck.DealCard();
            await this.repository.UpdateItemAsync(id.ToString(), deck);

            return new DealCardResponse() {
                Id = id,
                Card = card.ToString(),
                Message = "The next card is dealt"
            };
        }

        // GET deck/new
        [HttpGet]
        public async Task<DeckResponse> New()
        {
            var deck = Deck.NewDeck();
            await this.repository.CreateItemAsync(deck);

            return new DeckResponse() {
                Id = deck.Id,
                Message = "A new deck has been created"
            };
        }

        // GET deck/shuffle
        [HttpGet("{id}")]
        public async Task<DeckResponse> Shuffle(Guid id)
        {
            var deck = await this.repository.GetItemAsync(id.ToString());
            deck.Shuffle();
            await this.repository.UpdateItemAsync(id.ToString(), deck);

            return new DeckResponse() {
                Id = id,
                Message = "The deck has been suffled"
            };
        }

        [Route("/error")]
        public ErrorResponse Error()
        {
            return new ErrorResponse()
            {
                Message = "An internal server error occured",
                ErrorCode = "InternalServerError"
            };
        }
    }
}
