using Atom.Xml;
using Flurl;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zune.DataProviders.Discogs;

public class DiscogsProvider(IMediaIdMapper idMapper) : IArtistBiographyProvider, IArtistImageProvider
{
    public async Task<Content> GetArtistBiography(MediaId id)
    {
        var mappedId = await idMapper.MapTo(id, KnownMediaSources.Discogs);
        var dcid = mappedId?.AsInt32();
        if (dcid is null)
            return null;

        var dcArtist = await Net.Helpers.Discogs.GetDCArtistByDCID(dcid.Value);
        var dcProfile = dcArtist.Value<string>("profile");

        return await Net.Helpers.Discogs.DCProfileToBiographyContent(dcProfile, idMapper);
    }

    public async IAsyncEnumerable<Url> GetArtistImages(MediaId id)
    {
        var mappedId = await idMapper.MapTo(id, KnownMediaSources.Discogs);
        var dcid = mappedId?.AsInt32();
        if (dcid is null)
            yield break;

        var dcArtist = await Net.Helpers.Discogs.GetDCArtistByDCID(dcid.Value);
        var images = dcArtist.Value<JArray>("images");

        foreach (var image in images)
            yield return image.Value<string>("uri");
    }

    public async Task<Url> GetArtistPrimaryImage(MediaId id)
    {
        var mappedId = await idMapper.MapTo(id, KnownMediaSources.Discogs);
        var dcid = mappedId?.AsInt32();
        if (dcid is null)
            return null;

        var dcArtist = await Net.Helpers.Discogs.GetDCArtistByDCID(dcid.Value);

        return dcArtist.Value<JArray>("images")?
            .FirstOrDefault(i => i.Value<string>("type") == "primary")?
            .Value<string>("uri");
    }
}
