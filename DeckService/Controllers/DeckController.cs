using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private IDeckRepository repository;

        public DeckController(IDeckRepository repository)
        {
            this.repository = repository;
        }

        // GET deck/cut
        [HttpGet("{id}")]
        public DeckResponse Cut(Guid id)
        {
            var deck = this.repository.Load(id);
            deck.Cut();
            this.repository.Save(deck);

            return new DeckResponse() {
                Id = id,
                Message = "The deck has been cut"
            };
        }

        // GET deck/deal
        [HttpGet("{id}")]
        public DealCardResponse Deal(Guid id)
        {
            var deck = this.repository.Load(id);
            var card = deck.DealCard();
            this.repository.Save(deck);

            return new DealCardResponse() {
                Id = id,
                Card = card.ToString(),
                Message = "The next card is dealt"
            };
        }

        // GET deck/new
        [HttpGet]
        public DeckResponse New()
        {
            var deck = Deck.NewDeck();
            this.repository.Save(deck);
            return new DeckResponse() {
                Id = deck.Id,
                Message = "A new deck has been created"
            };
        }

        // GET deck/shuffle
        [HttpGet("{id}")]
        public DeckResponse Shuffle(Guid id)
        {
            var deck = this.repository.Load(id);
            deck.Shuffle();
            this.repository.Save(deck);

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
