using Zune.Net.Ontology.BaseProperties;

namespace Zune.Net.Ontology.Identifiers;

public class LastFmIdProperty(LastFmEntityType providerEntityType, EntityType entityType)
    : TypedEntityIdProperty<string, LastFmEntityType>(providerEntityType, entityType)
{
    public static LastFmIdProperty Artist => new(LastFmEntityType.Artist, EntityType.Artist);
    public static LastFmIdProperty Album => new(LastFmEntityType.Album, EntityType.Album);
    public static LastFmIdProperty Track => new(LastFmEntityType.Track, EntityType.Track);

    public override string Parse(string value) => value;
}

public enum LastFmEntityType
{
    Artist,
    Album,
    Track,
    User
}