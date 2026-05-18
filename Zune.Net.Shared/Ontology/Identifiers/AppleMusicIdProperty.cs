using Zune.Net.Ontology.BaseProperties;

namespace Zune.Net.Ontology.Identifiers;

public class AppleMusicIdProperty(AppleMusicEntityType providerEntityType, EntityType entityType, EntityFact fact)
    : TypedEntityIdProperty<ulong, AppleMusicEntityType>(providerEntityType, entityType, fact)
{
    public static AppleMusicIdProperty Album => new(AppleMusicEntityType.Album,
        EntityType.Album, EntityFact.Album);
    
    public static AppleMusicIdProperty Artist => new(AppleMusicEntityType.Artist,
        EntityType.Artist, EntityFact.Artist);
    
    public static AppleMusicIdProperty Track => new(AppleMusicEntityType.Track,
        EntityType.Track, EntityFact.Track);

    public override ulong Parse(string value) => ulong.Parse(value);
}

public enum AppleMusicEntityType
{
    Album,
    Artist,
    MusicVideo,
    MusicMovie,
    Label,
    Track
}