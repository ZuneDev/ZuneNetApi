using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Zune.Net.Identifiers;

public interface IPropertyMapper
{
    IReadOnlySet<PropertyMapping> AvailableMappings { get; }
    
    /// <summary>
    /// Maps the provided inputs to their outputs.
    /// </summary>
    Task<IPropertyBag> ExecuteAsync(IPropertyBag inputs, IReadOnlyPropertySet desiredOutputs);
}

[DebuggerDisplay("{Cost}: ({Inputs}) -> ({Outputs})")]
public record PropertyMapping(int Cost, IReadOnlyPropertySet Inputs, IReadOnlyPropertySet Outputs);

public record EntityProperty(EntityType EntityType, EntityPropertyType PropertyType)
{
    public override string ToString() => $"{EntityType}.{PropertyType}";
}

public enum EntityType
{
    Unknown,
    Artist,
    Album,
    Track,
    Genre,
    Feature,
}

public enum EntityPropertyType
{
    /* Artists */
    AllMusicArtistId,
    DiscogsArtistId,
    DeezerArtistId,
    LastFmArtistId,
    MusicBrainzArtistId,
    SpotifyArtistId,
    TidalArtistId,
    ArtistId,
    
    ArtistName,
    ArtistAlbumIds,
    ArtistAlbums,
    ArtistInfluenceIds,
    ArtistInfluences,
    ArtistInfluencerIds,
    ArtistInfluencers,
    ArtistBio,
    
    /* Albums */
    AllMusicAlbumId,
    AppleMusicAlbumId,
    DiscogsMasterId,
    DeezerAlbumId,
    LastFmAlbumId,
    MusicBrainzReleaseGroupId,
    MusicBrainzReleaseId,
    SpotifyAlbumId,
    TidalAlbumId,
    WikidataPerformerId,
    AlbumId,
    
    AlbumName,
    AlbumTrackIds,
    AlbumTracks,
    AlbumSimilarToIds,
    AlbumSimilarTo,
    AlbumArtistId,
    AlbumArtist,
}

public static class EntityPropertyTypes
{
    private static readonly Dictionary<EntityPropertyType, HashSet<EntityPropertyType>> IsIdOfMap = new()
    {
        {
            EntityPropertyType.ArtistId, [
                EntityPropertyType.ArtistName,
                EntityPropertyType.ArtistAlbumIds,
                EntityPropertyType.ArtistAlbums,
                EntityPropertyType.ArtistInfluenceIds,
                EntityPropertyType.ArtistInfluences,
                EntityPropertyType.ArtistInfluencerIds,
                EntityPropertyType.ArtistInfluencers,
                EntityPropertyType.ArtistBio,
            ]
        },
        
        {
            EntityPropertyType.AlbumId, [
                EntityPropertyType.AlbumName,
                EntityPropertyType.AlbumTrackIds,
                EntityPropertyType.AlbumTracks,
                EntityPropertyType.AlbumSimilarToIds,
                EntityPropertyType.AlbumSimilarTo,
            ]
        },

        { EntityPropertyType.ArtistAlbumIds, [EntityPropertyType.ArtistAlbums] },
        { EntityPropertyType.ArtistInfluenceIds, [EntityPropertyType.ArtistInfluences] },
        { EntityPropertyType.ArtistInfluencerIds, [EntityPropertyType.ArtistInfluencers] },
        
        { EntityPropertyType.AlbumTrackIds, [EntityPropertyType.AlbumTracks] },
        { EntityPropertyType.AlbumSimilarToIds, [EntityPropertyType.AlbumSimilarTo] },
        { EntityPropertyType.AlbumArtist, [EntityPropertyType.AlbumArtistId] },
    };
    
    /// <summary>
    /// Determines whether <paramref name="currentType"/> is an ID that can be used to fetch
    /// <paramref name="maybeChildType"/>.
    /// </summary>
    public static bool IsIdOf(this EntityPropertyType currentType, EntityPropertyType maybeChildType)
    {
        return IsIdOfMap.TryGetValue(currentType, out var childTypes)
            && childTypes.Contains(maybeChildType);
    }
    
    private static readonly Dictionary<EntityPropertyType, EntityPropertyType> GenericIdMap = new()
    {
        [EntityPropertyType.AllMusicArtistId] = EntityPropertyType.ArtistId,
        [EntityPropertyType.DiscogsArtistId] = EntityPropertyType.ArtistId,
        [EntityPropertyType.DeezerArtistId] = EntityPropertyType.ArtistId,
        [EntityPropertyType.LastFmArtistId] = EntityPropertyType.ArtistId,
        [EntityPropertyType.MusicBrainzArtistId] = EntityPropertyType.ArtistId,
        [EntityPropertyType.SpotifyArtistId] = EntityPropertyType.ArtistId,
        [EntityPropertyType.TidalArtistId] = EntityPropertyType.ArtistId,
        
        [EntityPropertyType.AllMusicAlbumId] = EntityPropertyType.AlbumId,
        [EntityPropertyType.AppleMusicAlbumId] = EntityPropertyType.AlbumId,
        [EntityPropertyType.DiscogsMasterId] = EntityPropertyType.AlbumId,
        [EntityPropertyType.DeezerAlbumId] = EntityPropertyType.AlbumId,
        [EntityPropertyType.LastFmAlbumId] = EntityPropertyType.AlbumId,
        [EntityPropertyType.MusicBrainzReleaseGroupId] = EntityPropertyType.AlbumId,
        [EntityPropertyType.MusicBrainzReleaseId] = EntityPropertyType.AlbumId,
        [EntityPropertyType.SpotifyAlbumId] = EntityPropertyType.AlbumId,
        [EntityPropertyType.TidalAlbumId] = EntityPropertyType.AlbumId,
        [EntityPropertyType.WikidataPerformerId] = EntityPropertyType.AlbumId,
    };
    
    /// <summary>
    /// Determines whether <paramref name="currentType"/> is an ID type and outputs the generic ID property type.
    /// </summary>
    public static bool IsGenericId(this EntityPropertyType currentType, out EntityPropertyType genericIdType)
    {
        return GenericIdMap.TryGetValue(currentType, out genericIdType);
    }
}