# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

#  cache the build result to speed up subsequent package steps
COPY ./ .
RUN dotnet restore ./Zune.Net.Commerce/Zune.Net.Commerce.csproj
RUN dotnet publish -c release -o /app --no-restore ./Zune.Net.Commerce/Zune.Net.Commerce.csproj

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "Zune.Net.Commerce.dll"]
