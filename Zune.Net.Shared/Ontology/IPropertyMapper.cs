using System;
using System.Collections.Generic;
using System.ComponentModel;
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

public interface IParsableEntityProperty : IEntityProperty
{
    object ParseObject(string value);
}

public interface ITypedEntityProperty<out T> : IParsableEntityProperty
{
    T Parse(string value);
}

public record TypedEntityProperty<T>(EntityType EntityType, EntityFact Fact)
    : EntityProperty(EntityType, Fact), ITypedEntityProperty<T>
{
    public virtual T Parse(string value)
    {
        var converter = TypeDescriptor.GetConverter(typeof(T));
        var result = converter.ConvertFromInvariantString(value);
        return (T)result;
    }
    
    public object ParseObject(string value) => Parse(value);

    public override int GetHashCode() => base.GetHashCode();

    protected override Type EqualityContract => base.EqualityContract;
    
    public override string ToString() => base.ToString();
}

public interface ITypedEntityIdProperty<out TId, out TProviderEntities>
    : ITypedEntityProperty<TId>
    where TProviderEntities : Enum
{
    /// <summary>
    /// The type of the entity this property applies to, specific to the provider of this identifier.
    /// </summary>
    TProviderEntities ProviderEntityType { get; }
}

public abstract class TypedEntityIdProperty<TId, TProviderEntities>
    (TProviderEntities providerEntityType, EntityType entityType, EntityFact fact)
    : ITypedEntityProperty<TId>
    where TProviderEntities : Enum
{
    public TProviderEntities ProviderEntityType { get; } = providerEntityType;
    public EntityType EntityType { get; } = entityType;
    public EntityFact Fact { get; } = fact;
    
    public abstract TId Parse(string value);
    public virtual object ParseObject(string value) => Parse(value);

    public override bool Equals(object otherProp)
    {
        if (otherProp is null)
            return false;
        
        if (ReferenceEquals(this, otherProp))
            return true;
        
        if (otherProp is not TypedEntityIdProperty<TId, TProviderEntities> otherTypedIdProp)
            return false;
        
        return ProviderEntityType.Equals(otherTypedIdProp.ProviderEntityType)
            && EntityType == otherTypedIdProp.EntityType
            && Fact == otherTypedIdProp.Fact;
    }

    public override int GetHashCode() => (ProviderEntityType, EntityType, Fact).GetHashCode();
    
    public override string ToString() => $"{typeof(TProviderEntities).Name}:{ProviderEntityType}.{Fact}";
}

/// <summary>
/// Represents a property that is a reference to another entity using its identifier.
/// </summary>
/// <param name="EntityType">The type of the entity this property applies to.</param>
/// <param name="Fact">The fact on the entity this property represents.</param>
/// <param name="IdProperty">The property on the referenced entity that contains the identifier.</param>
/// <typeparam name="TId">The type of the identifier for the referenced entity.</typeparam>
/// <typeparam name="TProvider">An enum specifying who assigned this ID and what kind of provider entity it applies to.</typeparam>
public record TypedEntityReferenceProperty<TId, TProvider>(EntityType EntityType, EntityFact Fact,
    ITypedEntityIdProperty<TId, TProvider> IdProperty)
    : EntityProperty(EntityType, Fact)
    where TProvider : Enum
{
    public override int GetHashCode() => base.GetHashCode();

    protected override Type EqualityContract => base.EqualityContract;
    
    public override string ToString() => $"{base.ToString()}<{IdProperty}>";
}

public record TypedListEntityProperty<T>(EntityType EntityType, EntityFact Fact, ITypedEntityProperty<T> ChildProperty)
    : EntityProperty(EntityType, Fact)
{
    public override int GetHashCode() => base.GetHashCode();

    protected override Type EqualityContract => base.EqualityContract;
    
    public override string ToString() => base.ToString();
}

public static class Ep
{
    public static class Artist
    {
        public static readonly TypedEntityProperty<string> Name = new(EntityType.Artist, EntityFact.ArtistName);
        public static TypedListEntityProperty<T> AlbumIds<T>(ITypedEntityProperty<T> childProp) => new(EntityType.Album, EntityFact.ArtistAlbumIds, childProp);
        public static TypedListEntityProperty<T> InfluenceIds<T>(ITypedEntityProperty<T> childProp) => new(EntityType.Album, EntityFact.ArtistInfluenceIds, childProp);
        public static TypedListEntityProperty<T> InfluencerIds<T>(ITypedEntityProperty<T> childProp) => new(EntityType.Album, EntityFact.ArtistInfluencerIds, childProp);
        public static TypedListEntityProperty<T> SimilarToIds<T>(ITypedEntityProperty<T> childProp) => new(EntityType.Album, EntityFact.ArtistSimilarToIds, childProp);
        public static readonly TypedEntityProperty<string> Bio = new(EntityType.Artist, EntityFact.ArtistBio);
        public static readonly TypedListEntityProperty<string> ImageUrls = new(EntityType.Artist, EntityFact.ArtistImageUrls, Image.ImageUrl);
        public static readonly TypedListEntityProperty<string> PrimaryImageUrl = new(EntityType.Artist, EntityFact.ArtistPrimaryImageUrl, Image.ImageUrl);
    }

    public static class Album
    {
        public static readonly TypedEntityProperty<string> Name = new(EntityType.Album, EntityFact.AlbumName);
        public static TypedListEntityProperty<T> TrackIds<T>(ITypedEntityProperty<T> childProp) => new(EntityType.Album, EntityFact.AlbumTrackIds, childProp);
        public static TypedListEntityProperty<T> SimilarToIds<T>(ITypedEntityProperty<T> childProp) => new(EntityType.Album, EntityFact.AlbumSimilarToIds, childProp);
        public static readonly TypedEntityProperty<string> ArtistId = new(EntityType.Album, EntityFact.AlbumArtistId);
        public static TypedEntityProperty<T> ImageId<T>(ITypedEntityProperty<T> imageProp) => new(EntityType.Album, EntityFact.AlbumImageId);
        public static readonly TypedEntityProperty<string> ImageUrl = new(EntityType.Album, EntityFact.AlbumImageUrl);
    }

    public static class Image
    {
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
    Unknown,
    
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
    
    /* Tracks */
    TrackId,
    TrackName,
    TrackNumber,
    TrackDuration,
    TrackDiscNumber,
    
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