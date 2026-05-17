namespace Zune.Net.Ontology.Identifiers;

public class TidalIdProperty(TidalEntityType providerEntityType, EntityType entityType, EntityFact fact)
    : TypedEntityIdProperty<ulong, TidalEntityType>(providerEntityType, entityType, fact)
{
    public static TidalIdProperty Artist => new(TidalEntityType.Artist,
        EntityType.Artist, EntityFact.ArtistId);
    
    public static TidalIdProperty Album => new(TidalEntityType.Album,
        EntityType.Album, EntityFact.AlbumId);
    
    public static TidalIdProperty Track => new(TidalEntityType.Track,
        EntityType.Track, EntityFact.TrackId);

    public override ulong Parse(string value) => ulong.Parse(value);
}

public enum TidalEntityType
{
    Artist,
    Album,
    Track,
    MusicVideo
}