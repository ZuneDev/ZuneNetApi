using Atom.Xml;
using Flurl;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zune.Xml.Catalog;

namespace Zune.DataProviders;

public interface IMediaProvider;

public interface IAlbumProvider : IMediaProvider
{
    Task<Artist> GetAlbum(MediaId id);
}

public interface IAlbumImageProvider : IMediaProvider
{
    IAsyncEnumerable<Url> GetAlbumImages(MediaId id);
}

public interface IArtistProvider : IMediaProvider
{
    Task<Artist> GetArtist(MediaId id);
}

public interface IArtistImageProvider : IMediaProvider
{
    IAsyncEnumerable<Url> GetArtistImages(MediaId id);

    Task<Url> GetArtistPrimaryImage(MediaId id);
}

public interface IArtistBiographyProvider : IMediaProvider
{
    Task<Content> GetArtistBiography(MediaId id);
}
