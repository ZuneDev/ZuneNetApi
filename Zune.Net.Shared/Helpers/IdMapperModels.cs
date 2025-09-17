#nullable enable

using System;

namespace Zune.Net.Helpers;

public enum WikidataProperty
{
    /* Artist */
    ALArtistId = 1728,
    DCArtistId = 1953,
    DZArtistId = 2722,
    MBArtistId = 434,
    SPArtistId = 1902,
    TIArtistId = 4576,
    
    /* Albums */
    ALAlbumId = 1729,
    AMAlbumId = 2281,
    DCMasterId = 1954,
    DZAlbumId = 2723,
    MBReleaseGroupId = 436,
    MBReleaseId = 5813,
    SPAlbumId = 2205,
    TIAlbumId = 4577,
    
    /* Universal */
    FMId = 3192,
}

public class ArtistIdMap
{
    public string? AllMusic { get; set; }
    public string? Discogs { get; set; }
    public string? Deezer { get; set; }
    public string? LastFM { get; set; }
    public Guid? MusicBrainz { get; set; }
    public string? Spotify { get; set; }
    public string? Tidal { get; set; }
}

public class AlbumIdMap
{
    public string? AppleMusic { get; set; }
    public string? AllMusic { get; set; }
    public string? Discogs { get; set; }
    public string? Deezer { get; set; }
    public string? LastFM { get; set; }
    public Guid? MusicBrainzReleaseGroup { get; set; }
    public string? Spotify { get; set; }
    public string? Tidal { get; set; }
}