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
        var dcid = await _mapper.MapOneToOneAsync<Guid, int>(EntityType.Artist,
            EntityPropertyType.MusicBrainzArtistId, mbid,
            EntityPropertyType.DiscogsArtistId);

        Assert.That(dcid, Is.EqualTo(61800));
    }

    [Test]
    public async Task MapArtistMbidToDcidDirect()
    {
        var wikidataIdMapper = new WikidataIdMapper();
        
        var mbid = new Guid("534ee493-bfac-4575-a44a-0ae41e2c3fe4");
        var output = await wikidataIdMapper.ExecuteAsync(new PropertyBag
        {
            [new EntityProperty(EntityType.Artist, EntityPropertyType.MusicBrainzArtistId)] = mbid,
        });

        Assert.That(output, Is.Not.Null);
        
        var targetProperty = new EntityProperty(EntityType.Artist, EntityPropertyType.DiscogsArtistId);

        var dcid = Convert.ToInt32(output[targetProperty]);
        Assert.That(dcid, Is.EqualTo(61800));
    }
}