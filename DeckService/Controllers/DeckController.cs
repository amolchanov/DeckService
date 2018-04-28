using System;
using System.Net;
using System.Threading.Tasks;
using DeckService.Contants;
using DeckService.Models;
using DeckService.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DeckService.Controllers
{
    [Route("v1.0/[controller]/[action]")]
    public class DeckController : Controller
    {
        private static IStorageRepository<Deck> repository;

        public DeckController(IHostingEnvironment env, IStorageRepositoryFactory<Deck> repositoryFactory)
        {
            repository = repositoryFactory.GetStorageRepository(env.IsDevelopment());
        }

        // GET deck/cut/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Cut(Guid id)
        {
            var deck = await repository.GetItemAsync(id.ToString());
            try
            {
                deck.Cut();
            }
            catch(InvalidOperationException e)
            {
                return new BadRequestObjectResult(new { e.Message });
            }

            await repository.UpdateItemAsync(id.ToString(), deck);

            return new ObjectResult(
                new
                {
                    Id = id,
                    Message = StringMessages.DeckHasBeenCut
                }
            );
        }

        // GET deck/deal/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Deal(Guid id)
        {
            var deck = await repository.GetItemAsync(id.ToString());
            Card card = null;
            try
            {
                card = deck.DealCard();
            }
            catch (InvalidOperationException e)
            {
                return new BadRequestObjectResult(new { e.Message });
            }

            await repository.UpdateItemAsync(id.ToString(), deck);

            return new ObjectResult(
                new
                {
                    Id = id,
                    Card = card.ToString(),
                    Message = StringMessages.CardIsDealt
                }
            );
        }

        [Route("/error")]
        public IActionResult Error()
        {
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }

        // GET deck/new/{id}
        [HttpGet]
        public async Task<IActionResult> New()
        {
            var deck = new Deck();
            await repository.CreateItemAsync(deck);

            return new ObjectResult(
                new {
                    Id = deck.Id,
                    Message = StringMessages.NewDeckHasBeenCreated
                }
            );
        }

        // GET deck/shuffle/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Shuffle(Guid id)
        {
            var deck = await repository.GetItemAsync(id.ToString());
            try
            {
                deck.Shuffle();
            }
            catch (InvalidOperationException e)
            {
                return new BadRequestObjectResult(new { e.Message });
            }

            await repository.UpdateItemAsync(id.ToString(), deck);

            return new ObjectResult(
                new
                {
                    Id = id,
                    Message = StringMessages.DeckHasBeenShuffled
                }
            );
        }

        // GET deck/state/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> State(Guid id)
        {
            var deck = await repository.GetItemAsync(id.ToString());
            var response = new
            {
                Id = deck.Id,
                CardsDealt = deck.NextCardIndex,
                CardsRemaining = deck.CardIndexies.Length - deck.NextCardIndex,
                RemainingCards = new string[deck.CardIndexies.Length - deck.NextCardIndex],
                DealtCards = new string[deck.NextCardIndex]
            };

            for (int i = deck.NextCardIndex; i < deck.CardIndexies.Length; i++)
            {
                response.RemainingCards[i - deck.NextCardIndex] = deck[i].ToString();
            }

            for (int i = 0; i < deck.NextCardIndex; i++)
            {
                response.DealtCards[i] = deck[i].ToString();
            }

            return new ObjectResult(response);
        }
    }
}
