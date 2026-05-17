namespace Zune.Net.Ontology.Identifiers;

public class SpotifyIdProperty(SpotifyEntityType providerEntityType, EntityType entityType, EntityFact fact)
    : TypedEntityIdProperty<string, SpotifyEntityType>(providerEntityType, entityType, fact)
{
    public static SpotifyIdProperty Artist => new(SpotifyEntityType.Artist,
        EntityType.Artist, EntityFact.ArtistId);
    
    public static SpotifyIdProperty Album => new(SpotifyEntityType.Album,
        EntityType.Album, EntityFact.AlbumId);
    
    public static SpotifyIdProperty Track => new(SpotifyEntityType.Track,
        EntityType.Track, EntityFact.TrackId);

    public override string Parse(string value) => value;
}

public enum SpotifyEntityType
{
    Artist,
    Songwriter,
    Album,
    Track,
    Show,
    Playlist,
    Episode,
    User
}