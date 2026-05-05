using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Query;

namespace Zune.Net.Identifiers;

public class WikidataIdMapper : IPropertyMapper
{
    private readonly SparqlQueryClient
        _wikidata = new(CreateHttpClient(), new Uri("https://query.wikidata.org/sparql"));

    private static readonly Dictionary<EntityType, Dictionary<EntityPropertyType, WikidataProperty>>
        EntityToWikidataPropMap = new()
        {
            /* Artists */
            {
                EntityType.Artist, new()
                {
                    { EntityPropertyType.MusicBrainzArtistId, WikidataProperty.MBArtistId },
                    { EntityPropertyType.AllMusicArtistId, WikidataProperty.ALArtistId },
                    { EntityPropertyType.DiscogsArtistId, WikidataProperty.DCArtistId },
                    { EntityPropertyType.DeezerArtistId, WikidataProperty.DZArtistId },
                    { EntityPropertyType.LastFmArtistId, WikidataProperty.FMId },
                    { EntityPropertyType.SpotifyArtistId, WikidataProperty.SPArtistId },
                    { EntityPropertyType.TidalArtistId, WikidataProperty.TIArtistId },
                    { EntityPropertyType.WikidataPerformerId, WikidataProperty.WKPerformerId },
                }
            },

        /* Albums */
        {
            EntityType.Album, new()
            {
                { EntityPropertyType.MusicBrainzReleaseGroupId, WikidataProperty.MBReleaseGroupId },
                { EntityPropertyType.MusicBrainzReleaseId, WikidataProperty.MBReleaseId },
                { EntityPropertyType.AllMusicAlbumId, WikidataProperty.ALAlbumId },
                { EntityPropertyType.AppleMusicAlbumId, WikidataProperty.AMAlbumId },
                { EntityPropertyType.DiscogsMasterId, WikidataProperty.DCMasterId },
                { EntityPropertyType.DeezerAlbumId, WikidataProperty.DZAlbumId },
                { EntityPropertyType.LastFmArtistId, WikidataProperty.FMId },
                { EntityPropertyType.SpotifyAlbumId, WikidataProperty.SPAlbumId },
                { EntityPropertyType.TidalAlbumId, WikidataProperty.TIAlbumId },
                { EntityPropertyType.WikidataPerformerId, WikidataProperty.WKPerformerId },
            }
        },
    };

    private static readonly Dictionary<EntityType, Dictionary<WikidataProperty, EntityPropertyType>> WikidataToEntityPropMaps = new()
    {
        {
            EntityType.Artist, new()
            {
                { WikidataProperty.MBArtistId, EntityPropertyType.MusicBrainzArtistId },
                { WikidataProperty.ALArtistId, EntityPropertyType.AllMusicArtistId },
                { WikidataProperty.DCArtistId, EntityPropertyType.DiscogsArtistId },
                { WikidataProperty.DZArtistId, EntityPropertyType.DeezerArtistId },
                { WikidataProperty.FMId, EntityPropertyType.LastFmArtistId },
                { WikidataProperty.SPArtistId, EntityPropertyType.SpotifyArtistId },
                { WikidataProperty.TIArtistId, EntityPropertyType.TidalArtistId },
                { WikidataProperty.WKPerformerId, EntityPropertyType.WikidataPerformerId },
            }
        },

        {
            EntityType.Album, new()
            {
                { WikidataProperty.MBReleaseGroupId, EntityPropertyType.MusicBrainzReleaseGroupId },
                { WikidataProperty.MBReleaseId, EntityPropertyType.MusicBrainzReleaseId },
                { WikidataProperty.ALAlbumId, EntityPropertyType.AllMusicAlbumId },
                { WikidataProperty.AMAlbumId, EntityPropertyType.AppleMusicAlbumId },
                { WikidataProperty.DCMasterId, EntityPropertyType.DiscogsMasterId },
                { WikidataProperty.DZAlbumId, EntityPropertyType.DeezerAlbumId },
                { WikidataProperty.FMId, EntityPropertyType.LastFmArtistId },
                { WikidataProperty.SPAlbumId, EntityPropertyType.SpotifyAlbumId },
                { WikidataProperty.TIAlbumId, EntityPropertyType.TidalAlbumId },
                { WikidataProperty.WKPerformerId, EntityPropertyType.WikidataPerformerId },
            }
        },
    };

    private static Dictionary<EntityType, HashSet<EntityProperty>> GetAvailableOutputEntityProperties()
    {
        return EntityToWikidataPropMap.ToDictionary(
            em => em.Key,
            em =>
                em.Value.Select(pm => new EntityProperty(em.Key, pm.Key)).ToHashSet());;
    }

    public IReadOnlySet<PropertyMapping> AvailableMappings { get; } = GetAvailableMappings();

    public ImmutableHashSet<EntityProperty> WouldMap(ISet<EntityProperty> inputs)
    {
        var inputsByEntityType = inputs
            .GroupBy(i => i.EntityType);

        var outputsByEntityType = GetAvailableOutputEntityProperties();

        HashSet<EntityProperty> outputs = [];

        foreach (var entityInputs in inputsByEntityType)
        {
            var entityType = entityInputs.Key;

            var availableInputs = entityInputs.ToHashSet();
            var availableOutputs = outputsByEntityType[entityType];
            
            availableOutputs.IntersectWith(availableInputs);
            
            outputs.UnionWith(availableOutputs);
        }

        return outputs.ToImmutableHashSet();
    }
    
    public async Task<IPropertyBag> ExecuteAsync(IPropertyBag inputs)
    {
        var inputsByEntityType = ((IDictionary<EntityProperty, object>)inputs)
            .GroupBy(i => i.Key.EntityType);

        var outputsByEntityType = GetAvailableOutputEntityProperties();

        Dictionary<EntityProperty, object> outputs = [];

        foreach (var entityInputs in inputsByEntityType)
        {
            var entityType = entityInputs.Key;
            var wikidataToEntityPropMap = WikidataToEntityPropMaps[entityType];
            var entityToWikidataPropMap = EntityToWikidataPropMap[entityType];

            var availableInputs = entityInputs.Select(i => i.Key).ToHashSet();
            var availableOutputs = outputsByEntityType[entityType];
            
            availableOutputs.ExceptWith(availableInputs);

            var wikidataOutputs = availableOutputs
                .Select(a => entityToWikidataPropMap[a.PropertyType])
                .ToHashSet();
            var entitySourceIds = entityInputs.ToDictionary(
                s => entityToWikidataPropMap[s.Key.PropertyType],
                s => s.Value?.ToString());
            
            await ExecuteMappingQueryAsync(entitySourceIds, wikidataOutputs, EntityOutputSetter);
            
            continue;

            void EntityOutputSetter(WikidataProperty prop, string id)
            {
                outputs[new EntityProperty(entityType, wikidataToEntityPropMap[prop])] = id;
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

    private static IReadOnlySet<PropertyMapping> GetAvailableMappings()
    {
        const int cost = 10;

        HashSet<PropertyMapping> mappings = new(EntityToWikidataPropMap.Count * 10);
        
        foreach (var (entityType, props) in EntityToWikidataPropMap)
        {
            foreach (var propType in props.Keys)
            {
                var input = new EntityProperty(entityType, propType);
                var inputs = new PropertySet([input]);
                
                var outputs = props.Keys
                    .Where(p => p != propType)
                    .Select(p => new EntityProperty(entityType, p))
                    .ToPropertySet();
                
                mappings.Add(new PropertyMapping(cost, inputs, outputs));
            }
        }

        return mappings;
    }
}