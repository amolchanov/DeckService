using System;
using Newtonsoft.Json;

namespace DeckService.Models
{
    public class Deck
    {
        private static Card[] AllCards = new Card[52];

        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "lastDealtCardIndex")]
        public int LastDealtCardIndex { get; set; }

        [JsonProperty(PropertyName = "cardsIndexies")]
        public int[] CardIndexies { get; }

        [JsonProperty(PropertyName = "_etag")]
        public string ETag { get; set; }

        public bool IsEmpty => this.LastDealtCardIndex == AllCards.Length;

        static Deck()
        {
            var index = 0;
            foreach (var rank in Enum.GetValues(typeof(Rank)))
            {
                foreach (var suit in Enum.GetValues(typeof(Suit)))
                {
                    AllCards[index++] = new Card((Rank)rank, (Suit)suit);
                }
            }
        }

        public Deck()
        {
            CardIndexies = new int[AllCards.Length];
            for (int i = 0; i < AllCards.Length; i++)
            {
                this.CardIndexies[i] = i;
            }
            Id = Guid.NewGuid();
        }

        public void Shuffle()
        {
            if (LastDealtCardIndex != 0)
            {
                throw new InvalidOperationException("One or more cards have been dealt. Deck cannot be suffled anymore.");
            }

            Random random = new Random();
            for(int i = this.CardIndexies.Length - 1; i > 0; i--)
            {
                int  randomIndex = random.Next(i - 1);
                var tmp = this.CardIndexies[i];
                this.CardIndexies[i] = this.CardIndexies[randomIndex];
                this.CardIndexies[randomIndex] = tmp;
            }
        }

        public void Cut()
        {
            Cut(new Random().Next(1, this.CardIndexies.Length - 2));
        }

        internal void Cut(int cutIndex)
        {
            if (LastDealtCardIndex != 0)
            {
                throw new InvalidOperationException("One or more cards have been dealt. Deck cannot be cut anymore.");
            }

            int startIndex = cutIndex;
            int mod = this.CardIndexies.Length % cutIndex;
            for (int i = 0; i < this.CardIndexies.Length - 1; i++)
            {
                if (startIndex == this.CardIndexies.Length)
                {
                    if (mod == 0)
                    {
                        break;
                    }

                    startIndex = this.CardIndexies.Length - mod;
                    mod = (this.CardIndexies.Length - i) % (startIndex - i);
                }

                var tmp = this.CardIndexies[i];
                this.CardIndexies[i] = this.CardIndexies[startIndex];
                this.CardIndexies[startIndex++] = tmp;
            }
        }

        public Card DealCard()
        {
            if (LastDealtCardIndex == this.CardIndexies.Length)
            {
                throw new InvalidOperationException("No more cards to deal. The deck is empty.");
            }

            return AllCards[this.CardIndexies[LastDealtCardIndex++]];
        }
    }
}
