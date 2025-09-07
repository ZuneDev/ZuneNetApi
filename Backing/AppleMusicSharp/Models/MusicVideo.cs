using Newtonsoft.Json;

namespace AppleMusicSharp.Models;

public class MusicVideo
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("href")]
    public string Href { get; set; }

    [JsonProperty("attributes")]
    public MusicVideoAttributes Attributes { get; set; }
}

public class MusicVideoAttributes
{
    public string ArtistName { get; set; }
    
    public Artwork Artwork { get; set; }
    
    public string ContentRating { get; set; }
    
    public int DurationInMilliseconds { get; set; }
    
    public List<string> GenreNames { get; set; }
    
    [JsonProperty("has4K")]
    public bool Has4K { get; set; }
    
    [JsonProperty("hasHDR")]
    public bool HasHDR { get; set; }
    
    [JsonProperty("isrc")]
    public string ISRC { get; set; }
    
    public string Name { get; set; }
    
    public PlayParams PlayParams { get; set; }
    
    public List<Preview> Previews { get; set; }
    
    public DateTime ReleaseDate { get; set; }
    
    public string Url { get; set; }
    
    public List<string> VideoTraits { get; set; }
    
    [JsonIgnore]
    public TimeSpan Duration => TimeSpan.FromMilliseconds(DurationInMilliseconds);
}