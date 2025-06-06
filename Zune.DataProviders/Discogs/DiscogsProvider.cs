using Atom.Xml;
using System.Threading.Tasks;

namespace Zune.DataProviders.Discogs;

public class DiscogsProvider(IMediaIdMapper idMapper) : IArtistBiographyProvider
{
    public async Task<Content> GetArtistBiography(MediaId id)
    {
        int? dcid = null;

        if (id.Source.OrdinalEquals(KnownMediaSources.Discogs))
        {
            dcid = id.AsInt32();
        }
        else
        {
            var mappedId = await idMapper.MapTo(id, KnownMediaSources.Discogs);
            dcid = mappedId?.AsInt32();
        }

        if (dcid is null)
            return null;

        var dcArtist = await Net.Helpers.Discogs.GetDCArtistByDCID(dcid.Value);
        var dcProfile = dcArtist.Value<string>("profile");
        return await Net.Helpers.Discogs.DCProfileToBiographyContent(dcProfile);
    }
}
