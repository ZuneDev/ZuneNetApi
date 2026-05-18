using System;
using Zune.Net.Ontology.BaseProperties;

namespace Zune.Net.Ontology.Identifiers;

public class MusicBrainzIdProperty(MusicBrainzEntityType providerEntityType, EntityType entityType, EntityFact fact)
    : TypedEntityIdProperty<Guid, MusicBrainzEntityType>(providerEntityType, entityType, fact)
{
    public static MusicBrainzIdProperty Artist => new(MusicBrainzEntityType.Artist,
        EntityType.Artist, EntityFact.Artist);
    
    public static MusicBrainzIdProperty Release => new(MusicBrainzEntityType.Release,
        EntityType.Album, EntityFact.Album);
    
    public static MusicBrainzIdProperty ReleaseGroup => new(MusicBrainzEntityType.ReleaseGroup,
        EntityType.Unknown, EntityFact.Unknown);
    
    public static MusicBrainzIdProperty Recording => new(MusicBrainzEntityType.Recording,
        EntityType.Track, EntityFact.Track);

    public static TypedEntityListProperty<Guid> ArtistReleases => Ep.Artist.AlbumIds(Release);

    public override Guid Parse(string value) => Guid.Parse(value);
}

public enum MusicBrainzEntityType
{
    Artist,
    Area,
    Event,
    Genre,
    Instrument,
    Label,
    Place,
    Series,
    Recording,
    Release,
    ReleaseGroup,
    Work,
}