using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DeckService.Models;
using DeckService.Repository;
using DeckService.Responses;
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
                new DeckResponse()
                {
                    Id = id,
                    Message = "The deck has been cut"
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
                new DealCardResponse()
                {
                    Id = id,
                    Card = card.ToString(),
                    Message = "The next card is dealt"
                }
            );
        }

        // GET deck/new/{id}
        [HttpGet]
        public async Task<IActionResult> New()
        {
            var deck = new Deck();
            await repository.CreateItemAsync(deck);

            return new ObjectResult(
                new DeckResponse() {
                    Id = deck.Id,
                    Message = "A new deck has been created"
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
                new DeckResponse()
                {
                    Id = id,
                    Message = "The deck has been suffled"
                }
            );
        }

        [Route("/error")]
        public IActionResult Error()
        {
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }
}
