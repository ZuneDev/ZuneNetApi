using System.Diagnostics;
using Zune.Net.Ontology;

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
        var dcid = await _mapper.MapAsync(
            Ep.Artist.MusicBrainzId, mbid,
            Ep.Artist.DiscogsId);

        Assert.That(dcid, Is.EqualTo(61800));
    }

    [Test]
    public async Task GetArtistNameFromMbid()
    {
        var mbid = new Guid("534ee493-bfac-4575-a44a-0ae41e2c3fe4");
        var name = await _mapper.MapAsync(
            Ep.Artist.MusicBrainzId, mbid,
            Ep.Artist.Name);

        Assert.That(name, Is.EqualTo("Rush"));
    }

    [Test]
    public async Task GetArtistNameAndBioFromMbid()
    {
        var mbid = new Guid("534ee493-bfac-4575-a44a-0ae41e2c3fe4");

        Stopwatch stopwatch = new();
        stopwatch.Start();

        var result = await _mapper.MapAsync(
            Ep.Artist.MusicBrainzId, mbid,
            [Ep.Artist.Name, Ep.Artist.Bio]);

        stopwatch.Stop();
        
        await TestContext.Out.WriteLineAsync();
        await TestContext.Out.WriteLineAsync($"Tested edges: {_mapper.DebugInfo.NumEdgesTested}");
        await TestContext.Out.WriteLineAsync($"Executed edges: {_mapper.DebugInfo.NumEdgesExecuted}");
        await TestContext.Out.WriteLineAsync($"Total cost: {_mapper.DebugInfo.TotalCost}");
        await TestContext.Out.WriteLineAsync($"Total time: {stopwatch.Elapsed.TotalSeconds} seconds");
        await TestContext.Out.WriteLineAsync();
        
        var name = result.Get(Ep.Artist.Name);
        var bio = result.Get(Ep.Artist.Bio);
        
        await TestContext.Out.WriteLineAsync($"== {name} ==");
        await TestContext.Out.WriteLineAsync(bio);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(name, Is.EqualTo("Rush"));
            Assert.That(bio, Is.Not.Null);
            
            Assert.That(_mapper.DebugInfo.TotalCost, Is.LessThanOrEqualTo(20));
        }
    }

    [Test]
    public async Task ManuallyGetArtistNameAndBioFromMbid()
    {
        var mbid = new Guid("534ee493-bfac-4575-a44a-0ae41e2c3fe4");

        var idMapper = new BatchIdMapper();

        Stopwatch stopwatch = new();
        stopwatch.Start();

        var idResults = await idMapper.GetArtistIdsByMbidAsync(mbid);
        var dcid = Convert.ToInt32(idResults?.Discogs);
        
        var (dcArtist, _) = await Helpers.Discogs.GetDCArtistByMBID(mbid);
        var name = dcArtist.Value<string>("name");
        var bio = Helpers.Discogs.DCProfileToBiographyContent(dcArtist.Value<string>("profile")).Value;

        stopwatch.Stop();
        
        await TestContext.Out.WriteLineAsync();
        await TestContext.Out.WriteLineAsync($"Total time: {stopwatch.Elapsed.TotalSeconds} seconds");
        await TestContext.Out.WriteLineAsync();
        
        await TestContext.Out.WriteLineAsync($"== {name} ==");
        await TestContext.Out.WriteLineAsync(bio);
        
        using (Assert.EnterMultipleScope())
        {
            Assert.That(dcid, Is.EqualTo(61800));
            Assert.That(name, Is.EqualTo("Rush"));
            Assert.That(bio, Is.Not.Null);
        }
    }

    [Test]
    public async Task MapArtistMbidToDcidUsingWikidata()
    {
        var wikidataIdMapper = new WikidataIdMapper();
        
        var mbid = new Guid("534ee493-bfac-4575-a44a-0ae41e2c3fe4");
        var targetProperty = Ep.Artist.DiscogsId;
        
        var output = await wikidataIdMapper.ExecuteAsync(
            new PropertyBag
            {
                [Ep.Artist.MusicBrainzId] = mbid,
            },
            [targetProperty]);

        Assert.That(output, Is.Not.Null);
        
        var dcid = output.Get(targetProperty);
        Assert.That(dcid, Is.EqualTo(61800));
    }
}