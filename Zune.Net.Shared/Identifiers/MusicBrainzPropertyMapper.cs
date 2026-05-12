using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MetaBrainz.MusicBrainz;
using Zune.Net.Helpers;

namespace Zune.Net.Identifiers;

public partial class MusicBrainzPropertyMapper : IPropertyMapper
{
    public IReadOnlySet<PropertyMapping> AvailableMappings { get; } = GetAvailableMappings().ToHashSet();

    private static readonly Dictionary<EntityProperty, (Guid, Func<Regex>, Func<string, object>)> PropsFromUrlRels = new()
    {
        [Ep.Artist.AllMusicId] = (Guid.Parse("6b3e3c85-0002-4f34-aca6-80ace0d7e846"), RxUrlArtistAllMusic, s => s),
        [Ep.Artist.DiscogsId] = (Guid.Parse("04a5b104-a4c2-4bac-99a1-7b837c37d9e4"), RxUrlArtistDiscogs, s => int.Parse(s)),
        [Ep.Artist.LastFmId] = (Guid.Parse("08db8098-c0df-4b78-82c3-c8697b4bba7f"), RxUrlLastFm, s => s),
        [Ep.Artist.WikidataId] = (Guid.Parse("689870a4-a1e4-4912-b17f-7b2664215698"), RxUrlWikidata, s => s),
    };
    
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
                
                // ---
                // I | Decide what we need to include in our request
                
                if (desiredOutputs.Contains(Ep.Artist.AlbumIds))
                    includes |= Include.Releases;

                var propsMappedFromRelationships = PropsFromUrlRels.Keys.Where(desiredOutputs.Contains).ToHashSet();
                if (propsMappedFromRelationships.Count > 0)
                    includes |= Include.UrlRelationships;
                
                // ---
                // II | Make the request
                
                var mbArtist = await MusicBrainz._query.LookupArtistAsync(mbid, includes);

                // ---
                // III | Parse the results
                
                outputs[Ep.Artist.Name] = mbArtist.Name;
                
                if (mbArtist.Releases is not null)
                {
                    // TODO: How should it be specified that the album IDs are MusicBrainz IDs?
                    var albumIds = mbArtist.Releases.Select(r => r.Id).ToArray();
                    outputs[Ep.Artist.AlbumIds] = albumIds;
                }

                foreach (var idProp in propsMappedFromRelationships)
                {
                    var (typeId, rxUrl, parse) = PropsFromUrlRels[idProp];
                    
                    var relationship = mbArtist.Relationships?.FirstOrDefault(rel => rel.TypeId == typeId);
                    var resourceUrl = relationship?.Url?.Resource?.ToString();
                    if (resourceUrl is null)
                        return outputs;

                    var match = rxUrl().Match(resourceUrl);
                    if (!match.Success)
                        continue;

                    outputs[idProp] = parse(match.Groups[1].Value);
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
                
                ..PropsFromUrlRels.Keys,
            ]);
    }
    
    [GeneratedRegex(@"^https?:\/\/(?:www\.)?allmusic\.com\/artist\/(?:[^\/]+)?(mn[0-9]{10})")]
    private static partial Regex RxUrlArtistAllMusic();
    
    [GeneratedRegex(@"^https?:\/\/(?:www\.)?discogs\.com\/artist\/([1-9][0-9]*)")]
    private static partial Regex RxUrlArtistDiscogs();
    
    [GeneratedRegex(@"^https?:\/\/(?:www\.)?last\.fm\/(?:[a-z]{2}\/)?music\/([^\/\?\#]+)$")]
    private static partial Regex RxUrlLastFm();
    
    [GeneratedRegex(@"^https?:\/\/(?:www\.)?wikidata\.org\/(?:wiki|entity)\/(Q\d+)$")]
    private static partial Regex RxUrlWikidata();
}