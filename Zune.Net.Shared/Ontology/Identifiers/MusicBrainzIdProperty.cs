using System;
using Zune.Net.Ontology.BaseProperties;

namespace Zune.Net.Ontology.Identifiers;

public class MusicBrainzIdProperty(MusicBrainzEntityType providerEntityType, EntityType entityType)
    : TypedEntityIdProperty<Guid, MusicBrainzEntityType>(providerEntityType, entityType)
{
    public static MusicBrainzIdProperty Artist => new(MusicBrainzEntityType.Artist, EntityType.Artist);
    
    public static MusicBrainzIdProperty Release => new(MusicBrainzEntityType.Release, EntityType.Album);
    public static MusicBrainzIdProperty ReleaseGroup => new(MusicBrainzEntityType.ReleaseGroup, EntityType.Unknown);
    public static MusicBrainzIdProperty Recording => new(MusicBrainzEntityType.Recording, EntityType.Track);

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