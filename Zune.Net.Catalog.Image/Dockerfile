# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

#  cache the build result to speed up subsequent package steps
COPY ./ .
RUN dotnet restore

RUN dotnet publish -c release -p:PublishDir=./publish --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /source/Zune.Net.Catalog.Image/publish/ ./
ENV DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "Zune.Net.Catalog.Image.dll"]