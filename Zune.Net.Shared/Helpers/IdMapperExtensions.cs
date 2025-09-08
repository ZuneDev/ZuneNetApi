#nullable enable

using System;
using System.Linq;
using System.Threading.Tasks;

namespace Zune.Net.Helpers;

public static class IdMapperExtensions
{
    public static async Task<ArtistIdMap?> GetArtistIdsAsync<TSrcId>(this IdMapper idMapper,
        TSrcId srcId, WikidataProperty srcProp) =>
        await idMapper.BatchGetArtistIdsAsync([srcId], srcProp).FirstOrDefaultAsync();
    
    public static async Task<AlbumIdMap?> GetAlbumIdsAsync<TSrcId>(this IdMapper idMapper,
        TSrcId srcId, WikidataProperty srcProp) =>
        await idMapper.BatchGetAlbumIdsAsync([srcId], srcProp).FirstOrDefaultAsync();
    
    public static async Task<AlbumIdMap?> GetAlbumIdsByMbidAsync(this IdMapper idMapper, Guid releaseGroupMbid) =>
        await idMapper.BatchGetAlbumIdsAsync([releaseGroupMbid], WikidataProperty.MBReleaseGroupId).FirstOrDefaultAsync();
}