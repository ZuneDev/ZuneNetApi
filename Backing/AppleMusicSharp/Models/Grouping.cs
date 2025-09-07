using Newtonsoft.Json;

namespace AppleMusicSharp.Models;

public class GroupingRoot : Root<Grouping>
{
    [JsonProperty("resources")]
    public GroupingResourceMap Resources { get; set; }
}

public class Grouping
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("href")]
    public string Href { get; set; }
}

public class GroupingResourceMap
{
    [JsonProperty("albums")]
    public Dictionary<string, Album> Albums { get; set; }

    [JsonProperty("editorial-elements")]
    public Dictionary<string, EditorialElement> EditorialElements { get; set; }

    [JsonProperty("music-videos")]
    public Dictionary<string, MusicVideo> MusicVideos { get; set; }
}