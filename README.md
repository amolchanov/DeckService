# Build the application

Link: https://github.com/dotnet/dotnet-docker/tree/master/samples/dotnetapp
Link: https://github.com/dotnet/dotnet-docker/blob/master/samples/aspnetapp/README.md#view-the-aspnet-core-app-in-a-running-container-on-windows

```console
docker build --pull -t deckservice . -f DeckService/Dockerfile
```
# Run the application

```console
docker run --name deckservice --rm -it -p 8000:80 deckservice
```

# Run the application in a container while you Develop

This didn't work. TODO: Figure out why.

```console
docker run --rm -it -v e:\Projects\DeckService\DeckService:c:\app\ -w \app\DeckService microsoft/dotnet-nightly:2.1-sdk dotnet watch run
```

# Push Docker Images to Azure Container Registry

Link: https://github.com/dotnet/dotnet-docker/blob/master/samples/dotnetapp/push-image-to-acr.md

```console
az login
az group create --name alexeim-containers --location westus
az acr create --name alexeim --resource-group alexeim-containers --sku Basic
```

# Tag the image

```console
docker tag deckservice alexeim.azurecr.io/deckservice
```

# Log to ACR

```console
az acr update -n alexeim --admin-enabled true
```

# To see credentials

```console
az acr credential show -n alexeim
```

# Persist creds

```console
az acr credential show -n alexeim --query passwords[0].value --output tsv > %USERPROFILE%\password-acr.txt
type %USERPROFILE%\password-acr.txt | docker login alexeim.azurecr.io -u alexeim --password-stdin
```

# Push the image to ACR

```console
docker push alexeim.azurecr.io/deckservice
```

# Create resource group

```console
az group create --name deck-service-resource-group --location westus
```

# Deploy application 

Link: https://docs.microsoft.com/en-us/azure/container-instances/container-instances-tutorial-deploy-app

```console
az acr show --name alexeim --query loginServer
az acr credential show --name alexeim --query "passwords[0].value"
az container create --resource-group deck-service-resource-group --os-type Windows --name deck-service-app --image alexeim.azurecr.io/deckservice --cpu 1 --memory 1 --registry-username alexeim --registry-password [password] --dns-name-label deck-service-demo --ports 80
```

# View deployment progress

```console
az container show --resource-group deck-service-resource-group --name deck-service-app --query instanceView.state
az container show --resource-group deck-service-resource-group --name deck-service-app --query ipAddress.fqdn
az container logs --resource-group deck-service-resource-group --name deck-service-app
```

# Delete resource group

```console
az group delete --name [resource group name]
```

# Update deployed container

Link: https://docs.microsoft.com/en-us/azure/container-registry/container-registry-tutorial-deploy-update

```console
docker build . -f ./Dockerfile -t alexeim.azurecr.io/deckservice
docker push alexeim.azurecr.io/deckservice
```

# Enable continuous deployment

```console
az container deployment container config --name deck-service-app --resource-group deck-service-resource-group --enable-cd true
```

Link: https://docs.microsoft.com/en-us/azure/app-service/containers/app-service-linux-ci-cd


# Other links

https://docs.microsoft.com/en-us/azure/container-instances/container-instances-using-azure-container-registry
https://stackoverflow.com/questions/41834111/azure-service-fabric-vs-azure-container-services
https://docs.microsoft.com/en-us/azure/aks/tutorial-kubernetes-app-update