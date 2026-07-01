using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zune.Net.Ontology.Identifiers;

namespace Zune.Net.Ontology.Mappers;

public class CoverArtArchivePropertyMapper : IPropertyMapper
{
    public IReadOnlySet<PropertyMapping> AvailableMappings { get; } = GetAvailableMappings().ToHashSet();
    
    public async Task<IPropertyBag> ExecuteAsync(IPropertyBag inputs, IReadOnlyPropertySet desiredOutputs)
    {
        throw new System.NotImplementedException();
    }
    
    private static IEnumerable<PropertyMapping> GetAvailableMappings()
    {
        const int cost = 5;
        
        yield return new PropertyMapping(cost,
            [MusicBrainzIdProperty.Release],
            [Ep.Album.PrimaryImageId(CoverArtArchiveIdProperty.ImageFront)]);
        
        // yield return new PropertyMapping(cost,
        //     [MusicBrainzIdProperty.Release, CoverArtArchiveIdProperty.ImageFront],
        //     [Ep.ImageIn]);
        //
        // // TODO: Implement release property mappings
        // yield return new PropertyMapping(cost,
        //     [MusicBrainzIdProperty.Release],
        //     [
        //         Ep.Album.Name,
        //         Ep.Album.ImageUrl,
        //         Ep.Album.ImageId(MusicBrainzIdProperty.CoverArt),
        //         Ep.Album.TrackIds(MusicBrainzIdProperty.Recording),
        //         
        //         ..ReleasePropsFromUrlRels.Keys,
        //     ]);
        //
        // yield return new PropertyMapping(0, [MusicBrainzIdProperty.CoverArt], [Ep.Image.Url]);
        //
        // const int caaValidationCost = 5;
        // yield return new PropertyMapping(caaValidationCost, [MusicBrainzIdProperty.Release], [Ep.Image.Url]);
    }
}