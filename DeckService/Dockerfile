FROM microsoft/aspnetcore:2.0-nanoserver-sac2016 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0-nanoserver-sac2016 AS build
WORKDIR /src
COPY DeckService.sln ./
COPY DeckService/DeckService.csproj DeckService/
COPY UnitTests/*.csproj UnitTests/

RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/DeckService
RUN dotnet build -c Release -o /app

FROM build AS testrunner
WORKDIR /src/UnitTests
ENTRYPOINT ["dotnet", "test", "--logger:trx"]

FROM build AS test
WORKDIR /src/UnitTests
RUN dotnet test

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DeckService.dll"]