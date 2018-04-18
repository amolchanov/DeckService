using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeckService.Models
{
    public class Card
    {
        public Rank Rank { get; set; }
        public Suit Suit { get; set; }

        public int Index { get; set; }

        public override string ToString()
        {
            return Rank.ToString() + " of " + Suit.ToString() + "s " + Index;
        }
    }
}
