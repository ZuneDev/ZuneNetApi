namespace Zune.Net.Ontology.Identifiers;

public class AllMusicIdProperty(AllMusicEntityType providerEntityType, EntityType entityType, EntityFact fact)
    : TypedEntityIdProperty<string, AllMusicEntityType>(providerEntityType, entityType, fact)
{
    public static AllMusicIdProperty Artist => new(AllMusicEntityType.Artist,
        EntityType.Artist, EntityFact.ArtistId);
    
    public static AllMusicIdProperty Album => new(AllMusicEntityType.Album,
        EntityType.Album, EntityFact.AlbumId);
    
    public static AllMusicIdProperty Song => new(AllMusicEntityType.Song,
        EntityType.Track, EntityFact.TrackId);
    
    public static AllMusicIdProperty Composition => new(AllMusicEntityType.Composition,
        EntityType.Unknown, EntityFact.Unknown);
    
    public static AllMusicIdProperty Release => new(AllMusicEntityType.Release,
        EntityType.Album, EntityFact.AlbumId);

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
