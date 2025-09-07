using Newtonsoft.Json;

namespace AppleMusicSharp.Models;

public class EditorialElement
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("href")]
    public string Href { get; set; }

    [JsonProperty("attributes")]
    public EditorialElementAttributes Attributes { get; set; }

    [JsonProperty("relationships")]
    public EditorialElementRelationships? Relationships { get; set; }
}

public class EditorialElementAttributes
{
    [JsonProperty("designBadge")]
    public string DesignBadge { get; set; }
    
    [JsonProperty("designTag")]
    public string DesignTag { get; set; }

    [JsonProperty("doNotFilter")]
    public bool DoNotFilter { get; set; }

    [JsonProperty("editorialElementKind")]
    public string EditorialElementKind { get; set; }

    [JsonProperty("editorialArtwork")]
    public EditorialArtwork? EditorialArtwork { get; set; }

    [JsonProperty("lastModifiedDate")]
    public DateTimeOffset LastModifiedDate { get; set; }

    [JsonProperty("resourceTypes")]
    public List<string> ResourceTypes { get; set; }
}

public class EditorialElementRelationships
{
    [JsonProperty("contents")]
    public Contents<DataReference> Contents { get; set; }
}

public class EditorialArtwork
{
    [JsonProperty("staticDetailSquare")]
    public ArtworkWithGradient? StaticDetailSquare { get; set; }

    [JsonProperty("staticDetailTall")]
    public ArtworkWithGradient? StaticDetailTall { get; set; }

    [JsonProperty("storeFlowcase")]
    public ArtworkWithGradient? StoreFlowcase { get; set; }

    [JsonProperty("subscriptionHero")]
    public ArtworkWithGradient? SubscriptionHero { get; set; }

    [JsonProperty("subscriptionCover")]
    public ArtworkWithGradient? SubscriptionCover { get; set; }

    [JsonProperty("superHeroTall")]
    public ArtworkWithGradient? SuperHeroTall { get; set; }

    [JsonProperty("brandLogo")]
    public ArtworkWithGradient? BrandLogo { get; set; }
}

public class ArtworkWithGradient : Artwork
{
    [JsonProperty("gradient")]
    public Gradient Gradient { get; set; }

    [JsonProperty("textGradient")]
    public List<string> TextGradient { get; set; }
}

public class Gradient
{
    [JsonProperty("color")]
    public string Color { get; set; }

    [JsonProperty("y2")]
    public double? Y2 { get; set; }
}
