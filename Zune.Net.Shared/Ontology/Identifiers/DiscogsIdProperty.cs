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
}