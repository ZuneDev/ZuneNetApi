using Zune.Net.Ontology.BaseProperties;

namespace Zune.Net.Ontology.Identifiers;

public class AllMusicIdProperty(AllMusicEntityType providerEntityType, EntityType entityType)
    : TypedEntityIdProperty<string, AllMusicEntityType>(providerEntityType, entityType)
{
    public static AllMusicIdProperty Artist => new(AllMusicEntityType.Artist, EntityType.Artist);
    public static AllMusicIdProperty Album => new(AllMusicEntityType.Album, EntityType.Album);
    public static AllMusicIdProperty Song => new(AllMusicEntityType.Song, EntityType.Track);
    public static AllMusicIdProperty Composition => new(AllMusicEntityType.Composition, EntityType.Unknown);
    public static AllMusicIdProperty Release => new(AllMusicEntityType.Release, EntityType.Album);

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
