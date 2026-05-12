#nullable enable

using System;
using System.Linq;
using System.Threading.Tasks;

namespace Zune.Net.Identifiers;

public static class BatchIdMapperExtensions
{
    public static async Task<ArtistIdMap?> GetArtistIdsAsync<TSrcId>(this BatchIdMapper batchIdMapper,
        TSrcId srcId, WikidataProperty srcProp) =>
        await batchIdMapper.BatchGetArtistIdsAsync([srcId], srcProp).FirstOrDefaultAsync();
    
    public static async Task<AlbumIdMap?> GetAlbumIdsAsync<TSrcId>(this BatchIdMapper batchIdMapper,
        TSrcId srcId, WikidataProperty srcProp) =>
        await batchIdMapper.BatchGetAlbumIdsAsync([srcId], srcProp).FirstOrDefaultAsync();
    
    public static async Task<ArtistIdMap?> GetArtistIdsByMbidAsync(this BatchIdMapper batchIdMapper, Guid artistMbid) =>
        await batchIdMapper.BatchGetArtistIdsAsync([artistMbid], WikidataProperty.MBArtistId).FirstOrDefaultAsync();
    
    public static async Task<AlbumIdMap?> GetAlbumIdsByMbidAsync(this BatchIdMapper batchIdMapper, Guid releaseGroupMbid) =>
        await batchIdMapper.BatchGetAlbumIdsAsync([releaseGroupMbid], WikidataProperty.MBReleaseGroupId).FirstOrDefaultAsync();
}