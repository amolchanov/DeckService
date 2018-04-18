using DeckService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeckService.Repository
{
    public interface IDeckRepository
    {
        void Save(Deck deck);

        Deck Load(Guid id);
    }
}
