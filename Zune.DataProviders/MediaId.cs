using IF.Lastfm.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zune.DataProviders;

public record MediaId(string Id, string Source)
{
    public MediaId(Guid id, string source) : this(id.ToString(), source) { }

    public MediaId(int id, string source) : this(id.ToString(), source) { }

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
