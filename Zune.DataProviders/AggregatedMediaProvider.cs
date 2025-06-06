using Atom.Xml;
using Flurl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zune.Xml.Catalog;

namespace Zune.DataProviders;

public class AggregatedMediaProvider : IMediaProvider,
    IArtistProvider, IArtistChartProvider, IArtistBiographyProvider, IArtistImageProvider,
    IAlbumProvider, IAlbumChartProvider, IAlbumImageProvider,
    ITrackProvider, ITrackChartProvider
{
    public List<IMediaProvider> Providers { get; } = [];

    public async Task<Artist> GetArtist(MediaId id)
    {
        return await AggregateAsync<IArtistProvider, Artist>(
            provider => provider.GetArtist(id));
    }

    public IAsyncEnumerable<Artist> GetArtistChart()
    {
        return AggregateAsync<IArtistChartProvider, Artist>(
            provider => provider.GetArtistChart());
    }

    public async Task<Content> GetArtistBiography(MediaId id)
    {
        return await AggregateAsync<IArtistBiographyProvider, Content>(
            provider => provider.GetArtistBiography(id));
    }

    public IAsyncEnumerable<Url> GetArtistImages(MediaId id)
    {
        return AggregateAsync<IArtistImageProvider, Url>(
            provider => provider.GetArtistImages(id));
    }

    public async Task<Url> GetArtistPrimaryImage(MediaId id)
    {
        return await AggregateAsync<IArtistImageProvider, Url>(
            provider => provider.GetArtistPrimaryImage(id));
    }

    public async Task<Album> GetAlbum(MediaId id)
    {
        return await AggregateAsync<IAlbumProvider, Album>(
            provider => provider.GetAlbum(id));
    }

    public IAsyncEnumerable<Url> GetAlbumImages(MediaId id)
    {
        return AggregateAsync<IAlbumImageProvider, Url>(
            provider => provider.GetAlbumImages(id));
    }

    public IAsyncEnumerable<Album> GetAlbumChart()
    {
        return AggregateAsync<IAlbumChartProvider, Album>(
            provider => provider.GetAlbumChart());
    }

    public async Task<Track> GetTrack(MediaId id)
    {
        return await AggregateAsync<ITrackProvider, Track>(
            provider => provider.GetTrack(id));
    }

    public IAsyncEnumerable<Track> GetTrackChart()
    {
        return AggregateAsync<ITrackChartProvider, Track>(
            provider => provider.GetTrackChart());
    }

    private async Task<TResult> AggregateAsync<TProvider, TResult>(Func<TProvider, Task<TResult>> func)
    {
        foreach (var provider in Providers.OfType<TProvider>())
        {
            var content = await func(provider);
            if (content is not null)
                return content;
        }

        return default;
    }

    private async IAsyncEnumerable<TResult> AggregateAsync<TProvider, TResult>(Func<TProvider, IAsyncEnumerable<TResult>> func)
    {
        foreach (var provider in Providers.OfType<TProvider>())
            await foreach (var result in func(provider))
                yield return result;
    }
}
