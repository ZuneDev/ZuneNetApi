using Atom.Xml;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Zune.DataProviders.Discogs;

public class DiscogsProvider(IMediaIdMapper idMapper) : IArtistBiographyProvider
{
    public async Task<Content> GetArtistBiography(MediaId id)
    {
        int? dcid = id.Source switch
        {
            KnownMediaSources.Discogs => id.AsInt32(),
            //KnownMediaSources.MusicBrainz => await GetDcidFromMbid(id.AsGuid()),
            _ => (await idMapper.MapTo(id, KnownMediaSources.Discogs)).AsInt32(),
        };

        if (dcid is null)
            return null;

        var dcArtist = await Net.Helpers.Discogs.GetDCArtistByDCID(dcid.Value);
        var dcProfile = dcArtist.Value<string>("profile");
        return Net.Helpers.Discogs.DCProfileToBiographyContent(dcProfile);
    }

    private static async Task<int?> GetDcidFromMbid(Guid mbid)
    {
        var mbArtist = await Net.Helpers.MusicBrainz.Query.LookupArtistAsync(mbid, MetaBrainz.MusicBrainz.Include.UrlRelationships);

        var dcRelationship = mbArtist.Relationships.FirstOrDefault(rel => rel.Type == "discogs");
        if (dcRelationship is null)
            return null;

        Flurl.Url dcWebUrl = dcRelationship.Url.Resource;
        if (int.TryParse(dcWebUrl.PathSegments.Last(), out int dcid))
            return dcid;

        return null;
    }
}
