# ZuneNetApi
This repository contains implementations of the now defunct Zune.net web services. Roughly each C# project titled `Zune.Net.*` corresponds with the original service domain, for example: `Zune.Net.Catalog` is an implementation of `catalog.zune.net`.

# Using these servers
It is highly advised to use the Community Webservices mod from [Zune Modding Helper](https://github.com/ZuneDev/ZuneModdingHelper/releases). This mod patches the official Zune desktop software to use versions of these servers hosted by the Zune community.

If you would rather host the servers yourself, you can add the following to your `hosts` file to redirect all requests made to the Zune.net endpoints to your local servers. Make sure you have also modified each project's `launchSettings.json` to use the correct `applicationUrl`.
```
127.0.0.1 fai.music.metaservices.microsoft.com
127.0.0.2 catalog.zune.net
127.0.0.2 catalog-ssl.zune.net
127.0.0.3 commerce.zune.net
127.0.0.4 image.catalog.zune.net
127.0.0.5 socialapi.zune.net
127.0.0.6 comments.zune.net
127.0.0.7 inbox.zune.net
127.0.0.8 mix.zune.net
127.0.0.9 stats.zune.net
127.0.0.10 cache-tiles.zune.net
127.0.0.11 tuners.zune.net
127.0.0.12 tiles.zune.net
127.0.0.13 social.zune.net
127.0.0.14 login.zune.net
```

# hosting these servers on linux

to use these first install `git`, `screen` and the `dotnet` runtime. installing screen is easy just run:
```shell
sudo apt update && sudo apt upgrade -y
sudo apt install screen git
```

to install dotnet please look at https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu

now to run the services you can do:

```shell
cd /opt/; git clone https://github.com/ZuneDev/ZuneNetApi.git && cd ./ZuneNetApi
bash ./run_all.sh
```

and to stop them all you can do
```shell
bash ./stop_all.sh
```

They will all run in their own screen sessions, which means you can close your SSH connection, and they will continue to run. Basic commands for managing screen sessions are:

To list all the running screens:

```shell
screen -ls
```

To reconnect to a specific screen session:

```shell
screen -rd <name>
```
look at the screen documentation for more information.


To access these services, you will need to use a proxy like Nginx and configure the following ports to point to the respective domains. Please note that if you are using your own domain, it cannot be more or less than 7 characters long.

```
8001 catalog.zunes.me
8802 catalog-ssl.zunes.me
8002 commerce.zunes.me
8003 image.catalog.zunes.me
8004 socialapi.zunes.me
8005 mix.zunes.me
8006 tiles.zunes.me
8007 login.zunes.me
```

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
/music/featured/albums
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
/podcast?q={query}
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