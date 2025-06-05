using Atom.Xml;
using Flurl;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zune.DataProviders;

public interface IAlbumImageProvider
{
    IAsyncEnumerable<Url> GetAlbumImages(MediaId id);

}

public interface IArtistImageProvider
{
    IAsyncEnumerable<Url> GetArtistImages(MediaId id);

    Task<Url> GetArtistPrimaryImage(MediaId id);
}

public interface IArtistBiographyProvider
{
    Task<Content> GetArtistBiography(MediaId id);
}
