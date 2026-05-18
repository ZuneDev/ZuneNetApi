using Zune.Net.Ontology.BaseProperties;

namespace Zune.Net.Ontology.Identifiers;

public class LastFmIdProperty(LastFmEntityType providerEntityType, EntityType entityType, EntityFact fact)
    : TypedEntityIdProperty<string, LastFmEntityType>(providerEntityType, entityType, fact)
{
    public static LastFmIdProperty Artist => new(LastFmEntityType.Artist,
        EntityType.Artist, EntityFact.Artist);
    
    public static LastFmIdProperty Album => new(LastFmEntityType.Album,
        EntityType.Album, EntityFact.Album);
    
    public static LastFmIdProperty Track => new(LastFmEntityType.Track,
        EntityType.Track, EntityFact.Track);

    public override string Parse(string value) => value;
}

public enum LastFmEntityType
{
    Artist,
    Album,
    Track,
    User
}