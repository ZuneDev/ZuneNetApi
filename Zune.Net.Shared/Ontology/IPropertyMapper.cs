using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Zune.Net.Ontology;

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

public interface IEntityProperty
{
    EntityType EntityType { get; }
    EntityFact Fact { get; }
}

public record EntityProperty(EntityType EntityType, EntityFact Fact) : IEntityProperty
{
    public override string ToString() => $"{EntityType}.{Fact}";
}

public record TypedEntityProperty<T>(EntityType EntityType, EntityFact Fact)
    : EntityProperty(EntityType, Fact)
{
    public override int GetHashCode() => base.GetHashCode();

    protected override Type EqualityContract => base.EqualityContract;
}

public record TypedListEntityProperty<T>(EntityType EntityType, EntityFact Fact, TypedEntityProperty<T> ChildProperty)
    : EntityProperty(EntityType, Fact)
{
    public override int GetHashCode() => base.GetHashCode();

    protected override Type EqualityContract => base.EqualityContract;
}

public static class Ep
{
    public static class Artist
    {
        public static readonly TypedEntityProperty<string> Id = new(EntityType.Artist, EntityFact.ArtistId);
        public static readonly TypedEntityProperty<string> AllMusicId = new(EntityType.Artist, EntityFact.AllMusicArtistId);
        public static readonly TypedEntityProperty<int> DiscogsId = new(EntityType.Artist, EntityFact.DiscogsArtistId);
        public static readonly TypedEntityProperty<int> DeezerId = new(EntityType.Artist, EntityFact.DeezerArtistId);
        public static readonly TypedEntityProperty<string> LastFmId = new(EntityType.Artist, EntityFact.LastFmArtistId);
        public static readonly TypedEntityProperty<Guid> MusicBrainzId = new(EntityType.Artist, EntityFact.MusicBrainzArtistId);
        public static readonly TypedEntityProperty<string> SpotifyId = new(EntityType.Artist, EntityFact.SpotifyArtistId);
        public static readonly TypedEntityProperty<int> TidalId = new(EntityType.Artist, EntityFact.TidalArtistId);
        public static readonly TypedEntityProperty<string> WikidataPerformerId = new(EntityType.Artist, EntityFact.WikidataPerformerId);
    
        public static readonly TypedEntityProperty<string> Name = new(EntityType.Artist, EntityFact.ArtistName);
        public static TypedListEntityProperty<T> AlbumIds<T>(TypedEntityProperty<T> childProp) => new(EntityType.Album, EntityFact.ArtistAlbumIds, childProp);
        public static TypedListEntityProperty<T> InfluenceIds<T>(TypedEntityProperty<T> childProp) => new(EntityType.Album, EntityFact.ArtistInfluenceIds, childProp);
        public static TypedListEntityProperty<T> InfluencerIds<T>(TypedEntityProperty<T> childProp) => new(EntityType.Album, EntityFact.ArtistInfluencerIds, childProp);
        public static TypedListEntityProperty<T> SimilarToIds<T>(TypedEntityProperty<T> childProp) => new(EntityType.Album, EntityFact.ArtistSimilarToIds, childProp);
        public static readonly TypedEntityProperty<string> Bio = new(EntityType.Artist, EntityFact.ArtistBio);
        public static readonly TypedListEntityProperty<string> ImageUrls = new(EntityType.Artist, EntityFact.ArtistImageUrls, Image.ImageUrl);
        public static readonly TypedListEntityProperty<string> PrimaryImageUrl = new(EntityType.Artist, EntityFact.ArtistPrimaryImageUrl, Image.ImageUrl);
        
    }

    public static class Album
    {
        public static readonly TypedEntityProperty<string> Id = new(EntityType.Album, EntityFact.AlbumId);
        public static readonly TypedEntityProperty<string> AllMusicId = new(EntityType.Album, EntityFact.AllMusicAlbumId);
        public static readonly TypedEntityProperty<int> AppleMusicId = new(EntityType.Album, EntityFact.AppleMusicAlbumId);
        public static readonly TypedEntityProperty<int> DiscogsMasterId = new(EntityType.Album, EntityFact.DiscogsMasterId);
        public static readonly TypedEntityProperty<int> DeezerId = new(EntityType.Album, EntityFact.DeezerAlbumId);
        public static readonly TypedEntityProperty<string> LastFmId = new(EntityType.Album, EntityFact.LastFmAlbumId);
        public static readonly TypedEntityProperty<Guid> MusicBrainzReleaseGroupId = new(EntityType.Album, EntityFact.MusicBrainzReleaseGroupId);
        public static readonly TypedEntityProperty<Guid> MusicBrainzReleaseId = new(EntityType.Album, EntityFact.MusicBrainzReleaseId);
        public static readonly TypedEntityProperty<string> SpotifyId = new(EntityType.Album, EntityFact.SpotifyAlbumId);
        public static readonly TypedEntityProperty<int> TidalId = new(EntityType.Album, EntityFact.TidalAlbumId);
    
        public static readonly TypedEntityProperty<string> Name = new(EntityType.Album, EntityFact.AlbumName);
        public static TypedListEntityProperty<T> TrackIds<T>(TypedEntityProperty<T> childProp) => new(EntityType.Album, EntityFact.AlbumTrackIds, childProp);
        public static TypedListEntityProperty<T> SimilarToIds<T>(TypedEntityProperty<T> childProp) => new(EntityType.Album, EntityFact.AlbumSimilarToIds, childProp);
        public static readonly TypedEntityProperty<string> ArtistId = new(EntityType.Album, EntityFact.AlbumArtistId);
        public static readonly TypedEntityProperty<string> ImageId = new(EntityType.Album, EntityFact.AlbumImageId);
        public static readonly TypedEntityProperty<string> ImageUrl = new(EntityType.Album, EntityFact.AlbumImageUrl);
    }

    public static class Image
    {
        public static readonly TypedEntityProperty<string> Id = new(EntityType.Image, EntityFact.ImageId);
        public static readonly TypedEntityProperty<Guid> CoverArtArchiveId = new(EntityType.Image, EntityFact.CoverArtArchiveId);
        public static readonly TypedEntityProperty<Guid> ZuneId = new(EntityType.Image, EntityFact.ZuneImageId);
    
        public static readonly TypedEntityProperty<string> ImageUrl = new(EntityType.Image, EntityFact.ImageUrl);
        public static readonly TypedEntityProperty<System.IO.Stream> ImageStream = new(EntityType.Image, EntityFact.ImageStream);
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

public enum EntityFact
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
    AlbumImageId,
    AlbumImageUrl,
    
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
    private static readonly Dictionary<EntityFact, HashSet<EntityFact>> IsIdOfMap = new()
    {
        {
            EntityFact.ArtistId, [
                EntityFact.ArtistName,
                EntityFact.ArtistAlbumIds,
                EntityFact.ArtistAlbums,
                EntityFact.ArtistInfluenceIds,
                EntityFact.ArtistInfluences,
                EntityFact.ArtistInfluencerIds,
                EntityFact.ArtistInfluencers,
                EntityFact.ArtistSimilarToIds,
                EntityFact.ArtistSimilarTo,
                EntityFact.ArtistBio,
                EntityFact.ArtistImageUrls,
                EntityFact.ArtistPrimaryImageUrl,
            ]
        },
        
        {
            EntityFact.AlbumId, [
                EntityFact.AlbumName,
                EntityFact.AlbumTrackIds,
                EntityFact.AlbumTracks,
                EntityFact.AlbumSimilarToIds,
                EntityFact.AlbumSimilarTo,
            ]
        },

        { EntityFact.ArtistAlbumIds, [EntityFact.ArtistAlbums] },
        { EntityFact.ArtistInfluenceIds, [EntityFact.ArtistInfluences] },
        { EntityFact.ArtistInfluencerIds, [EntityFact.ArtistInfluencers] },
        { EntityFact.ArtistSimilarToIds, [EntityFact.ArtistSimilarTo] },
        
        { EntityFact.AlbumTrackIds, [EntityFact.AlbumTracks] },
        { EntityFact.AlbumSimilarToIds, [EntityFact.AlbumSimilarTo] },
        { EntityFact.AlbumArtist, [EntityFact.AlbumArtistId] },
    };
    
    /// <summary>
    /// Determines whether <paramref name="currentType"/> is an ID that can be used to fetch
    /// <paramref name="maybeChildType"/>.
    /// </summary>
    public static bool IsIdOf(this EntityFact currentType, EntityFact maybeChildType)
    {
        return IsIdOfMap.TryGetValue(currentType, out var childTypes)
            && childTypes.Contains(maybeChildType);
    }
    
    private static readonly Dictionary<EntityFact, EntityFact> GenericIdMap = new()
    {
        [EntityFact.AllMusicArtistId] = EntityFact.ArtistId,
        [EntityFact.DiscogsArtistId] = EntityFact.ArtistId,
        [EntityFact.DeezerArtistId] = EntityFact.ArtistId,
        [EntityFact.LastFmArtistId] = EntityFact.ArtistId,
        [EntityFact.MusicBrainzArtistId] = EntityFact.ArtistId,
        [EntityFact.SpotifyArtistId] = EntityFact.ArtistId,
        [EntityFact.TidalArtistId] = EntityFact.ArtistId,
        
        [EntityFact.AllMusicAlbumId] = EntityFact.AlbumId,
        [EntityFact.AppleMusicAlbumId] = EntityFact.AlbumId,
        [EntityFact.DiscogsMasterId] = EntityFact.AlbumId,
        [EntityFact.DeezerAlbumId] = EntityFact.AlbumId,
        [EntityFact.LastFmAlbumId] = EntityFact.AlbumId,
        [EntityFact.MusicBrainzReleaseGroupId] = EntityFact.AlbumId,
        [EntityFact.MusicBrainzReleaseId] = EntityFact.AlbumId,
        [EntityFact.SpotifyAlbumId] = EntityFact.AlbumId,
        [EntityFact.TidalAlbumId] = EntityFact.AlbumId,
        [EntityFact.WikidataPerformerId] = EntityFact.AlbumId,
    };
    
    /// <summary>
    /// Determines whether <paramref name="currentType"/> is an ID type and outputs the generic ID property type.
    /// </summary>
    public static bool IsGenericId(this EntityFact currentType, out EntityFact genericIdType)
    {
        return GenericIdMap.TryGetValue(currentType, out genericIdType);
    }
}