using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeckService.Models
{
    public class Deck
    {
        public Guid Id { get; set; }

        public int CardsDealt { get; set; }

        public Card[] Cards { get; set; }

        public Card[] DealtCards { get; set; }

        public bool IsEmpty
        {
            get
            {
                return CardsDealt == 52;
            }
        }

        private Deck()
        {
            Cards = new Card[52];
            DealtCards = new Card[52];
            Id = Guid.NewGuid();
        }

        public static Deck NewDeck()
        {
            var deck = new Deck();
            var index = 0;
            foreach (var rank in Enum.GetValues(typeof(Rank)))
            {
                foreach (var suit in Enum.GetValues(typeof(Suit)))
                {
                    deck.Cards[index++] = new Card() {
                        Suit = (Suit)suit,
                        Rank = (Rank)rank,
                        Index = index
                    };
                }
            }
            return deck;
        }

        public void Shuffle()
        {
            Random random = new Random();
            for(int i = this.Cards.Length - 1; i > 0; i--)
            {
                int  randomIndex = random.Next(i - 1);
                var tmp = this.Cards[i];
                this.Cards[i] = this.Cards[randomIndex];
                this.Cards[randomIndex] = tmp;
            }
        }

        public void Cut()
        {
            Cut(new Random().Next(1, this.Cards.Length - 2));
        }

        internal void Cut(int cutIndex)
        {
            if (CardsDealt != 0)
            {
                throw new InvalidOperationException("One or more cards have been dealt. Deck cannot be cut anymore.");
            }

            int startIndex = cutIndex;
            int mod = this.Cards.Length % cutIndex;
            for (int i = 0; i < this.Cards.Length - 1; i++)
            {
                if (startIndex == this.Cards.Length)
                {
                    if (mod == 0)
                    {
                        break;
                    }

                    startIndex = this.Cards.Length - mod;
                    mod = (this.Cards.Length - i) % (startIndex - i);
                }

                var tmp = this.Cards[i];
                this.Cards[i] = this.Cards[startIndex];
                this.Cards[startIndex++] = tmp;
            }
        }

        public Card DealCard()
        {
            if (CardsDealt == this.Cards.Length)
            {
                throw new InvalidOperationException("No more cards to deal. The deck is empty.");
            }
            this.DealtCards[CardsDealt] = this.Cards[CardsDealt];
            return this.Cards[CardsDealt++];
        }
    }
}
