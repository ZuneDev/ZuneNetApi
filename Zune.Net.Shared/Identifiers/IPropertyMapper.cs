using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zune.Net.Identifiers;

public interface IPropertyMapper
{
    IReadOnlySet<PropertyMapping> AvailableMappings { get; }
    
    /// <summary>
    /// Maps the provided inputs to their outputs.
    /// </summary>
    Task<IPropertyBag> ExecuteAsync(IPropertyBag inputs);
}

public record PropertyMapping(int Cost, IReadOnlyPropertySet Inputs, IReadOnlyPropertySet Outputs);

public record EntityProperty(EntityType EntityType, EntityPropertyType PropertyType);

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
    
    ArtistName,
    ArtistAlbums,
    ArtistInflunces,
    ArtistInfluencers,
    
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
    
    AlbumName,
    AlbumTracks,
    AlbumSimilarTo,
    
    /* Universal */
}