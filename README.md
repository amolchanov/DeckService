# Description

Sample .NET Core web service application to manage deck of cards running on Azure that uses Comsos DB for storage. All decks are created with a unique id in Guid format. The service supports multiple concurrent users operating independently on their decks by referencing them using unique id. The app uses Managed Service Identity to protect access to Azure Storage Vault that stores the connection string to Cosmos DB.

API reference:

- http://{app_name}.azurewebsites.net/v1.0/deck/new
	 - create a new deck
- http://{app_name}.azurewebsites.net/v1.0/deck/cut/{id}
	- cuts the deck
- http://{app_name}.azurewebsites.net/v1.0/deck/shuffle/{id}
	- shuffles the deck
- http://{app_name}.azurewebsites.net/v1.0/deck/deal/{id}
	- deals the next card from the deck
- http://{app_name}.azurewebsites.net/v1.0/deck/state/{id}
	- returns the state of the deck

# Environment variables

```console
set USER_NAME = username
set ACR_NAME = usernameacr
set RESOURCE_GROUP_NAME = username-resource-group
set APP_NAME = deck-service-webapp
set DNS_LABEL = deck-service-demo
set AZURE_LOCATION = westus
set DOCKER_TAG = deckservice
```

# Build the application


```console
docker build --pull -t %DOCKER_TAG% . -f DeckService/Dockerfile
```
# Run the application

```console
docker run --name %DOCKER_TAG% --rm -it -p 8000:80 %DOCKER_TAG%
```

# Push Docker Images to Azure Container Registry

Link: https://github.com/dotnet/dotnet-docker/blob/master/samples/dotnetapp/push-image-to-acr.md

```console
az login
az group create --name %RESOURCE_GROUP_NAME% --location %AZURE_LOCATION%
az acr create --name %ACR_NAME% --resource-group %RESOURCE_GROUP_NAME% --sku Basic
```

# Tag the image

```console
docker tag %DOCKER_TAG% %ACR_NAME%.azurecr.io/%DOCKER_TAG%
```

# Log to ACR

```console
az acr update -n %USER_NAME% --admin-enabled true
```

# To see credentials

```console
az acr credential show -n %USER_NAME%
```

# Persist creds

```console
az acr credential show -n %USER_NAME% --query passwords[0].value --output tsv > %USERPROFILE%\password-acr.txt
type %USERPROFILE%\password-acr.txt | docker login %ACR_NAME%.azurecr.io -u %USER_NAME% --password-stdin
```

# Push the image to ACR

```console
docker push %ACR_NAME%.azurecr.io/%DOCKER_TAG%
```

# Create resource group

```console
az group create --name %RESOURCE_GROUP_NAME% --location %AZURE_LOCATION%
```

# Deploy application 

Link: https://docs.microsoft.com/en-us/azure/container-instances/container-instances-tutorial-deploy-app

```console
az acr show --name %USER_NAME% --query loginServer
az acr credential show --name %USER_NAME% --query "passwords[0].value"
az container create --resource-group %RESOURCE_GROUP_NAME% --os-type Windows --name %APP_NAME% --image %USER_NAME%.azurecr.io/%DOCKER_TAG% --cpu 1 --memory 1 --registry-username %USER_NAME% --registry-password [password] --dns-name-label %DNS_LABEL% --ports 80
```

# View deployment progress

```console
az container show --resource-group %RESOURCE_GROUP_NAME% --name %APP_NAME% --query instanceView.state
az container show --resource-group %RESOURCE_GROUP_NAME% --name %APP_NAME% --query ipAddress.fqdn
az container logs --resource-group %RESOURCE_GROUP_NAME% --name %APP_NAME%
```

# Delete resource group

```console
az group delete --name %RESOURCE_GROUP_NAME%
```

# Update deployed container

Link: https://docs.microsoft.com/en-us/azure/container-registry/container-registry-tutorial-deploy-update

```console
docker build . -f ./Dockerfile -t %ACR_NAME%.azurecr.io/%DOCKER_TAG%
docker push %ACR_NAME%.azurecr.io/%DOCKER_TAG%
```

# Enable continuous deployment

```console
az container deployment container config --name %ACR_NAME% --resource-group %RESOURCE_GROUP_NAME% --enable-cd true
```

Link: https://docs.microsoft.com/en-us/azure/app-service/containers/app-service-linux-ci-cd

# TO-DO

- [ ] Add instructions on creating Cosmos DB
- [ ] Add instructions on setting up Azure KeyVault 
- [ ] Add instructions on configuring Managed Service Identity

# Other links

https://docs.microsoft.com/en-us/azure/container-instances/container-instances-using-azure-container-registry
https://stackoverflow.com/questions/41834111/azure-service-fabric-vs-azure-container-services
https://docs.microsoft.com/en-us/azure/aks/tutorial-kubernetes-app-update