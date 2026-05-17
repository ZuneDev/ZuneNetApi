namespace Zune.Net.Ontology.Identifiers;

public class LastFmIdProperty(LastFmEntityType providerEntityType, EntityType entityType, EntityFact fact)
    : TypedEntityIdProperty<string, LastFmEntityType>(providerEntityType, entityType, fact)
{
    public static LastFmIdProperty Artist => new(LastFmEntityType.Artist,
        EntityType.Artist, EntityFact.ArtistId);
    
    public static LastFmIdProperty Album => new(LastFmEntityType.Album,
        EntityType.Album, EntityFact.AlbumId);
    
    public static LastFmIdProperty Track => new(LastFmEntityType.Track,
        EntityType.Track, EntityFact.TrackId);

    public override string Parse(string value) => value;
}

public enum LastFmEntityType
{
    Artist,
    Album,
    Track,
    User
}