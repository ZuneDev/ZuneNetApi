using Atom.Xml;
using Flurl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zune.Xml.Catalog;

namespace Zune.DataProviders;

public class AggregatedMediaProvider : IArtistProvider, IArtistBiographyProvider, IArtistImageProvider, IAlbumImageProvider
{
    public List<IMediaProvider> Providers { get; } = [];

    public async Task<Artist> GetArtist(MediaId id)
    {
        return await AggregateAsync<IArtistProvider, Artist>(
            provider => provider.GetArtist(id));
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

    public IAsyncEnumerable<Url> GetAlbumImages(MediaId id)
    {
        return AggregateAsync<IAlbumImageProvider, Url>(
            provider => provider.GetAlbumImages(id));
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
