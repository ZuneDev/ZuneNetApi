using Zune.Net.Ontology.BaseProperties;

namespace Zune.Net.Ontology.Identifiers;

public class SpotifyIdProperty(SpotifyEntityType providerEntityType, EntityType entityType)
    : TypedEntityIdProperty<string, SpotifyEntityType>(providerEntityType, entityType)
{
    public static SpotifyIdProperty Artist => new(SpotifyEntityType.Artist, EntityType.Artist);
    public static SpotifyIdProperty Album => new(SpotifyEntityType.Album, EntityType.Album);
    public static SpotifyIdProperty Track => new(SpotifyEntityType.Track, EntityType.Track);

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