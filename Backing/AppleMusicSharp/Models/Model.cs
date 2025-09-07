using Newtonsoft.Json;

namespace AppleMusicSharp.Models;

public class Root<TData>
{
    [JsonProperty("data")]
    public List<TData> Data { get; set; }
}

public class Room
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("href")]
    public string Href { get; set; }

    [JsonProperty("attributes")]
    public RoomAttributes Attributes { get; set; }

    [JsonProperty("relationships")]
    public RoomRelationships Relationships { get; set; }
}

public class RoomAttributes
{
    [JsonProperty("defaultSort")]
    public string DefaultSort { get; set; }

    [JsonProperty("doNotFilter")]
    public bool DoNotFilter { get; set; }

    [JsonProperty("editorialElementKind")]
    public string EditorialElementKind { get; set; }

    [JsonProperty("lastModifiedDate")]
    public DateTime LastModifiedDate { get; set; }

    [JsonProperty("resourceTypes")]
    public List<string> ResourceTypes { get; set; }

    [JsonProperty("sorts")]
    public List<string> Sorts { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }
}

public class RoomRelationships
{
    [JsonProperty("contents")]
    public Contents Contents { get; set; }
}

public class Contents
{
    [JsonProperty("href")]
    public string Href { get; set; }

    [JsonProperty("next")]
    public string Next { get; set; }

    [JsonProperty("data")]
    public List<Album> Data { get; set; }
}

public class Album
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("href")]
    public string Href { get; set; }

    [JsonProperty("attributes")]
    public AlbumAttributes Attributes { get; set; }

    [JsonProperty("meta")]
    public AlbumMeta Meta { get; set; }
}

public class AlbumAttributes
{
    [JsonProperty("artistName")]
    public string ArtistName { get; set; }

    [JsonProperty("artwork")]
    public Artwork Artwork { get; set; }

    [JsonProperty("audioTraits")]
    public List<string> AudioTraits { get; set; }

    [JsonProperty("contentRating")]
    public string ContentRating { get; set; }

    [JsonProperty("copyright")]
    public string Copyright { get; set; }

    [JsonProperty("editorialNotes")]
    public EditorialNotes EditorialNotes { get; set; }

    [JsonProperty("genreNames")]
    public List<string> GenreNames { get; set; }

    [JsonProperty("isCompilation")]
    public bool IsCompilation { get; set; }

    [JsonProperty("isComplete")]
    public bool IsComplete { get; set; }

    [JsonProperty("isMasteredForItunes")]
    public bool IsMasteredForItunes { get; set; }

    [JsonProperty("isPrerelease")]
    public bool IsPrerelease { get; set; }

    [JsonProperty("isSingle")]
    public bool IsSingle { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("playParams")]
    public PlayParams PlayParams { get; set; }

    [JsonProperty("recordLabel")]
    public string RecordLabel { get; set; }

    [JsonProperty("releaseDate")]
    public string ReleaseDate { get; set; }

    [JsonProperty("trackCount")]
    public int TrackCount { get; set; }

    [JsonProperty("upc")]
    public string Upc { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }
}

public class Artwork
{
    [JsonProperty("bgColor")]
    public string BgColor { get; set; }

    [JsonProperty("hasP3")]
    public bool HasP3 { get; set; }

    [JsonProperty("height")]
    public int Height { get; set; }

    [JsonProperty("textColor1")]
    public string TextColor1 { get; set; }

    [JsonProperty("textColor2")]
    public string TextColor2 { get; set; }

    [JsonProperty("textColor3")]
    public string TextColor3 { get; set; }

    [JsonProperty("textColor4")]
    public string TextColor4 { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("width")]
    public int Width { get; set; }
}

public class EditorialNotes
{
    [JsonProperty("short")]
    public string Short { get; set; }

    [JsonProperty("standard")]
    public string Standard { get; set; }

    [JsonProperty("tagline")]
    public string Tagline { get; set; }
}

public class PlayParams
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("kind")]
    public string Kind { get; set; }
}

public class AlbumMeta
{
    [JsonProperty("contentVersion")]
    public ContentVersion ContentVersion { get; set; }
}

public class ContentVersion
{
    [JsonProperty("MZ_INDEXER")]
    public long MzIndexer { get; set; }

    [JsonProperty("RTCI")]
    public int Rtci { get; set; }
}
