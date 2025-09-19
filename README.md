# ZuneNetApi
This repository contains implementations of the now defunct Zune.net web services. Roughly each C# project titled `Zune.Net.*` corresponds with the original service domain, for example: `Zune.Net.Catalog` is an implementation of `catalog.zune.net`.

# Setup
## Community-hosted
It is highly advised to use the Community Webservices mod from [Zune Modding Helper](https://github.com/ZuneDev/ZuneModdingHelper/releases). This mod patches the official Zune desktop software to use versions of these servers hosted by the Zune community at [zunes.me](https://zunes.me).

## Local (for debugging)
It can be helpful during development to run the servers directly on your dev machine.

1. **Install a MongoDB server.** If you would like a GUI for exploring the database, consider [MongoDB Compass](https://www.mongodb.com/try/download/compass); otherwise, [MongoDB Community Edition](https://www.mongodb.com/try/download/community) should be sufficient. The ZuneNetApi services will automatically create the necessary collections.

2. **Install required system packages.** Follow Microsoft's instructions for your operating system to [install the .NET 8 SDK](https://learn.microsoft.com/en-us/dotnet/core/install/). To install `git`...
   - On Debian-based systems:
     ```sh
     sudo apt update
     sudo apt install -y git
     ```
   - On Windows systems with `winget`:
     ```sh
     winget install -e --id Git.Git 
     ```

3. **Download the ZuneNetApi source code:**
    ```sh
    git clone --recurse-submodules -j8 https://github.com/ZuneDev/ZuneNetApi.git
    cd ZuneNetApi
    ```

4. **Obtain API keys.** ZuneNetApi is simply a data aggregator-- to use it, you'll need to sign up for API keys from [Discogs](https://www.discogs.com/developers), [Last.fm](https://www.last.fm/api). [ListenNotes](https://www.listennotes.com/api/), and [Taddy](https://taddy.org/developers/podcast-api). The free tier for each service should be sufficient for local development.

5. **Populate your secret constants.** Create a file named `Constants.Secrets.cs` in the `Zune.Net.Shared` project directory. Use the following template, being sure to replace any placeholders with your API secrets.
   ```cs
   namespace Zune.Net;

   public static partial class Constants
   {
       public const string DC_API_KEY = "<Discogs API key>";
       public const string DC_API_SECRET = "<Discogs API secret>";
   
       public const string LN_API_KEY = "<ListenNotes API key>";
   
       public const string FM_API_KEY = "<Last.fm API key>";
       public const string FM_API_SECRET = "<Last.fm API key>";
   
       public const string TD_API_KEY = "<Taddy API key>";
       public const int TD_USER_ID = <Taddy user ID>;
   }
   ```
   
6. **Build and run the microservices.** This can be done for a single project from your IDE of choice or from a terminal. For example, to run the Catalog service:
   ```sh
   dotnet run --project Zune.Net.Catalog
   ```
   On Windows, if you wish to start all available services, you can run `./scripts/run_all.ps1`. 

## Docker
Using the pre-made Docker containers is recommended if you wish to deploy your own complete instance of ZuneNetApi.

1. **Install Docker.** Follow the [official instructions](https://docs.docker.com/engine/install/) for your host system.
2. **Set up source code.** Follow steps 2 through 5 for [local setup](#local-for-debugging) to download and prepare the ZuneNetApi source code for building.
3. **Configure containers.** In `./scripts/dockerSecrets.sh`, update the following variables:
    - `DOMAIN_ROOT` with the root domain you intend to host the servers at. For example, the community-hosted services use `zunes.me`, which results in microservices running at `catalog.zunes.me`.
    - `MONGO_INITDB_ROOT_PASSWORD` with a unique and secure password.
4. **Configure Let's Encrypt SSL.** In `./traefik/traefik.yml`, set the `letsencrypt` ACME email to your own address.
5. **Build and run the Docker containers.** On Linux-based systems, it is recommended to use `./scripts/dockerBuild.sh`. Otherwise, each step in that script can be performed manually as appropriate for your system.

# Unimplemented endpoints

## Catalog
### Artist
```
/music/artist/{mbid}/musicVideos
/music/artist/{mbid}/playlists
/music/artist/{mbid}/appearsOnAlbums
/music/artist/{mbid}/relatedAlbums
/music/artist/{mbid}/influencers
/music/artist/{mbid}/events
```

### Music hub
```
/music/chart/zune/playlists
/music/chart/musicVideos
```

### Video hub
```
/music/hub/video
/tv/chart/zune/episodes
/chart/zuneDownload/movie
```

### Podcast hub
```
/music/hub/podcast
/podcastchart/zune/podcasts
/podcast?url={rssUrl}
/podcastCategories/{catId}
```

### Channel hub
```
/music/channel/categories
/music/hub/channel
```

### App hub
```
/appCategories
/apps?cost={paid|free}&store=Zest&orderby={downloadRank|releaseDate}
/clientTypes/hubTypes/apps/hub
```

### Search
```
/?prefix={query}&includeTracks=true&includeAlbums=true&includeArtists=true&includeMovies=true&includeVideoShorts=true&includeTVSeries=true&includeMusicVideos=true&includePodcasts=true&includeApplications=true&clientType=PC/Windows

/music/playlist?q={query}
/music/musicvideo?q={query}
/movie?q={query}
/tv/short?q={query}
/tv/series?q={query}
/apps?q={query}
```

## Social API
```
/music/artist/{mbid}/toplisteners
/members?q={query}
```