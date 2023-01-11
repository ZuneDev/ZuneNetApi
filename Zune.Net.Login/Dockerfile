# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY ./ .
RUN dotnet restore
RUN dotnet build --no-restore

WORKDIR /source/Zune.Net.Login
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./
ENV DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "Zune.Net.Login.dll"]
