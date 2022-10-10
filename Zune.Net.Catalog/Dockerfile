#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Zune.Net.Catalog/Zune.Net.Catalog.csproj", "Zune.Net.Catalog/"]
COPY ["Zune.Net.Shared/Zune.Net.Shared.csproj", "Zune.Net.Shared/"]
COPY ["Backing/lastfm/src/IF.Lastfm.Core/IF.Lastfm.Core.csproj", "Backing/lastfm/src/IF.Lastfm.Core/"]
COPY ["Zune.Xml/Zune.Xml.csproj", "Zune.Xml/"]
COPY ["Atom/Atom.csproj", "Atom/"]
COPY ["Zune.DB/Zune.DB.csproj", "Zune.DB/"]
RUN dotnet restore "Zune.Net.Catalog/Zune.Net.Catalog.csproj"
COPY . .
WORKDIR "/src/Zune.Net.Catalog"
RUN dotnet build "Zune.Net.Catalog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Zune.Net.Catalog.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Zune.Net.Catalog.dll"]