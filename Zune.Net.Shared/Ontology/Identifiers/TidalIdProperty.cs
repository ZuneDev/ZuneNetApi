using Zune.Net.Ontology.BaseProperties;

namespace Zune.Net.Ontology.Identifiers;

public class TidalIdProperty(TidalEntityType providerEntityType, EntityType entityType, EntityFact fact)
    : TypedEntityIdProperty<ulong, TidalEntityType>(providerEntityType, entityType, fact)
{
    public static TidalIdProperty Artist => new(TidalEntityType.Artist,
        EntityType.Artist, EntityFact.Artist);
    
    public static TidalIdProperty Album => new(TidalEntityType.Album,
        EntityType.Album, EntityFact.Album);
    
    public static TidalIdProperty Track => new(TidalEntityType.Track,
        EntityType.Track, EntityFact.Track);

    public override ulong Parse(string value) => ulong.Parse(value);
}

public enum TidalEntityType
{
    Artist,
    Album,
    Track,
    MusicVideo
}