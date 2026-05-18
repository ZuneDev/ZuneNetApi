using Zune.Net.Ontology.BaseProperties;

namespace Zune.Net.Ontology.Identifiers;

public class DeezerIdProperty(DeezerEntityType providerEntityType, EntityType entityType, EntityFact fact)
    : TypedEntityIdProperty<ulong, DeezerEntityType>(providerEntityType, entityType, fact)
{
    public static DeezerIdProperty Artist => new(DeezerEntityType.Artist,
        EntityType.Artist, EntityFact.Artist);
    
    public static DeezerIdProperty Album => new(DeezerEntityType.Album,
        EntityType.Album, EntityFact.Album);
    
    public static DeezerIdProperty Track => new(DeezerEntityType.Track,
        EntityType.Track, EntityFact.Track);

    public override ulong Parse(string value) => ulong.Parse(value);
}

public enum DeezerEntityType
{
    Artist,
    Album,
    Track,
    Show
}