# ZuneNetApi
This repository contains implementations of the now defunct Zune.net web services. Roughly each C# project titled `Zune.Net.*` corresponds with the original service domain, for example: `Zune.Net.Catalog` is an implementation of `catalog.zune.net`.

# Using these servers
It is highly advised to use the Community Webservices mod from [Zune Modding Helper](https://github.com/ZuneDev/ZuneModdingHelper/releases). This mod patches the official Zune desktop software to use versions of these servers hosted by the Zune community.

If you would rather host the servers yourself, you can add the following to your `hosts` file to redirect all requests made to the Zune.net endpoints to your local servers. Make sure you have also modified each project's `launchSettings.json` to use the correct `applicationUrl`.
```
127.0.0.1 fai.music.metaservices.microsoft.com
127.0.0.2 catalog.zunes.me
127.0.0.2 catalog-ssl.zunes.me
127.0.0.3 commerce.zunes.me
127.0.0.4 image.catalog.zunes.me
127.0.0.5 socialapi.zunes.me
127.0.0.6 comments.zunes.me
127.0.0.7 inbox.zunes.me
127.0.0.8 mix.zunes.me
127.0.0.9 stats.zunes.me
127.0.0.10 cache-tiles.zunes.me
127.0.0.11 tuners.zunes.me
127.0.0.12 tiles.zunes.me
127.0.0.13 social.zunes.me
127.0.0.14 login.zunes.me
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