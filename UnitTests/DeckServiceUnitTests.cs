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
            var deck = Deck.NewDeck();
            Assert.AreEqual(0, deck.CardsDealt, "The new deck must have no cards dealt");
            AssertDeckHasExpectedAllCards(deck);
        }

        [TestMethod]
        public void TestCutMethod()
        {
            for (int cutIndex = 1; cutIndex <= 50; cutIndex++)
            {
                var deck = Deck.NewDeck();
                deck.Shuffle();
                var cardsBeforeCutIndex = new List<Card>();
                var cardsAfterCutIndex = new List<Card>();
                for (int i = 0; i < deck.Cards.Length; i++)
                {
                    if (i < cutIndex)
                    {
                        cardsBeforeCutIndex.Add(deck.Cards[i]);
                    }
                    else
                    {
                        cardsAfterCutIndex.Add(deck.Cards[i]);
                    }
                }

                deck.Cut(cutIndex);

                Func<Card, string> contactFunc = (Card c) => c + (c != deck.Cards[deck.Cards.Length - 1] ? "," : "");

                var actualCards = String.Concat(deck.Cards.Select(c => contactFunc(c)));
                var expectedCards = String.Concat(cardsAfterCutIndex.Concat(cardsBeforeCutIndex).Select(c => contactFunc(c)));

                Assert.AreEqual(expectedCards, actualCards, "The deck wasn't cut properly");
                AssertDeckHasExpectedAllCards(deck);
            }
        }

        [TestMethod]
        public void TestShuffleDeckMethod()
        {
            var deck = Deck.NewDeck();
            deck.Shuffle();
            AssertDeckHasExpectedAllCards(deck);
        }

        [TestMethod]
        public void TestDealCardMethod()
        {
            var deck = Deck.NewDeck();
            deck.Shuffle();
            deck.Cut();

            for (var i = 0; i < deck.Cards.Length; i++)
            {
                deck.DealCard();
            }

            AssertDeckHasExpectedAllCards(deck);
            Assert.IsTrue(deck.IsEmpty, "The deck should be empty after all cards are dealt");
        }

        private void AssertDeckHasExpectedAllCards(Deck deck)
        {
            foreach (var rank in Enum.GetValues(typeof(Rank)))
            {
                foreach (var suit in Enum.GetValues(typeof(Suit)))
                {
                    if (!deck.Cards.Any(c => c.Suit == (Suit)suit && c.Rank == (Rank)rank))
                    {
                        Assert.Fail("The deck doesn't have all required cards");
                    }
                }
            }
        }
    }
}
