using System;
using Newtonsoft.Json;

namespace DeckService.Models
{
    public class Deck
    {
        private static Card[] AllCards = new Card[52];

        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "nextCardIndex")]
        public int NextCardIndex { get; set; }

        [JsonProperty(PropertyName = "cardIndexies")]
        public int[] CardIndexies { get; }

        [JsonProperty(PropertyName = "_etag")]
        public string ETag { get; set; }

        [JsonIgnore]
        public Card this[int i]
        {
            get
            {
                if (i <0 || i >= this.CardIndexies.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(i));
                }

                return AllCards[this.CardIndexies[i]];
            }
        }

        public bool IsEmpty => this.NextCardIndex == AllCards.Length;

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
            this.CardIndexies = new int[AllCards.Length];
            for (int i = 0; i < AllCards.Length; i++)
            {
                this.CardIndexies[i] = i;
            }
            this.Id = Guid.NewGuid();
        }

        public void Shuffle()
        {
            if (this.NextCardIndex != 0)
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
            this.Cut(new Random().Next(1, this.CardIndexies.Length - 2));
        }

        internal void Cut(int cutIndex)
        {
            if (this.NextCardIndex != 0)
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
            if (this.NextCardIndex == this.CardIndexies.Length)
            {
                throw new InvalidOperationException("No more cards to deal. The deck is empty.");
            }

            return AllCards[this.CardIndexies[this.NextCardIndex++]];
        }
    }
}
