# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY ./ .
RUN dotnet restore

RUN dotnet publish -c release -p:PublishDir=./publish --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /source/Zune.Net.Tiles/publish/ ./
ENV DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "Zune.Net.Tiles.dll"]
