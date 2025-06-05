using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zune.DataProviders.MusicBrainz;

public class MusicBrainzProvider(IMediaIdMapper idMapper) : IAlbumImageProvider, IMediaIdMapper
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

                    Url alWebUrl = alRelationship.Item.Resource;
                    if (int.TryParse(alWebUrl.PathSegments.Last(), out int alid))
                        return new(alid, targetSource);

                    return null;

                case KnownMediaSources.Deezer:
                    var dzRelationship = urls.FirstOrDefault(url => url.Item.Resource?.Host.EndsWith("deezer.com") ?? false);

                    Url dzWebUrl = dzRelationship.Item.Resource;
                    if (int.TryParse(dzWebUrl.PathSegments.Last(), out int dzid))
                        return new(dzid, targetSource);

                    return null;

                case KnownMediaSources.Discogs:
                    var dcRelationship = urls.FirstOrDefault(url => url.Item.Relationships.Any(rel => rel.Type == "discogs"));

                    Url dcWebUrl = dcRelationship.Item.Resource;
                    if (int.TryParse(dcWebUrl.PathSegments.Last(), out int dcid))
                        return new(dcid, targetSource);

                    return null;

                case KnownMediaSources.Tidal:
                    var tiRelationship = urls.FirstOrDefault(url => url.Item.Resource?.Host.EndsWith("tidal.com") ?? false);

                    Url tiWebUrl = tiRelationship.Item.Resource;
                    if (int.TryParse(tiWebUrl.PathSegments.Last(), out int tiid))
                        return new(tiid, targetSource);

                    return null;
            }
        }

        return null;
    }
}
