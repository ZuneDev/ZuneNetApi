using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MetaBrainz.MusicBrainz;
using Zune.Net.Helpers;
using Zune.Net.Ontology.BaseProperties;
using Zune.Net.Ontology.Identifiers;

namespace Zune.Net.Ontology.Mappers;

public class MusicBrainzPropertyMapper : IPropertyMapper
{
    public IReadOnlySet<PropertyMapping> AvailableMappings { get; } = GetAvailableMappings().ToHashSet();

    private static readonly Dictionary<IEntityProperty, (Guid, Func<Regex>)> ArtistPropsFromUrlRels = new()
    {
        [AllMusicIdProperty.Artist]   = (MusicBrainz.UrlIdAllMusic,  MusicBrainz.RxUrlAllMusicArtist),
        [DiscogsIdProperty.Artist]    = (MusicBrainz.UrlIdDiscogs,   MusicBrainz.RxUrlDiscogsArtist),
        [LastFmIdProperty.Artist]     = (MusicBrainz.UrlIdLastFm,    MusicBrainz.RxUrlLastFm),
    };

    private static readonly Dictionary<IEntityProperty, (Guid, Func<Regex>)> ReleasePropsFromUrlRels = new()
    {
        [AllMusicIdProperty.Album]    = (MusicBrainz.UrlIdAllMusic,  MusicBrainz.RxUrlAllMusicAlbum),
        [AllMusicIdProperty.Release]  = (MusicBrainz.UrlIdAllMusic,  MusicBrainz.RxUrlAllMusicRelease),
        [DiscogsIdProperty.Master]    = (MusicBrainz.UrlIdDiscogs,   MusicBrainz.RxUrlDiscogsMaster),
        [DiscogsIdProperty.Release]   = (MusicBrainz.UrlIdDiscogs,   MusicBrainz.RxUrlDiscogsRelease),
        [LastFmIdProperty.Album]      = (MusicBrainz.UrlIdLastFm,    MusicBrainz.RxUrlLastFm),
    };
    
    public async Task<IPropertyBag> ExecuteAsync(IPropertyBag inputs, IReadOnlyPropertySet desiredOutputs)
    {
        var outputs = new PropertyBag();
        
        try
        {
            var inputProperty = inputs.Keys.OfType<MusicBrainzIdProperty>().Single();
            var mbid = inputs.Get(inputProperty);
            
            Func<Guid, IReadOnlyPropertySet, PropertyBag, Task> executeAsync = inputProperty.ProviderEntityType switch
            {
                MusicBrainzEntityType.Artist => ExecuteForArtistAsync,
                
                _ => throw new NotSupportedException($"Execution for entity type '{inputProperty.EntityType}' is not supported.")
            };
            
            await executeAsync(mbid, desiredOutputs, outputs);
        }
        catch (Exception e)
        {
        }

        return outputs;
    }

    private static async Task ExecuteForArtistAsync(Guid mbid, IReadOnlyPropertySet desiredOutputs, PropertyBag outputs)
    {
        // ---
        // I | Decide what we need to include in our request
                
        var includes = Include.None;

        var releaseMbidsProp = Ep.Artist.AlbumIds(MusicBrainzIdProperty.Release);
        if (desiredOutputs.Contains(releaseMbidsProp))
            includes |= Include.Releases;

        var releaseGroupMbidsProp = Ep.Artist.AlbumIds(MusicBrainzIdProperty.ReleaseGroup);
        if (desiredOutputs.Contains(releaseGroupMbidsProp))
            includes |= Include.ReleaseGroups;

        var propsMappedFromRelationships = AddIncludesForUrlRelationships(
            ArtistPropsFromUrlRels, desiredOutputs, ref includes);
        
        // ---
        // II | Make the request
        
        var mbArtist = await MusicBrainz._query.LookupArtistAsync(mbid, includes);

        // ---
        // III | Parse the results
        
        outputs.Set(Ep.Artist.Name, mbArtist.Name);
        
        if (mbArtist.Releases is not null)
        {
            var releaseMbids = mbArtist.Releases
                .Select(r => r.Id)
                .ToPropertyValueList(MusicBrainzIdProperty.Release);
            outputs.Set(releaseMbidsProp, releaseMbids);
        }
        
        if (mbArtist.ReleaseGroups is not null)
        {
            var releaseGroupMbids = mbArtist.ReleaseGroups
                .Select(r => r.Id)
                .ToPropertyValueList(MusicBrainzIdProperty.ReleaseGroup);
            outputs.Set(releaseGroupMbidsProp, releaseGroupMbids);
        }

        foreach (var idProp in propsMappedFromRelationships)
        {
            var (typeId, rxUrl) = ArtistPropsFromUrlRels[idProp];
            
            var relationship = mbArtist.Relationships?.FirstOrDefault(rel => rel.TypeId == typeId);
            var resourceUrl = relationship?.Url?.Resource?.ToString();
            if (resourceUrl is null)
                continue;

            var match = rxUrl().Match(resourceUrl);
            if (!match.Success)
                continue;
            
            var idStr = match.Groups[1].Value;
            
            outputs[idProp] = idStr;
        }
    }
    
    private static HashSet<IEntityProperty> AddIncludesForUrlRelationships(
        Dictionary<IEntityProperty, (Guid, Func<Regex>)> propsFromUrlRels,
        IReadOnlyPropertySet desiredOutputs, ref Include includes)
    {
        var propsMappedFromRelationships = propsFromUrlRels.Keys.ToHashSet();
        propsMappedFromRelationships.ExceptWith(desiredOutputs);
        
        if (propsMappedFromRelationships.Count > 0)
            includes |= Include.UrlRelationships;

        return propsMappedFromRelationships;
    }
    
    private static IEnumerable<PropertyMapping> GetAvailableMappings()
    {
        const int cost = 20;
        
        yield return new PropertyMapping(cost,
            [MusicBrainzIdProperty.Artist],
            [
                Ep.Artist.Name,
                Ep.Artist.AlbumIds(MusicBrainzIdProperty.Release),
                
                ..ArtistPropsFromUrlRels.Keys,
            ]);
        
        // TODO: Implement release property mappings
        yield return new PropertyMapping(cost,
            [MusicBrainzIdProperty.Release],
            [
                Ep.Album.Name,
                Ep.Album.PrimaryImageId(CoverArtArchiveIdProperty.ImageFront),
                Ep.Album.TrackIds(MusicBrainzIdProperty.Recording),
                
                ..ReleasePropsFromUrlRels.Keys,
            ]);

        // TODO: yield return new PropertyMapping(0, [MusicBrainzIdProperty.CoverArt], [Ep.Image.Url]);
        
        const int caaValidationCost = 5;
        // TODO: yield return new PropertyMapping(caaValidationCost, [MusicBrainzIdProperty.Release], [Ep.Image.Url]);
    }
}