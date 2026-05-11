using Zune.Net.Identifiers;

namespace Zune.Net.Shared.Tests;

public class Tests
{
    private CompositePropertyMapper _mapper;
    
    [SetUp]
    public void Setup()
    {
        var registry = PropertyMapperRegistry.CreateDefault();
        _mapper = new(registry);
    }

    [Test]
    public async Task MapArtistMbidToDcid()
    {
        var mbid = new Guid("534ee493-bfac-4575-a44a-0ae41e2c3fe4");
        var result = await _mapper.MapOneToOneAsync(EntityType.Artist,
            EntityPropertyType.MusicBrainzArtistId, mbid,
            EntityPropertyType.DiscogsArtistId);

        var dcid = Convert.ToInt32(result);
        Assert.That(dcid, Is.EqualTo(61800));
    }

    [Test]
    public async Task GetArtistNameFromMbid()
    {
        var mbid = new Guid("534ee493-bfac-4575-a44a-0ae41e2c3fe4");
        var result = await _mapper.MapOneToOneAsync(EntityType.Artist,
            EntityPropertyType.MusicBrainzArtistId, mbid,
            EntityPropertyType.ArtistName);

        var name = result?.ToString();
        Assert.That(name, Is.EqualTo("Rush"));
    }

    [Test]
    public async Task GetArtistNameAndBioFromMbid()
    {
        var mbid = new Guid("534ee493-bfac-4575-a44a-0ae41e2c3fe4");
        
        var nameProperty = new EntityProperty(EntityType.Artist, EntityPropertyType.ArtistName);
        var bioProperty = new EntityProperty(EntityType.Artist, EntityPropertyType.ArtistBio);

        var result = await _mapper.MapOneToManyAsync(EntityType.Artist,
            EntityPropertyType.MusicBrainzArtistId, mbid,
            [nameProperty, bioProperty]);

        var name = result[nameProperty]?.ToString();
        var bio = result[bioProperty]?.ToString();
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(name, Is.EqualTo("Rush"));
            Assert.That(bio, Is.Not.Null);
            
            Assert.That(_mapper.TotalCost, Is.LessThanOrEqualTo(20));
        }
    }

    [Test]
    public async Task MapArtistMbidToDcidUsingWikidata()
    {
        var wikidataIdMapper = new WikidataIdMapper();
        
        var mbid = new Guid("534ee493-bfac-4575-a44a-0ae41e2c3fe4");
        var targetProperty = new EntityProperty(EntityType.Artist, EntityPropertyType.DiscogsArtistId);
        
        var output = await wikidataIdMapper.ExecuteAsync(
            new PropertyBag
            {
                [new EntityProperty(EntityType.Artist, EntityPropertyType.MusicBrainzArtistId)] = mbid,
            },
            [targetProperty]);

        Assert.That(output, Is.Not.Null);
        

        var dcid = Convert.ToInt32(output[targetProperty]);
        Assert.That(dcid, Is.EqualTo(61800));
    }
}