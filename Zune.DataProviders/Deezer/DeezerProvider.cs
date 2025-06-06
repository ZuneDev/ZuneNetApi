using System.Collections.Generic;
using System.Threading.Tasks;
using Zune.Xml.Catalog;

namespace Zune.DataProviders.Deezer;

public class DeezerProvider(IAlbumProvider albumProvider, IArtistProvider artistProvider, ITrackProvider trackProvider, IModifiableMediaIdMapper idMapper = null)
    : IAlbumChartProvider, IArtistChartProvider, ITrackChartProvider
{
    public async IAsyncEnumerable<Album> GetAlbumChart()
    {
        var dzAlbums = await Net.Helpers.Deezer.GetChartDZAlbums();
        var albumCount = 0;

        foreach (var dzAlbum in dzAlbums)
        {
            var dzAlbumId = new MediaId(dzAlbum.Value<long>("id").ToString(), KnownMediaSources.Deezer, MediaType.Album);
            var album = await albumProvider.GetAlbum(dzAlbumId);
            if (album is null)
                continue;

            album.Explicit = dzAlbum.Value<bool>("explicit_lyrics");
            
            // TODO: Add previews via rights?

            if (++albumCount > 10)
                yield break;

            yield return album;
        }
    }

    public async IAsyncEnumerable<Artist> GetArtistChart()
    {
        var dzArtists = await Net.Helpers.Deezer.GetChartDZArtists();

        foreach (var dzArtist in dzArtists)
        {
            var dzAlbumId = new MediaId(dzArtist.Value<long>("id").ToString(), KnownMediaSources.Deezer, MediaType.Artist);
            var artist = await artistProvider.GetArtist(dzAlbumId);
            if (artist is null)
                continue;

            yield return artist;
        }
    }

    public async IAsyncEnumerable<Track> GetTrackChart()
    {
        var dzTracks = await Net.Helpers.Deezer.GetChartDZTracks();
        var trackCount = 0;

        foreach (var dzTrack in dzTracks)
        {
            var dzid = dzTrack.Value<long>("id").ToString();
            var dzTrackId = new MediaId(dzid, KnownMediaSources.Deezer, MediaType.Track);
            var track = await trackProvider.GetTrack(dzTrackId);

            if (track is null)
            {
                // No mapping from DZID was found, so fallback to ISRC (MusicBrainz has a lookup)
                var dzTrackFull = await Net.Helpers.Deezer.GetDZTrack(dzid);
                var isrc = dzTrackFull.Value<string>("isrc");
                var mbTrackId = await idMapper.MapTo(new(isrc, KnownMediaSources.ISRC), KnownMediaSources.MusicBrainz);

                if (mbTrackId is null)
                    continue;

                if (idMapper is not null)
                {
                    // Cache DZID <-> MBID mapping
                    await idMapper.RegisterMapping(mbTrackId, dzTrackId);
                }

                track = Net.Helpers.Deezer.DZTrackToTrack(dzTrackFull);
                track.Id = mbTrackId.Id;
            }
            else
            {
                track.Popularity = dzTrack.Value<long>("rank");
                track.Explicit = dzTrack.Value<bool>("explicit_lyrics");
            }

            // TODO: Add previews via rights?

            if (++trackCount > 10)
                yield break;

            yield return track;
        }
    }
}
