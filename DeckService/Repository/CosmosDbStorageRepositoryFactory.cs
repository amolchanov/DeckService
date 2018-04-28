using System;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;

namespace DeckService.Repository
{
    public class CosmosDbStorageRepositoryFactory<T> : IStorageRepositoryFactory<T> where T : class
    {
        private static object LockObj = new object();
        private IConfiguration config;
        private static IStorageRepository<T> Repository;

        public CosmosDbStorageRepositoryFactory(IConfiguration config)
        {
            this.config = config;
        }

        public IStorageRepository<T> GetStorageRepository(bool isDevEnvironment)
        {
            if (Repository == null)
            {
                lock(LockObj)
                {
                    if (Repository == null)
                    {
                        string cosmosDbAuthKey;
                        if (isDevEnvironment)
                        {
                            cosmosDbAuthKey = this.config["CosmosDbAuthKey"];
                        }
                        else
                        {
                            var azureServiceTokenProvider = new AzureServiceTokenProvider();
                            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                            var getSecretTask = kv.GetSecretAsync(new Uri(new Uri(this.config["KeyVaultEndpoint"]), "/secrets/CosmosDbAuthKey").ToString());
                            getSecretTask.Wait();
                            cosmosDbAuthKey = getSecretTask.Result.Value;
                        }

                        Repository = new CosmosDBRepository<T>(
                            this.config["CosmosDbEndpoint"],
                            cosmosDbAuthKey,
                            this.config["CosmosDbDatabaseId"],
                            this.config["CosmosDbCollectionId"]
                        );

                        Repository.Initialize().Wait();
                    }
                }
            }
            return Repository;
        }
    }
}
