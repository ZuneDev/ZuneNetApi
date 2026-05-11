using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zune.Net.Helpers;

namespace Zune.Net.Identifiers;

public class DiscogsPropertyMapper : IPropertyMapper
{
    public IReadOnlySet<PropertyMapping> AvailableMappings { get; } = GetAvailableMappings().ToHashSet();
    
    public async Task<IPropertyBag> ExecuteAsync(IPropertyBag inputs, IReadOnlyPropertySet desiredOutputs)
    {
        var outputs = new PropertyBag();
        
        try
        {
            var inputProperty = inputs.Keys.Single(p =>
                p.PropertyType is EntityPropertyType.DiscogsArtistId or EntityPropertyType.DiscogsMasterId);
            
            var dcid = Convert.ToInt32(inputs[inputProperty]);
            
            if (inputProperty.EntityType is EntityType.Artist)
            {
                var dcArtist = await Discogs.GetDCArtistByDCID(dcid);
                
                var name = dcArtist.Value<string>("name");
                var bio = Discogs.DCProfileToBiographyContent(dcArtist.Value<string>("profile")).Value;
                
                outputs[new EntityProperty(EntityType.Artist, EntityPropertyType.ArtistName)] = name;
                outputs[new EntityProperty(EntityType.Artist, EntityPropertyType.ArtistBio)] = bio;
            }
        }
        catch (Exception e)
        {
        }

        return outputs;
    }

    private static IEnumerable<PropertyMapping> GetAvailableMappings()
    {
        yield return new PropertyMapping(10,
            [new EntityProperty(EntityType.Artist, EntityPropertyType.DiscogsArtistId)],
            [
                new EntityProperty(EntityType.Artist, EntityPropertyType.ArtistName),
                new EntityProperty(EntityType.Artist, EntityPropertyType.ArtistBio),
            ]);
    }
}