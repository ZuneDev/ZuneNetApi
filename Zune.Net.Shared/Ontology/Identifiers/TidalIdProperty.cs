using Zune.Net.Ontology.BaseProperties;

namespace Zune.Net.Ontology.Identifiers;

public class TidalIdProperty(TidalEntityType providerEntityType, EntityType entityType)
    : TypedEntityIdProperty<ulong, TidalEntityType>(providerEntityType, entityType)
{
    public static TidalIdProperty Artist => new(TidalEntityType.Artist, EntityType.Artist);
    public static TidalIdProperty Album => new(TidalEntityType.Album, EntityType.Album);
    public static TidalIdProperty Track => new(TidalEntityType.Track, EntityType.Track);

    public override ulong Parse(string value) => ulong.Parse(value);
}

public enum TidalEntityType
{
    Artist,
    Album,
    Track,
    MusicVideo
}