namespace DeckService.Models
{
    public class Card
    {
        public Card(Rank rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        public Rank Rank { get; private set; }

        public Suit Suit { get; private set; }

        public override string ToString()
        {
            return Rank.ToString() + " of " + Suit.ToString() + "s";
        }
    }
}
