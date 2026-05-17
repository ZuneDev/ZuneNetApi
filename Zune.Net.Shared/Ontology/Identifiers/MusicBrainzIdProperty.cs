using System;

namespace Zune.Net.Ontology.Identifiers;

public class MusicBrainzIdProperty(MusicBrainzEntityType providerEntityType, EntityType entityType, EntityFact fact)
    : TypedEntityIdProperty<Guid, MusicBrainzEntityType>(providerEntityType, entityType, fact)
{
    public static MusicBrainzIdProperty Artist => new(MusicBrainzEntityType.Artist,
        EntityType.Artist, EntityFact.ArtistId);
    
    public static MusicBrainzIdProperty CoverArt => new(MusicBrainzEntityType.CoverArt,
        EntityType.Image, EntityFact.ImageId);
    
    public static MusicBrainzIdProperty Release => new(MusicBrainzEntityType.Release,
        EntityType.Album, EntityFact.AlbumId);
    
    public static MusicBrainzIdProperty ReleaseGroup => new(MusicBrainzEntityType.ReleaseGroup,
        EntityType.Unknown, EntityFact.Unknown);
    
    public static MusicBrainzIdProperty Recording => new(MusicBrainzEntityType.Recording,
        EntityType.Track, EntityFact.TrackId);

    public override Guid Parse(string value) => Guid.Parse(value);
}

public enum MusicBrainzEntityType
{
    Artist,
    Area,
    Event,
    Genre,
    Instrument,
    CoverArt,
    Label,
    Place,
    Series,
    Recording,
    Release,
    ReleaseGroup,
    Work,
}