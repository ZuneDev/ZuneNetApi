using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MetaBrainz.MusicBrainz;
using Zune.Net.Helpers;

namespace Zune.Net.Identifiers;

public partial class MusicBrainzIdMapper : IPropertyMapper
{
    public IReadOnlySet<PropertyMapping> AvailableMappings { get; } = GetAvailableMappings().ToHashSet();
    
    public async Task<IPropertyBag> ExecuteAsync(IPropertyBag inputs, IReadOnlyPropertySet desiredOutputs)
    {
        var inputProperty = inputs.Keys.Single();
        var mbid = (Guid)inputs[inputProperty];

        var outputProperty = desiredOutputs.Single();
        
        var outputs = new PropertyBag();

        try
        {
            if (inputProperty.EntityType is EntityType.Artist)
            {
                if (outputProperty.PropertyType is EntityPropertyType.DiscogsArtistId)
                {
                    var mbArtist = await MusicBrainz._query
                        .LookupArtistAsync(mbid, Include.UrlRelationships);

                    var discogsRel = mbArtist.Relationships?.FirstOrDefault(rel => rel.Type == "discogs");
                    var discogsLink = discogsRel?.Url?.Resource?.ToString();
                    if (discogsLink is null)
                        return outputs;

                    var rx = RxUrlDiscogs().Match(discogsLink);
                    var discogsId = Convert.ToInt32(rx.Groups[1].Value);
                    outputs[outputProperty] = discogsId;
                }
            }
        }
        catch (Exception e)
        {
        }

        return outputs;
    }

    private static IEnumerable<PropertyMapping> GetAvailableMappings()
    {
        yield return new PropertyMapping(20,
            [new EntityProperty(EntityType.Artist, EntityPropertyType.MusicBrainzArtistId)],
            [new EntityProperty(EntityType.Artist, EntityPropertyType.DiscogsArtistId)]);
    }

    [GeneratedRegex(@"/^https?:\/\/(?:www\.)?discogs\.com\/artist\/([1-9][0-9]*)/")]
    private static partial Regex RxUrlDiscogs();
}