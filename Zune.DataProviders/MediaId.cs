using IF.Lastfm.Core;
using System;

namespace Zune.DataProviders;

public record MediaId(string Id, string Source, MediaType Type = default)
{
    public MediaId(Guid id, string source, MediaType type = default) : this(id.ToString(), source, type) { }

    public MediaId(int id, string source, MediaType type = default) : this(id.ToString(), source, type) { }

    /// <summary>
    /// Creates a canonical representation of this identifier from this source.
    /// </summary>
    public Guid IntoGuid()
    {
        var idBytes = MD5.GetHash($"{Id}/{Source}");
        return new Guid(idBytes);
    }

    /// <summary>
    /// Parses <see cref="Id"/> as a GUID.
    /// </summary>
    public Guid AsGuid() => Guid.Parse(Id);

    /// <summary>
    /// Parses <see cref="Id"/> as a signed 32-bit integer.
    /// </summary>
    public int AsInt32() => int.Parse(Id);
}

[Flags]
public enum MediaType : uint
{
    Unknown     = 0x00000000,
    Any         = 0xFFFFFFFF,

    Album       = 1 << 1,
    App         = 1 << 2,
    Artist      = 1 << 3,
    Channel     = 1 << 4,
    Feature     = 1 << 5,
    Genre       = 1 << 6,
    Image       = 1 << 7,
    Playlist    = 1 << 8,
    Podcast     = 1 << 9,
    Right       = 1 << 10,
    Track       = 1 << 11,
    Video       = 1 << 12,
}

public static class KnownMediaSources
{
    public const string AllMusic = "allmusic";
    public const string Amazon = "amazon";
    public const string AppleMusic = "applemusic";
    public const string Deezer = "deezer";
    public const string Discogs = "discogs";
    public const string LastFM = "lastfm";
    public const string Listen = "listen";
    public const string MusicBrainz = "musicbrainz";
    public const string Spotify = "spotify";
    public const string Taddy = "taddy";
    public const string Tidal = "tidal";
    public const string YouTube = "youtube";

    public static bool OrdinalEquals(this string source1, string source2) => source1.Equals(source2, StringComparison.Ordinal);
}
