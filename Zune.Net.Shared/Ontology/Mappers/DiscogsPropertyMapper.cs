using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zune.Net.Helpers;

namespace Zune.Net.Ontology.Mappers;

public class DiscogsPropertyMapper : IPropertyMapper
{
    public IReadOnlySet<PropertyMapping> AvailableMappings { get; } = GetAvailableMappings().ToHashSet();
    
    public async Task<IPropertyBag> ExecuteAsync(IPropertyBag inputs, IReadOnlyPropertySet desiredOutputs)
    {
        var outputs = new PropertyBag();
        
        try
        {
            var inputProperty = inputs.Keys.Single(p =>
                p.Fact is EntityFact.DiscogsArtistId or EntityFact.DiscogsMasterId);
            
            var dcid = Convert.ToInt32(inputs[inputProperty]);
            
            if (inputProperty.EntityType is EntityType.Artist)
            {
                var dcArtist = await Discogs.GetDCArtistByDCID(dcid);
                
                var name = dcArtist.Value<string>("name");
                outputs[Ep.Artist.Name] = name;

                if (desiredOutputs.Contains(Ep.Artist.Bio))
                {
                    var bio = Discogs.DCProfileToBiographyContent(dcArtist.Value<string>("profile")).Value;
                    outputs[Ep.Artist.Bio] = bio;
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
            [Ep.Artist.DiscogsId],
            [
                Ep.Artist.Name,
                Ep.Artist.Bio,
                Ep.Artist.ImageUrls,
                Ep.Artist.PrimaryImageUrl,
            ]);
    }
}