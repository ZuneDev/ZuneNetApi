using System.Diagnostics.CodeAnalysis;
using Zune.Net.Ontology;
using Zune.Net.Ontology.BaseProperties;
using Zune.Net.Ontology.Identifiers;

namespace Zune.Net.Shared.Tests;

public class Identifiers
{
    [Test]
    [Description("Ensures that ID properties from different providers are never equal.")]
    public void IdPropertiesDifferentProvidersNotEqual()
    {
        Assert.Multiple(() =>
        {
            Assert.That<IEntityProperty>(MusicBrainzIdProperty.Artist, Is.Not.EqualTo(DiscogsIdProperty.Artist));
            Assert.That<IEntityProperty>(DiscogsIdProperty.Artist, Is.Not.EqualTo(MusicBrainzIdProperty.Artist));
            Assert.That<IEntityProperty>(MusicBrainzIdProperty.Release, Is.Not.EqualTo(DiscogsIdProperty.Release));
            Assert.That<IEntityProperty>(DiscogsIdProperty.Artist, Is.Not.EqualTo(TidalIdProperty.Artist));
            Assert.That<IEntityProperty>(SpotifyIdProperty.Artist, Is.Not.EqualTo(AllMusicIdProperty.Artist));
            
            Assert.That<IEntityProperty>(MusicBrainzIdProperty.ReleaseGroup, Is.Not.EqualTo(DiscogsIdProperty.Artist));
            Assert.That<IEntityProperty>(DiscogsIdProperty.Track, Is.Not.EqualTo(MusicBrainzIdProperty.Artist));
            Assert.That<IEntityProperty>(DiscogsIdProperty.Master, Is.Not.EqualTo(TidalIdProperty.Track));
            Assert.That<IEntityProperty>(SpotifyIdProperty.Track, Is.Not.EqualTo(AllMusicIdProperty.Album));
        });
    }
    
    [Test]
    [Description("Ensures that ID properties from the same provider are not equal if they have different entity types")]
    public void IdPropertiesSameProviderDiffEntityTypeNotEqual()
    {
        Assert.Multiple(() =>
        {
            Assert.That<IEntityProperty>(AllMusicIdProperty.Artist, Is.Not.EqualTo(AllMusicIdProperty.Release));
            Assert.That<IEntityProperty>(AppleMusicIdProperty.Artist, Is.Not.EqualTo(AppleMusicIdProperty.Album));
            Assert.That<IEntityProperty>(DeezerIdProperty.Artist, Is.Not.EqualTo(DeezerIdProperty.Album));
            Assert.That<IEntityProperty>(DiscogsIdProperty.Artist, Is.Not.EqualTo(DiscogsIdProperty.Master));
            Assert.That<IEntityProperty>(LastFmIdProperty.Artist, Is.Not.EqualTo(LastFmIdProperty.Album));
            Assert.That<IEntityProperty>(MusicBrainzIdProperty.Artist, Is.Not.EqualTo(MusicBrainzIdProperty.Release));
            Assert.That<IEntityProperty>(SpotifyIdProperty.Artist, Is.Not.EqualTo(SpotifyIdProperty.Album));
            Assert.That<IEntityProperty>(TidalIdProperty.Artist, Is.Not.EqualTo(TidalIdProperty.Album));
        });
    }
    
    [Test]
    [Description("Ensures that ID properties from the same provider are equal if they have the same entity type")]
    [SuppressMessage("Assertion", "NUnit2009:The same value has been provided as both the actual and the expected argument")]
    public void IdPropertiesSameProviderSameEntityTypeEqual()
    {
        Assert.Multiple(() =>
        {
            Assert.That<IEntityProperty>(AllMusicIdProperty.Artist, Is.EqualTo(AllMusicIdProperty.Artist));
            Assert.That<IEntityProperty>(AppleMusicIdProperty.Artist, Is.EqualTo(AppleMusicIdProperty.Artist));
            Assert.That<IEntityProperty>(DeezerIdProperty.Artist, Is.EqualTo(DeezerIdProperty.Artist));
            Assert.That<IEntityProperty>(DiscogsIdProperty.Artist, Is.EqualTo(DiscogsIdProperty.Artist));
            Assert.That<IEntityProperty>(LastFmIdProperty.Artist, Is.EqualTo(LastFmIdProperty.Artist));
            Assert.That<IEntityProperty>(MusicBrainzIdProperty.Artist, Is.EqualTo(MusicBrainzIdProperty.Artist));
            Assert.That<IEntityProperty>(SpotifyIdProperty.Artist, Is.EqualTo(SpotifyIdProperty.Artist));
            Assert.That<IEntityProperty>(TidalIdProperty.Artist, Is.EqualTo(TidalIdProperty.Artist));
        });
    }
}