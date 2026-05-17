using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Query;
using Zune.Net.Ontology.Identifiers;

namespace Zune.Net.Ontology.Mappers;

public class WikidataIdMapper : IPropertyMapper
{
    private readonly SparqlQueryClient
        _wikidata = new(CreateHttpClient(), new Uri("https://query.wikidata.org/sparql"));

    private static readonly Dictionary<IEntityProperty, WikidataProperty> EntityToWikidataPropMap = new()
    {
        /* Artists */
        { AllMusicIdProperty.Artist,           WikidataProperty.ALArtistId },
        { DiscogsIdProperty.Artist,            WikidataProperty.DCArtistId },
        { DeezerIdProperty.Artist,             WikidataProperty.DZArtistId },
        { LastFmIdProperty.Artist,             WikidataProperty.FMId },
        { MusicBrainzIdProperty.Artist,        WikidataProperty.MBArtistId },
        { SpotifyIdProperty.Artist,            WikidataProperty.SPArtistId },
        { TidalIdProperty.Artist,              WikidataProperty.TIArtistId },
        
        /* Albums */
        { MusicBrainzIdProperty.ReleaseGroup,  WikidataProperty.MBReleaseGroupId },
        { MusicBrainzIdProperty.Release,       WikidataProperty.MBReleaseId },
        { AllMusicIdProperty.Album,            WikidataProperty.ALAlbumId },
        { AppleMusicIdProperty.Album,          WikidataProperty.AMAlbumId },
        { DiscogsIdProperty.Master,            WikidataProperty.DCMasterId },
        { DeezerIdProperty.Album,              WikidataProperty.DZAlbumId },
        { LastFmIdProperty.Album,              WikidataProperty.FMId },
        { SpotifyIdProperty.Album,             WikidataProperty.SPAlbumId },
        { TidalIdProperty.Album,               WikidataProperty.TIAlbumId },
    };

    private static readonly Dictionary<EntityType, Dictionary<WikidataProperty, IEntityProperty>> WikidataToEntityPropMaps = new()
    {
        {
            EntityType.Artist, new()
            {
                { WikidataProperty.MBArtistId,        MusicBrainzIdProperty.Artist },
                { WikidataProperty.ALArtistId,        AllMusicIdProperty.Artist },
                { WikidataProperty.DCArtistId,        DiscogsIdProperty.Artist },
                { WikidataProperty.DZArtistId,        DeezerIdProperty.Artist },
                { WikidataProperty.FMId,              LastFmIdProperty.Artist },
                { WikidataProperty.SPArtistId,        SpotifyIdProperty.Artist },
                { WikidataProperty.TIArtistId,        TidalIdProperty.Artist },
            }
        },

        {
            EntityType.Album, new()
            {
                { WikidataProperty.MBReleaseGroupId,  MusicBrainzIdProperty.ReleaseGroup },
                { WikidataProperty.MBReleaseId,       MusicBrainzIdProperty.Release },
                { WikidataProperty.ALAlbumId,         AllMusicIdProperty.Album },
                { WikidataProperty.AMAlbumId,         AppleMusicIdProperty.Album },
                { WikidataProperty.DCMasterId,        DiscogsIdProperty.Master },
                { WikidataProperty.DZAlbumId,         DeezerIdProperty.Album },
                { WikidataProperty.FMId,              LastFmIdProperty.Album },
                { WikidataProperty.SPAlbumId,         SpotifyIdProperty.Album },
                { WikidataProperty.TIAlbumId,         TidalIdProperty.Album },
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
        var inputsByEntityType = inputs
            .GroupBy(i => i.Key.EntityType);

        PropertyBag outputs = [];

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
                outputs[entityProp] = id;
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