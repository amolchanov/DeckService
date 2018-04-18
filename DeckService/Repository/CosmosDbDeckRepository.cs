using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DeckService.Models;
using Newtonsoft.Json;

namespace DeckService.Repository
{
    public class CosmosDbDeckRepository : IDeckRepository
    {
        private static Dictionary<Guid, string> decks = new Dictionary<Guid, string>();

        public Deck Load(Guid id)
        {
            return JsonSerializer.Create().Deserialize<Deck>(new JsonTextReader(new StringReader(decks[id])));
        }

        public void Save(Deck deck)
        {
            StringWriter writer = new StringWriter();
            JsonSerializer.Create().Serialize(new JsonTextWriter(writer), deck);

            if (decks.ContainsKey(deck.Id))
            {
                decks[deck.Id] = writer.ToString();
            }
            else
            {
                decks.Add(deck.Id, writer.ToString());
            }
        }
    }
}
