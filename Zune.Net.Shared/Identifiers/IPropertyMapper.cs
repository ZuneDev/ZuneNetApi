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

public static class Ep
{
    public static class Artist
    {
        public static readonly EntityProperty Id = new(EntityType.Artist, EntityPropertyType.ArtistId);
        public static readonly EntityProperty AllMusicId = new(EntityType.Artist, EntityPropertyType.AllMusicArtistId);
        public static readonly EntityProperty DiscogsId = new(EntityType.Artist,EntityPropertyType.DiscogsArtistId);
        public static readonly EntityProperty DeezerId = new(EntityType.Artist,EntityPropertyType.DeezerArtistId);
        public static readonly EntityProperty LastFmId = new(EntityType.Artist,EntityPropertyType.LastFmArtistId);
        public static readonly EntityProperty MusicBrainzId = new(EntityType.Artist,EntityPropertyType.MusicBrainzArtistId);
        public static readonly EntityProperty SpotifyId = new(EntityType.Artist,EntityPropertyType.SpotifyArtistId);
        public static readonly EntityProperty TidalId = new(EntityType.Artist,EntityPropertyType.TidalArtistId);
        public static readonly EntityProperty WikidataId = new(EntityType.Artist, EntityPropertyType.WikidataPerformerId);
    
        public static readonly EntityProperty Name = new(EntityType.Artist,EntityPropertyType.ArtistName);
        public static readonly EntityProperty AlbumIds = new(EntityType.Artist,EntityPropertyType.ArtistAlbumIds);
        public static readonly EntityProperty Albums = new(EntityType.Artist,EntityPropertyType.ArtistAlbums);
        public static readonly EntityProperty InfluenceIds = new(EntityType.Artist,EntityPropertyType.ArtistInfluenceIds);
        public static readonly EntityProperty Influences = new(EntityType.Artist,EntityPropertyType.ArtistInfluences);
        public static readonly EntityProperty InfluencerIds = new(EntityType.Artist,EntityPropertyType.ArtistInfluencerIds);
        public static readonly EntityProperty Influencers = new(EntityType.Artist,EntityPropertyType.ArtistInfluencers);
        public static readonly EntityProperty SimilarToIds = new(EntityType.Artist,EntityPropertyType.ArtistSimilarToIds);
        public static readonly EntityProperty SimilarTo = new(EntityType.Artist,EntityPropertyType.ArtistSimilarTo);
        public static readonly EntityProperty Bio = new(EntityType.Artist,EntityPropertyType.ArtistBio);
        public static readonly EntityProperty Images = new(EntityType.Artist,EntityPropertyType.ArtistImageUrls);
        public static readonly EntityProperty PrimaryImage = new(EntityType.Artist,EntityPropertyType.ArtistPrimaryImageUrl);
    }

    public static class Album
    {
        public static readonly EntityProperty Id = new(EntityType.Album, EntityPropertyType.AlbumId);
        public static readonly EntityProperty AllMusicId = new(EntityType.Album, EntityPropertyType.AllMusicAlbumId);
        public static readonly EntityProperty AppleMusicId = new(EntityType.Album, EntityPropertyType.AppleMusicAlbumId);
        public static readonly EntityProperty DiscogsMasterId = new(EntityType.Album, EntityPropertyType.DiscogsMasterId);
        public static readonly EntityProperty DeezerId = new(EntityType.Album, EntityPropertyType.DeezerAlbumId);
        public static readonly EntityProperty LastFmId = new(EntityType.Album, EntityPropertyType.LastFmAlbumId);
        public static readonly EntityProperty MusicBrainzReleaseGroupId = new(EntityType.Album, EntityPropertyType.MusicBrainzReleaseGroupId);
        public static readonly EntityProperty MusicBrainzReleaseId = new(EntityType.Album, EntityPropertyType.MusicBrainzReleaseId);
        public static readonly EntityProperty SpotifyId = new(EntityType.Album, EntityPropertyType.SpotifyAlbumId);
        public static readonly EntityProperty TidalId = new(EntityType.Album, EntityPropertyType.TidalAlbumId);
    
        public static readonly EntityProperty Name = new(EntityType.Album, EntityPropertyType.AlbumName);
        public static readonly EntityProperty TrackIds = new(EntityType.Album, EntityPropertyType.AlbumTrackIds);
        public static readonly EntityProperty Tracks = new(EntityType.Album, EntityPropertyType.AlbumTracks);
        public static readonly EntityProperty SimilarToIds = new(EntityType.Album, EntityPropertyType.AlbumSimilarToIds);
        public static readonly EntityProperty SimilarTo = new(EntityType.Album, EntityPropertyType.AlbumSimilarTo);
        public static readonly EntityProperty ArtistId = new(EntityType.Album, EntityPropertyType.AlbumArtistId);
        public static readonly EntityProperty Artist = new(EntityType.Album, EntityPropertyType.AlbumArtist);
    }

    public static class Image
    {
        public static readonly EntityProperty Id = new(EntityType.Image, EntityPropertyType.ImageId);
        public static readonly EntityProperty CoverArtArchvieId = new(EntityType.Image, EntityPropertyType.CoverArtArchiveId);
        public static readonly EntityProperty ZuneId = new(EntityType.Image, EntityPropertyType.ZuneImageId);
    
        public static readonly EntityProperty ImageUrl = new(EntityType.Image, EntityPropertyType.ImageUrl);
        public static readonly EntityProperty ImageStream = new(EntityType.Image, EntityPropertyType.ImageStream);
    }
}

public enum EntityType
{
    Unknown,
    Artist,
    Album,
    Track,
    Genre,
    Feature,
    Image,
}

public enum EntityPropertyType
{
    /* Artists */
    AllMusicArtistId,
    BandcampArtistId,
    DiscogsArtistId,
    DeezerArtistId,
    LastFmArtistId,
    MusicBrainzArtistId,
    SpotifyArtistId,
    TidalArtistId,
    WikidataPerformerId,
    ArtistId,
    
    ArtistName,
    ArtistAlbumIds,
    ArtistAlbums,
    ArtistInfluenceIds,
    ArtistInfluences,
    ArtistInfluencerIds,
    ArtistInfluencers,
    ArtistSimilarToIds,
    ArtistSimilarTo,
    ArtistBio,
    ArtistImageUrls,
    ArtistPrimaryImageUrl,
    
    /* Albums */
    AllMusicAlbumId,
    AppleMusicAlbumId,
    BandcampAlbumId,
    DiscogsMasterId,
    DeezerAlbumId,
    LastFmAlbumId,
    MusicBrainzReleaseGroupId,
    MusicBrainzReleaseId,
    SpotifyAlbumId,
    TidalAlbumId,
    AlbumId,
    
    AlbumName,
    AlbumTrackIds,
    AlbumTracks,
    AlbumSimilarToIds,
    AlbumSimilarTo,
    AlbumArtistId,
    AlbumArtist,
    
    /* Images */
    BandcampImageId,
    CoverArtArchiveId,
    ZuneImageId,
    ImageId,
    
    ImageUrl,
    ImageStream,
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
                EntityPropertyType.ArtistSimilarToIds,
                EntityPropertyType.ArtistSimilarTo,
                EntityPropertyType.ArtistBio,
                EntityPropertyType.ArtistImageUrls,
                EntityPropertyType.ArtistPrimaryImageUrl,
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
        { EntityPropertyType.ArtistSimilarToIds, [EntityPropertyType.ArtistSimilarTo] },
        
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