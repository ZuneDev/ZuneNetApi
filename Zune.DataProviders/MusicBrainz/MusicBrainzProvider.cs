using Flurl;
using Flurl.Http;
using MetaBrainz.MusicBrainz;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zune.Xml.Catalog;

namespace Zune.DataProviders.MusicBrainz;

public class MusicBrainzProvider(IMediaIdMapper idMapper) : IArtistProvider, IAlbumProvider, IAlbumImageProvider, ITrackProvider, IMediaIdMapper
{
    public async IAsyncEnumerable<Url> GetAlbumImages(MediaId id)
    {
        if (!id.Source.OrdinalEquals(KnownMediaSources.MusicBrainz))
            id = await idMapper.MapTo(id, KnownMediaSources.MusicBrainz);

        var coverArtIndex = await "https://coverartarchive.org/release"
            .AppendPathSegment(id.Id)
            .GetJsonAsync<List<JObject>>();

        foreach (var coverArt in coverArtIndex)
            if (coverArt.Value<bool>("front"))
                yield return coverArt.Value<string>("image");
    }

    public async Task<MediaId> MapTo(MediaId id, string targetSource)
    {
        // Short-circuit if we already have the correct ID
        if (id.Source.OrdinalEquals(targetSource))
            return id;


        if (id.Source.OrdinalEquals(KnownMediaSources.MusicBrainz))
        {
            var mbid = id.AsGuid();

            // Use MusicBrainz relationships to map IDs
            var urlSearch = await Net.Helpers.MusicBrainz.Query.FindUrlsAsync($"targetid:{mbid}");
            var urls = urlSearch.AsStream();

            switch (targetSource)
            {
                case KnownMediaSources.AllMusic:
                    var alRelationship = urls.FirstOrDefault(url => url.Item.Relationships.Any(rel => rel.Type == "allmusic"));

                    var alWebUrl = alRelationship?.Item?.Resource;
                    if (alWebUrl is null)
                        return null;

                    return new(alWebUrl.Segments[^1], targetSource);

                case KnownMediaSources.Deezer:
                    var dzRelationship = urls.FirstOrDefault(url => url.Item.Resource?.Host.EndsWith("deezer.com") ?? false);

                    var dzWebUrl = dzRelationship?.Item?.Resource;
                    if (dzWebUrl is null)
                        return null;

                    return new(dzWebUrl.Segments[^1], targetSource);

                case KnownMediaSources.Discogs:
                    var dcRelationship = urls.FirstOrDefault(url => url.Item.Relationships.Any(rel => rel.Type == "discogs"));

                    var dcWebUrl = dcRelationship?.Item?.Resource;
                    if (int.TryParse(dcWebUrl.Segments[^1], out int dcid))
                        return new(dcid, targetSource);

                    return null;

                case KnownMediaSources.Tidal:
                    var tiRelationship = urls.FirstOrDefault(url => url.Item.Resource?.Host.EndsWith("tidal.com") ?? false);

                    var tiWebUrl = tiRelationship?.Item?.Resource;
                    if (tiWebUrl is null)
                        return null;

                    return new(tiWebUrl.Segments[^1], targetSource);
            }
        }

        if (targetSource.OrdinalEquals(KnownMediaSources.MusicBrainz))
        {
            switch (id.Source)
            {
                case KnownMediaSources.AllMusic:
                    return await TryGetMbidByUrlAsync(id.Type, searchType => searchType switch
                    {
                        MediaType.Artist => $"https://www.allmusic.com/artist/{id.Id}",
                        MediaType.Album => $"https://www.allmusic.com/album/{id.Id}",
                        MediaType.Track => $"https://www.allmusic.com/song/{id.Id}",
                        _ => throw new ArgumentOutOfRangeException(nameof(searchType))
                    });

                case KnownMediaSources.Deezer:
                    return await TryGetMbidByUrlAsync(id.Type, searchType => searchType switch
                    {
                        MediaType.Artist => $"https://www.deezer.com/artist/{id.Id}",
                        MediaType.Album => $"https://www.deezer.com/album/{id.Id}",
                        _ => throw new ArgumentOutOfRangeException(nameof(searchType))
                    });

                case KnownMediaSources.Discogs:
                    return await TryGetMbidByUrlAsync(id.Type, searchType => searchType switch
                    {
                        MediaType.Artist => $"https://www.discogs.com/artist/{id.Id}",
                        MediaType.Album => $"https://www.discogs.com/release/{id.Id}",
                        _ => throw new ArgumentOutOfRangeException(nameof(searchType))
                    });

                case KnownMediaSources.ISRC:
                    var isrcEntity = await Net.Helpers.MusicBrainz.Query.LookupIsrcAsync(id.Id);
                    if (isrcEntity is not null)
                        return new(isrcEntity.Recordings[0].Id, KnownMediaSources.MusicBrainz, MediaType.Track);
                    break;

                case KnownMediaSources.Tidal:
                    return await TryGetMbidByUrlAsync(id.Type, searchType => searchType switch
                    {
                        MediaType.Artist => $"https://tidal.com/artist/{id.Id}",
                        MediaType.Album => $"https://tidal.com/album/{id.Id}",
                        MediaType.Track => $"https://tidal.com/track/{id.Id}",
                        _ => throw new ArgumentOutOfRangeException(nameof(searchType))
                    });
            }
        }

        return null;
    }

    private static async Task<MediaId> TryGetMbidByUrlAsync(MediaType type, Func<MediaType, string> formatUrl)
    {
        if (type.HasFlag(MediaType.Artist))
        {
            try
            {
                var url = formatUrl(MediaType.Artist);
                var mbUrl = await Net.Helpers.MusicBrainz.Query.LookupUrlAsync(new Uri(url), inc: Include.ArtistRelationships);

                var mbid = mbUrl?.Relationships[0].Artist.Id;
                if (mbid.HasValue)
                    return new(mbid.Value, KnownMediaSources.MusicBrainz, MediaType.Artist);
            }
            catch { }
        }

        if (type.HasFlag(MediaType.Album))
        {
            try
            {
                var url = formatUrl(MediaType.Album);
                var mbUrl = await Net.Helpers.MusicBrainz.Query.LookupUrlAsync(new Uri(url), inc: Include.ReleaseRelationships);

                var mbid = mbUrl?.Relationships[0].Release.Id;
                if (mbid.HasValue)
                    return new(mbid.Value, KnownMediaSources.MusicBrainz, MediaType.Album);
            }
            catch { }
        }

        return null;
    }

    public async Task<Artist> GetArtist(MediaId id)
    {
        id = await idMapper.MapTo(id, KnownMediaSources.MusicBrainz);
        if (id is null)
            return null;

        return Net.Helpers.MusicBrainz.GetArtistByMBID(id.AsGuid());
    }

    public async Task<Album> GetAlbum(MediaId id)
    {
        id = await idMapper.MapTo(id, KnownMediaSources.MusicBrainz);
        if (id is null)
            return null;

        return Net.Helpers.MusicBrainz.GetAlbumByMBID(id.AsGuid());
    }

    public async Task<Track> GetTrack(MediaId id)
    {
        id = await idMapper.MapTo(id, KnownMediaSources.MusicBrainz);
        if (id is null)
            return null;

        return Net.Helpers.MusicBrainz.GetTrackByMBID(id.AsGuid());
    }
}
