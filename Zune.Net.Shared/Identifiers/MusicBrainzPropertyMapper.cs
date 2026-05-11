using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetaBrainz.MusicBrainz;
using Zune.Net.Helpers;

namespace Zune.Net.Identifiers;

public class MusicBrainzPropertyMapper : IPropertyMapper
{
    public IReadOnlySet<PropertyMapping> AvailableMappings { get; } = GetAvailableMappings().ToHashSet();
    
    public async Task<IPropertyBag> ExecuteAsync(IPropertyBag inputs, IReadOnlyPropertySet desiredOutputs)
    {
        var outputs = new PropertyBag();
        
        try
        {
            var inputProperty = inputs.Keys.Single(p =>
                p.PropertyType is EntityPropertyType.MusicBrainzArtistId or EntityPropertyType.MusicBrainzReleaseId);
            
            var mbid = (Guid)inputs[inputProperty];
            
            if (inputProperty.EntityType is EntityType.Artist)
            {
                var includes = Include.None;
                if (desiredOutputs.Contains(new EntityProperty(EntityType.Artist, EntityPropertyType.ArtistAlbumIds)))
                {
                    includes |= Include.Releases;
                }
                
                var mbArtist = await MusicBrainz._query.LookupArtistAsync(mbid, includes);
                outputs[new EntityProperty(EntityType.Artist, EntityPropertyType.ArtistName)] = mbArtist.Name;

                if (mbArtist.Releases is not null)
                {
                    // TODO: How should it be specified that the album IDs are MusicBrainz IDs?
                    var albumIds = mbArtist.Releases.Select(r => r.Id).ToArray();
                    outputs[new EntityProperty(EntityType.Artist, EntityPropertyType.ArtistAlbumIds)] = albumIds;
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
            [
                new EntityProperty(EntityType.Artist, EntityPropertyType.ArtistName),
                new EntityProperty(EntityType.Artist, EntityPropertyType.ArtistAlbumIds),
            ]);
    }
}