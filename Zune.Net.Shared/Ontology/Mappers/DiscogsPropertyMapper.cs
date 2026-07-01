using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zune.Net.Helpers;
using Zune.Net.Ontology.Identifiers;

namespace Zune.Net.Ontology.Mappers;

public class DiscogsPropertyMapper : IPropertyMapper
{
    public IReadOnlySet<PropertyMapping> AvailableMappings { get; } = GetAvailableMappings().ToHashSet();
    
    public async Task<IPropertyBag> ExecuteAsync(IPropertyBag inputs, IReadOnlyPropertySet desiredOutputs)
    {
        var outputs = new PropertyBag();
        
        try
        {
            var inputProperty = inputs.Keys.OfType<DiscogsIdProperty>().Single();
            var dcid = inputs.Get(inputProperty);
            
            if (inputProperty.EntityType is EntityType.Artist)
            {
                var dcArtist = await Discogs.GetDCArtistByDCID(dcid);
                
                var name = dcArtist.Value<string>("name");
                outputs.Set(Ep.Artist.Name, name);

                if (desiredOutputs.Contains(Ep.Artist.Bio))
                {
                    var profile = dcArtist.Value<string>("profile");
                    var bio = Discogs.DCProfileToBiographyContent(profile).Value;
                    outputs.Set(Ep.Artist.Bio, bio);
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
        yield return new PropertyMapping(10,
            [DiscogsIdProperty.Artist],
            [
                Ep.Artist.Name,
                Ep.Artist.Bio,
                Ep.Artist.PrimaryImageId(DiscogsImageIdProperty.Image),
            ]);
    }
}