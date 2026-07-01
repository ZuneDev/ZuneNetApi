using System;
using Zune.Net.Ontology.BaseProperties;

namespace Zune.Net.Ontology.Identifiers;

public class DiscogsIdProperty(DiscogsEntityType providerEntityType, EntityType entityType)
    : TypedEntityIdProperty<ulong, DiscogsEntityType>(providerEntityType, entityType)
{
    public static DiscogsIdProperty Artist => new(DiscogsEntityType.Artist, EntityType.Artist);
    public static DiscogsIdProperty Release => new(DiscogsEntityType.Release, EntityType.Album);
    public static DiscogsIdProperty Master => new(DiscogsEntityType.Master, EntityType.Album);
    public static DiscogsIdProperty Track => new(DiscogsEntityType.Track, EntityType.Track);

    public override ulong Parse(string value) => ulong.Parse(value);
}

public class DiscogsImageIdProperty(DiscogsEntityType providerEntityType, EntityType entityType)
    : TypedEntityIdProperty<Uri, DiscogsEntityType>(providerEntityType, entityType)
{
    public static DiscogsImageIdProperty Image => new(DiscogsEntityType.Image, EntityType.Image);
    public static DiscogsImageIdProperty Release => new(DiscogsEntityType.Release, EntityType.Album);

    public override Uri Parse(string value) => new(value);
}

public enum DiscogsEntityType
{
    Artist,
    Release,
    Label,
    Master,
    Track,
    Composition,
    Style,
    Genre,
    Image,
    ImageInstance
}