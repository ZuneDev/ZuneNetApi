namespace Zune.Net.Ontology.Identifiers;

public class DiscogsIdProperty(DiscogsEntityType providerEntityType, EntityType entityType, EntityFact fact)
    : TypedEntityIdProperty<ulong, DiscogsEntityType>(providerEntityType, entityType, fact)
{
    public static DiscogsIdProperty Artist => new(DiscogsEntityType.Artist,
        EntityType.Artist, EntityFact.ArtistId);
    
    public static DiscogsIdProperty Release => new(DiscogsEntityType.Release,
        EntityType.Album, EntityFact.AlbumId);
    
    public static DiscogsIdProperty Master => new(DiscogsEntityType.Master,
        EntityType.Album, EntityFact.AlbumId);
    
    public static DiscogsIdProperty Track => new(DiscogsEntityType.Track,
        EntityType.Track, EntityFact.TrackId);

    public override ulong Parse(string value) => ulong.Parse(value);
}

public enum DiscogsEntityType
{
    Artist,
    Release,
    Label,
    Master,
    Track,
    Composition,
    Style,
    Genre,
}