using Newtonsoft.Json;

namespace DeckService.Models
{
    public class Card
    {
        [JsonProperty(PropertyName = "rank")]
        public Rank Rank { get; set; }

        [JsonProperty(PropertyName = "suit")]
        public Suit Suit { get; set; }

        public override string ToString()
        {
            return Rank.ToString() + " of " + Suit.ToString() + "s";
        }
    }
}
