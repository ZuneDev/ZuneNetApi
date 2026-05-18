using Zune.Net.Ontology.BaseProperties;

namespace Zune.Net.Ontology.Identifiers;

public class AllMusicIdProperty(AllMusicEntityType providerEntityType, EntityType entityType, EntityFact fact)
    : TypedEntityIdProperty<string, AllMusicEntityType>(providerEntityType, entityType, fact)
{
    public static AllMusicIdProperty Artist => new(AllMusicEntityType.Artist,
        EntityType.Artist, EntityFact.Artist);
    
    public static AllMusicIdProperty Album => new(AllMusicEntityType.Album,
        EntityType.Album, EntityFact.Album);
    
    public static AllMusicIdProperty Song => new(AllMusicEntityType.Song,
        EntityType.Track, EntityFact.Track);
    
    public static AllMusicIdProperty Composition => new(AllMusicEntityType.Composition,
        EntityType.Unknown, EntityFact.Unknown);
    
    public static AllMusicIdProperty Release => new(AllMusicEntityType.Release,
        EntityType.Album, EntityFact.Album);

    public override string Parse(string value) => value;
}

public enum AllMusicEntityType
{
    Artist,
    Album,
    Song,
    Composition,
    Release,
    Genre,
    Style = Genre,
    Performance
}
