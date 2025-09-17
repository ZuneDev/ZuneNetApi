#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using VDS.RDF;
using VDS.RDF.Query;

namespace Zune.Net.Helpers;

public class IdMapper
{
    private readonly SparqlQueryClient _wikidata = new(CreateHttpClient(), new Uri("https://query.wikidata.org/sparql"));

    private readonly Dictionary<string, WikidataPropertyRequest<ArtistIdMap>> _artistIdRequests = new()
    {
        ["mbid"] = new(WikidataProperty.MBArtistId, (m, v) => m.MusicBrainz = new Guid(v)),
        ["alid"] = new(WikidataProperty.ALArtistId, (m, v) => m.AllMusic = v),
        ["dcid"] = new(WikidataProperty.DCArtistId, (m, v) => m.Discogs = v),
        ["dzid"] = new(WikidataProperty.DZArtistId, (m, v) => m.Deezer = v),
        ["fmid"] = new(WikidataProperty.FMId, (m, v) => m.LastFM = v),
        ["spid"] = new(WikidataProperty.SPArtistId, (m, v) => m.Spotify = v),
        ["tiid"] = new(WikidataProperty.TIArtistId, (m, v) => m.Tidal = v),
    };

    private readonly Dictionary<string, WikidataPropertyRequest<AlbumIdMap>> _albumIdRequests = new()
    {
        ["mbid"] = new(WikidataProperty.MBReleaseGroupId, (m, v) => m.MusicBrainzReleaseGroup = new Guid(v)),
        ["alid"] = new(WikidataProperty.ALAlbumId, (m, v) => m.AllMusic = v),
        ["amid"] = new(WikidataProperty.AMAlbumId, (m, v) => m.AppleMusic = v),
        ["dcid"] = new(WikidataProperty.DCMasterId, (m, v) => m.Discogs = v),
        ["dzid"] = new(WikidataProperty.DZAlbumId, (m, v) => m.Deezer = v),
        ["fmid"] = new(WikidataProperty.FMId, (m, v) => m.LastFM = v),
        ["spid"] = new(WikidataProperty.SPAlbumId, (m, v) => m.Spotify = v),
        ["tiid"] = new(WikidataProperty.TIAlbumId, (m, v) => m.Tidal = v),
    };

    public IAsyncEnumerable<ArtistIdMap> BatchGetArtistIdsAsync<TSrcId>(IEnumerable<TSrcId> srcIds, WikidataProperty srcProp) =>
        ExecuteMappingQueryAsync(srcIds, srcProp, _artistIdRequests);

    public IAsyncEnumerable<AlbumIdMap> BatchGetAlbumIdsAsync<TSrcId>(IEnumerable<TSrcId> srcIds, WikidataProperty srcProp) =>
        ExecuteMappingQueryAsync(srcIds, srcProp, _albumIdRequests);

    private async IAsyncEnumerable<TMap> ExecuteMappingQueryAsync<TMap, TSrcId>(IEnumerable<TSrcId> srcIds, WikidataProperty srcIdProp,
        Dictionary<string, WikidataPropertyRequest<TMap>> requests) where TMap : new()
    {
        var srcIdValues = string.Join(' ', srcIds.Select(id => $"\"{id}\""));
        
        var selectRequestedIds = string.Join(' ', requests.Keys.Select(k => $"?{k}"));

        var optionalQueryRequestedIds = requests
            .Select(kvp => $"OPTIONAL {{ ?item wdt:P{kvp.Value.Property:D} ?{kvp.Key} }}");
        var optionalQueryRequestedIdsBlock = string.Join('\n', optionalQueryRequestedIds);
        
        var queryString = $$"""
            PREFIX wikibase: <http://wikiba.se/ontology#>
            PREFIX wd: <http://www.wikidata.org/entity/> 
            PREFIX wdt: <http://www.wikidata.org/prop/direct/>
            PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
            PREFIX p: <http://www.wikidata.org/prop/>
            PREFIX v: <http://www.wikidata.org/prop/statement/>

            SELECT {{selectRequestedIds}} {
                VALUES ?srcId {
                    {{srcIdValues}}
                }
            
                ?item wdt:P{{srcIdProp:D}} ?srcId.
                
                {{optionalQueryRequestedIdsBlock}}
            }
            """;
        
        var resultSet = await _wikidata.QueryWithResultSetAsync(queryString);
        
        foreach (var row in resultSet)
        {
            TMap idMap = new();
            
            foreach (var (key, value) in row)
            {
                if (value is not LiteralNode idValue)
                    continue;
                
                if (!requests.TryGetValue(key, out var request))
                    continue;
                
                request.IdSetter(idMap, idValue.Value);
            }
            
            yield return idMap;
        }
    }

    private static HttpClient CreateHttpClient()
    {
        HttpClient httpClient = new();
        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Zune", "4.8"));
        return httpClient;
    }

    private record WikidataPropertyRequest<TMap>(WikidataProperty Property, Action<TMap, string> IdSetter);
}
