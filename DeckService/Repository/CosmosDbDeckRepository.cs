using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using DeckService.Models;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DeckService.Repository
{
    public class CosmosDbDeckRepository : IDeckRepository
    {
        private static Dictionary<Guid, string> decks = new Dictionary<Guid, string>();
        private MongoClient client;
        private const string dbName = "Decks";
        private string collectionName = "Deck";

        public CosmosDbDeckRepository(string connectionString)
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(
              new MongoUrl(connectionString)
            );

            settings.SslSettings =
              new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };

            client = new MongoClient(settings);
        }

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
