using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Zune.Net.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/music/chart/zune/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ChartController : Controller
    {
        private const bool useDeezer = true;

        [HttpGet, Route("tracks")]
        public async Task<ActionResult<Feed<Track>>> Tracks()
        {
            Feed<Track> feed;

            if (useDeezer)
            {
                var dz_tracks = await Deezer.GetChartDZTracks();

                ConcurrentBag<Track> tracks = [];

                Parallel.ForEach(dz_tracks, dz_track =>
                {
                    var mb_recording = Deezer.GetMBRecordingByDZTrack(dz_track);
                    if (mb_recording == null)
                        return;

                    var track = MusicBrainz.MBRecordingToTrack(mb_recording, updated: DateTime.Now, includeRights: true);
                    track.Popularity = dz_track.Value<long>("rank");
                    track.Explicit = dz_track.Value<bool>("explicit_lyrics");

                    tracks.Add(track);
                });

                feed = new()
                {
                    Id = "tracks",
                    Title = "Tracks",
                    Author = Deezer.DZ_AUTHOR,
                    Updated = DateTime.Now,
                    Entries = tracks.OrderByDescending(t => t.Popularity).ToList()
                };
            }
            else
            {
                var fm_tracks = await LastFM.GetTopTracks();
                feed = LastFM.CreateFeed<Track>("/music/chart/zune/tracks", "Top tracks");

                foreach (var fm_track in fm_tracks.Take(10))
                {
                    var mb_recording = LastFM.GetMBRecordingByFMTrack(fm_track);
                    if (mb_recording == null)
                        continue;

                    var track = MusicBrainz.MBRecordingToTrack(mb_recording, updated: feed.Updated, includeRights: true);
                    track.Popularity = fm_track.Rank ?? 0;
                    track.PlayCount = fm_track.PlayCount ?? 0;

                    feed.Entries.Add(track);
                }
            }

            return feed;
        }

        [HttpGet, Route("albums")]
        public async Task<ActionResult<Feed<Album>>> Albums()
        {
            var dz_albums = (await Deezer.GetChartDZAlbums()).ToList();

            ConcurrentBag<Album> albums = [];

            await Parallel.ForEachAsync(dz_albums, async (dz_album, token) =>
            {
                var mb_release = await Deezer.GetMBReleaseByDZAlbumAsync(dz_album);
                if (mb_release == null)
                    return;

                var album = MusicBrainz.MBReleaseToAlbum(mb_release, updated: DateTime.Now);
                album.Explicit = dz_album.Value<bool>("explicit_lyrics");
                album.Popularity = dz_albums.Count - dz_album.Value<int>("position");

                albums.Add(album);
            });

            Feed<Album> feed = new()
            {
                Id = "albums",
                Title = "Albums",
                Author = Deezer.DZ_AUTHOR,
                Updated = DateTime.Now,
                Entries = albums.OrderByDescending(t => t.Popularity).ToList()
            };

            return feed;
        }
    }
}
