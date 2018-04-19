using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace DeckService.Repository
{
    public class CosmosDBRepository<T> : IStorageRepository<T> where T : class
    {
        private string databaseId;
        private string collectionId;
        private DocumentClient client;

        public CosmosDBRepository(string endpoint, string authKey, string databaseId, string collectionId)
        {
            client = new DocumentClient(new Uri(endpoint), authKey);
            this.databaseId = databaseId;
            this.collectionId = collectionId;

        }

        public async Task Initialize()
        {
            await CreateDatabaseIfNotExistsAsync();
            await CreateCollectionIfNotExistsAsync();
        }

        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                Document document =
                    await this.client.ReadDocumentAsync(
                        UriFactory.CreateDocumentUri(this.databaseId, this.collectionId, id));

                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }
        public async Task<Document> CreateItemAsync(T item)
        {
            return await this.client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(this.databaseId, this.collectionId), item);
        }

        public async Task<Document> UpdateItemAsync(string id, T item)
        {
            return await this.client.ReplaceDocumentAsync(
                UriFactory.CreateDocumentUri(
                    this.databaseId,
                    this.collectionId, id),
                item,
                new RequestOptions() {
                    AccessCondition = new AccessCondition() {
                        Condition = ((dynamic) item).ETag,
                        Type = AccessConditionType.IfMatch
                    }
                });
        }

        public async Task DeleteItemAsync(string id)
        {
            await this.client.DeleteDocumentAsync(
                UriFactory.CreateDocumentUri(this.databaseId, this.collectionId, id));
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await this.client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(this.databaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await this.client.CreateDatabaseAsync(new Database { Id = this.databaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await this.client.ReadDocumentCollectionAsync(
                    UriFactory.CreateDocumentCollectionUri(this.databaseId, this.collectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(this.databaseId),
                        new DocumentCollection
                            {
                                Id = this.collectionId
                            },
                        new RequestOptions { OfferThroughput = 400 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
