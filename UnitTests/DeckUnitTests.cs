using System;
using System.Collections.Generic;
using System.Linq;
using DeckService.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class DeckServiceUnitTests
    {
        [TestMethod]
        public void TestNewDeckMethod()
        {
            var deck = new Deck();
            Assert.AreEqual(0, deck.NextCardIndex, "The new deck must have no cards dealt");
            AssertDeckHasAllExpectedCards(deck);
        }

        [TestMethod]
        public void TestCutMethod()
        {
            for (int cutIndex = 1; cutIndex <= 50; cutIndex++)
            {
                var deck = new Deck();
                deck.Shuffle();
                var cardsBeforeCutIndex = new List<int>();
                var cardsAfterCutIndex = new List<int>();
                for (int i = 0; i < deck.CardIndexies.Length; i++)
                {
                    if (i < cutIndex)
                    {
                        cardsBeforeCutIndex.Add(deck.CardIndexies[i]);
                    }
                    else
                    {
                        cardsAfterCutIndex.Add(deck.CardIndexies[i]);
                    }
                }

                deck.Cut(cutIndex);

                Func<int,string> contactFunc = (int c) => c + (c != deck.CardIndexies[deck.CardIndexies.Length - 1] ? "," : "");

                var actualCards = String.Concat(deck.CardIndexies.Select(c => contactFunc(c)));
                var expectedCards = String.Concat(cardsAfterCutIndex.Concat(cardsBeforeCutIndex).Select(c => contactFunc(c)));

                Assert.AreEqual(expectedCards, actualCards, "The deck wasn't cut properly");
                AssertDeckHasAllExpectedCards(deck);
            }
        }

        [TestMethod]
        public void TestShuffleDeckMethod()
        {
            var deck = new Deck();
            deck.Shuffle();
            AssertDeckHasAllExpectedCards(deck);
        }

        [TestMethod]
        public void TestDealCardMethod()
        {
            var deck = new Deck();
            deck.Shuffle();
            deck.Cut();

            for (var i = 0; i < deck.CardIndexies.Length; i++)
            {
                deck.DealCard();
            }

            AssertDeckHasAllExpectedCards(deck);
            Assert.IsTrue(deck.IsEmpty, "The deck should be empty after all cards are dealt");
        }

        private void AssertDeckHasAllExpectedCards(Deck deck)
        {
            for (int i = 0; i < 52; i++)
            {
                if (!deck.CardIndexies.Any(ci => ci == i))
                {
                    Assert.Fail("The deck doesn't have all required cards");
                }
            }
        }
    }
}
