using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Query;

namespace Zune.Net.Ontology;

public class WikidataIdMapper : IPropertyMapper
{
    private readonly SparqlQueryClient
        _wikidata = new(CreateHttpClient(), new Uri("https://query.wikidata.org/sparql"));

    private static readonly Dictionary<IEntityProperty, WikidataProperty> EntityToWikidataPropMap = new()
    {
        /* Artists */
        { Ep.Artist.AllMusicId, WikidataProperty.ALArtistId },
        { Ep.Artist.DiscogsId, WikidataProperty.DCArtistId },
        { Ep.Artist.DeezerId, WikidataProperty.DZArtistId },
        { Ep.Artist.LastFmId, WikidataProperty.FMId },
        { Ep.Artist.MusicBrainzId, WikidataProperty.MBArtistId },
        { Ep.Artist.SpotifyId, WikidataProperty.SPArtistId },
        { Ep.Artist.TidalId, WikidataProperty.TIArtistId },
        { Ep.Artist.WikidataPerformerId, WikidataProperty.WKPerformerId },
        
        /* Albums */
        { Ep.Album.MusicBrainzReleaseGroupId, WikidataProperty.MBReleaseGroupId },
        { Ep.Album.MusicBrainzReleaseId, WikidataProperty.MBReleaseId },
        { Ep.Album.AllMusicId, WikidataProperty.ALAlbumId },
        { Ep.Album.AppleMusicId, WikidataProperty.AMAlbumId },
        { Ep.Album.DiscogsMasterId, WikidataProperty.DCMasterId },
        { Ep.Album.DeezerId, WikidataProperty.DZAlbumId },
        { Ep.Album.LastFmId, WikidataProperty.FMId },
        { Ep.Album.SpotifyId, WikidataProperty.SPAlbumId },
        { Ep.Album.TidalId, WikidataProperty.TIAlbumId },
    };

    private static readonly Dictionary<EntityType, Dictionary<WikidataProperty, IEntityProperty>> WikidataToEntityPropMaps = new()
    {
        {
            EntityType.Artist, new()
            {
                { WikidataProperty.MBArtistId, Ep.Artist.MusicBrainzId },
                { WikidataProperty.ALArtistId, Ep.Artist.AllMusicId },
                { WikidataProperty.DCArtistId, Ep.Artist.DiscogsId },
                { WikidataProperty.DZArtistId, Ep.Artist.DeezerId },
                { WikidataProperty.FMId, Ep.Artist.LastFmId },
                { WikidataProperty.SPArtistId, Ep.Artist.SpotifyId },
                { WikidataProperty.TIArtistId, Ep.Artist.TidalId },
                { WikidataProperty.WKPerformerId, Ep.Artist.WikidataPerformerId },
            }
        },

        {
            EntityType.Album, new()
            {
                { WikidataProperty.MBReleaseGroupId, Ep.Album.MusicBrainzReleaseGroupId },
                { WikidataProperty.MBReleaseId, Ep.Album.MusicBrainzReleaseId },
                { WikidataProperty.ALAlbumId, Ep.Album.AllMusicId },
                { WikidataProperty.AMAlbumId, Ep.Album.AppleMusicId },
                { WikidataProperty.DCMasterId, Ep.Album.DiscogsMasterId },
                { WikidataProperty.DZAlbumId, Ep.Album.DeezerId },
                { WikidataProperty.FMId, Ep.Album.LastFmId },
                { WikidataProperty.SPAlbumId, Ep.Album.SpotifyId },
                { WikidataProperty.TIAlbumId, Ep.Album.TidalId },
                // { WikidataProperty.WKPerformerId, Ep.Album.WikidataPerformerId },
            }
        },
    };

    private static HashSet<IEntityProperty> GetAvailableOutputEntityProperties(EntityType entityType)
    {
        return EntityToWikidataPropMap
            .Keys
            .Where(p => p.EntityType == entityType)
            .ToHashSet();
    }

    public IReadOnlySet<PropertyMapping> AvailableMappings { get; } = GetAvailableMappings();
    
    public async Task<IPropertyBag> ExecuteAsync(IPropertyBag inputs, IReadOnlyPropertySet desiredOutputs)
    {
        var inputsByEntityType = ((IDictionary<IEntityProperty, object>)inputs)
            .GroupBy(i => i.Key.EntityType);

        Dictionary<IEntityProperty, object> outputs = [];

        foreach (var entityInputs in inputsByEntityType)
        {
            var entityType = entityInputs.Key;
            var wikidataToEntityPropMap = WikidataToEntityPropMaps[entityType];

            var availableInputs = entityInputs.Select(i => i.Key).ToHashSet();
            var availableOutputs = GetAvailableOutputEntityProperties(entityType);
            
            availableOutputs.ExceptWith(availableInputs);

            var wikidataOutputs = availableOutputs
                .Select(a => EntityToWikidataPropMap[a])
                .ToHashSet();
            var entitySourceIds = entityInputs.ToDictionary(
                s => EntityToWikidataPropMap[s.Key],
                s => s.Value?.ToString());
            
            await ExecuteMappingQueryAsync(entitySourceIds, wikidataOutputs, EntityOutputSetter);
            
            continue;

            void EntityOutputSetter(WikidataProperty prop, string id)
            {
                var entityProp = wikidataToEntityPropMap[prop];

                var entityPropType = entityProp.GetType();
                if (entityPropType.GetGenericTypeDefinition() == typeof(TypedEntityProperty<>))
                {
                    var valueType = entityPropType.GenericTypeArguments[0];
                    var converter = TypeDescriptor.GetConverter(valueType);
                    var result = converter.ConvertFromInvariantString(id);
                    outputs[wikidataToEntityPropMap[prop]] = result;
                }
                else
                {
                    outputs[wikidataToEntityPropMap[prop]] = id;
                }
            }
        }

        return outputs.ToPropertyBag();
    }
    
    private async Task ExecuteMappingQueryAsync(IDictionary<WikidataProperty, string> sourceIds,
        ICollection<WikidataProperty> targetIds, Action<WikidataProperty, string> setter)
    {
        var selectRequestedIds = string.Join(' ', targetIds.Select(k => $"?{k}"));
        
        var sourceIdValues = sourceIds
            .Select(kvp => $"VALUES ?{kvp.Key} {{ \"{kvp.Value}\" }}");
        var sourceIdValuesBlock = string.Join('\n', sourceIdValues);

        var constraints = sourceIds.Select(kvp => $"?item wdt:P{kvp.Key:D} ?{kvp.Key}.");
        var contraintsBlock = string.Join('\n', constraints);

        var optionalQueryRequestedIds = targetIds
            .Select(prop => $"OPTIONAL {{ ?item wdt:P{prop:D} ?{prop} }}");
        var optionalQueryRequestedIdsBlock = string.Join('\n', optionalQueryRequestedIds);
        
        var queryString = $$"""
            PREFIX wikibase: <http://wikiba.se/ontology#>
            PREFIX wd: <http://www.wikidata.org/entity/> 
            PREFIX wdt: <http://www.wikidata.org/prop/direct/>
            PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
            PREFIX p: <http://www.wikidata.org/prop/>
            PREFIX v: <http://www.wikidata.org/prop/statement/>

            SELECT {{selectRequestedIds}} {
                {{sourceIdValuesBlock}}

                {{contraintsBlock}}
                
                {{optionalQueryRequestedIdsBlock}}
            }
            """;
        
        var resultSet = await _wikidata.QueryWithResultSetAsync(queryString);
        
        foreach (var row in resultSet)
        {
            foreach (var (key, value) in row)
            {
                if (value is not LiteralNode idValue || !Enum.TryParse<WikidataProperty>(key, out var prop))
                    continue;
                
                setter(prop, idValue.Value);
            }
        }
    }
    
    private static HttpClient CreateHttpClient()
    {
        HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Zune", "4.8"));
        return httpClient;
    }

    private static HashSet<PropertyMapping> GetAvailableMappings()
    {
        const int cost = 10;

        HashSet<PropertyMapping> mappings = new(EntityToWikidataPropMap.Count * 10);
        
        foreach (var (inputEntityProp, wkProp) in EntityToWikidataPropMap)
        {
            IPropertySet inputs = [inputEntityProp];
                
            var outputs = EntityToWikidataPropMap.Keys
                .Where(p => p.EntityType == inputEntityProp.EntityType)
                .Where(p => p != inputEntityProp)
                .ToPropertySet();
                
            mappings.Add(new PropertyMapping(cost, inputs, outputs));
        }

        return mappings;
    }
}