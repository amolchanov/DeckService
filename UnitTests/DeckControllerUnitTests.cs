using System;
using System.Threading.Tasks;
using DeckService.Contants;
using DeckService.Controllers;
using DeckService.Models;
using DeckService.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class DeckControllerUnitTests
    {
        private Mock<IHostingEnvironment> hostingEnv;
        private Mock<IStorageRepositoryFactory<Deck>> storageRepoFactory;
        private Mock<IStorageRepository<Deck>> storageRepo;
        private Deck deck;

        [TestInitialize]
        public void TestInitialize()
        {
            this.deck = new Deck();
            this.hostingEnv = new Mock<IHostingEnvironment>();
            this.storageRepoFactory = new Mock<IStorageRepositoryFactory<Deck>>();
            this.storageRepo = new Mock<IStorageRepository<Deck>>();
            this.storageRepo.Setup(m => m.CreateItemAsync(It.IsAny<Deck>())).ReturnsAsync(new Microsoft.Azure.Documents.Document());
            this.storageRepo.Setup(m => m.UpdateItemAsync(It.IsAny<string>(), It.IsAny<Deck>())).ReturnsAsync(new Microsoft.Azure.Documents.Document());
            this.storageRepo.Setup(m => m.GetItemAsync(It.IsAny<string>())).ReturnsAsync(deck);
            this.storageRepo.Setup(m => m.DeleteItemAsync(It.IsAny<string>())).ReturnsAsync(new Microsoft.Azure.Documents.Document());
            this.storageRepoFactory.Setup(m => m.GetStorageRepository(It.IsAny<bool>())).Returns(this.storageRepo.Object);
        }

        [TestMethod]

        public async Task NewActionTest()
        {
            var controller = new DeckController(this.hostingEnv.Object, this.storageRepoFactory.Object);
            var response = await controller.New();
            Assert.IsInstanceOfType(response, typeof(ObjectResult));
            var objectResult = response as ObjectResult;
            dynamic value = objectResult.Value;
            Assert.IsInstanceOfType(value.Id, typeof(Guid), "Id property is not Guid");
            Assert.AreEqual(value.Message, StringMessages.NewDeckHasBeenCreated);
        }

        [TestMethod]

        public async Task CutActionTest()
        {
            var controller = new DeckController(this.hostingEnv.Object, this.storageRepoFactory.Object);
            var response = await controller.Cut(deck.Id);
            Assert.IsInstanceOfType(response, typeof(ObjectResult));
            var objectResult = response as ObjectResult;
            dynamic value = objectResult.Value;
            Assert.AreEqual(value.Id, deck.Id, "Id doesn't match");
            Assert.AreEqual(value.Message, StringMessages.DeckHasBeenCut);
        }

        [TestMethod]
        public async Task DealActionTest()
        {
            var controller = new DeckController(this.hostingEnv.Object, this.storageRepoFactory.Object);
            var response = await controller.Deal(deck.Id);
            Assert.IsInstanceOfType(response, typeof(ObjectResult));
            var objectResult = response as ObjectResult;
            dynamic value = objectResult.Value;
            Assert.AreEqual(value.Id, deck.Id, "Id doesn't match");
            Assert.AreEqual(value.Message, StringMessages.CardIsDealt);
            Assert.AreEqual(value.Card, this.deck[this.deck.NextCardIndex - 1].ToString());
        }

        [TestMethod]
        public async Task ShuffleActionTest()
        {
            var controller = new DeckController(this.hostingEnv.Object, this.storageRepoFactory.Object);
            var response = await controller.Shuffle(deck.Id);
            Assert.IsInstanceOfType(response, typeof(ObjectResult));
            var objectResult = response as ObjectResult;
            dynamic value = objectResult.Value;
            Assert.AreEqual(value.Id, deck.Id, "Id doesn't match");
            Assert.AreEqual(value.Message, StringMessages.DeckHasBeenShuffled);
        }

        [TestMethod]
        public async Task StateActionTest()
        {
            var dealtCard = this.deck.DealCard();

            var controller = new DeckController(this.hostingEnv.Object, this.storageRepoFactory.Object);
            var response = await controller.State(deck.Id);
            Assert.IsInstanceOfType(response, typeof(ObjectResult));
            var objectResult = response as ObjectResult;
            dynamic value = objectResult.Value;
            Assert.AreEqual(value.Id, deck.Id, "Id doesn't match");
            Assert.AreEqual(value.CardsRemaining, 51);
            Assert.AreEqual(value.CardsDealt, 1);
            Assert.AreEqual(value.RemainingCards.Length, 51, "Unexpected quantity of RemainingCards");
            Assert.AreEqual(value.DealtCards.Length, 1, "Unexpected quantity of DealtCards");
            Assert.AreEqual(value.DealtCards[0], dealtCard.ToString(), "Unexpected dealt card");
        }
    }
}