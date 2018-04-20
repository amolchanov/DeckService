using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;
using System;

namespace DeckService.Repository
{
    public class CosmosDbStorageRepositoryFactory<T> : IStorageRepositoryFactory<T> where T : class
    {
        private IConfiguration config;

        public CosmosDbStorageRepositoryFactory(IConfiguration config)
        {
            this.config = config;
        }

        public IStorageRepository<T> GetStorageRepository(bool isDevEnvironment)
        {
            string cosmosDbAuthKey;
            if (isDevEnvironment)
            {
                cosmosDbAuthKey = config["CosmosDbAuthKey"];
            }
            else
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                var getSecretTask = kv.GetSecretAsync(new Uri(new Uri(config["KeyVaultEndpoint"]), "/secrets/CosmosDbAuthKey").ToString());
                getSecretTask.Wait();
                cosmosDbAuthKey = getSecretTask.Result.Value;
            }

            var repository = new CosmosDBRepository<T>(
                config["CosmosDbEndpoint"],
                cosmosDbAuthKey,
                config["CosmosDbDatabaseId"],
                config["CosmosDbCollectionId"]
            );

            repository.Initialize().Wait();

            return repository;
        }
    }
}
