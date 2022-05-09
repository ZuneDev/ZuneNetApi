using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Zune.Net.Shared.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/v3.2/{culture}/music/chart/zune/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ChartController : Controller
    {
        [HttpGet, Route("tracks")]
        public async Task<ActionResult<Feed<Track>>> Tracks()
        {
            var dz_tracks = await Deezer.GetChartDZTracks();
            DateTime updated = DateTime.Now;

            Feed<Track> feed = new()
            {
                Id = "tracks",
                Title = "Tracks",
                Author = Deezer.DZ_AUTHOR,
                Updated = updated
            };

            foreach (var dz_track in dz_tracks)
            {
                var mb_recording = Deezer.GetMBRecordingByDZTrack(dz_track);
                if (mb_recording == null)
                    continue;

                var track = MusicBrainz.MBRecordingToTrack(mb_recording, updated: updated, includeRights: true);
                track.Popularity = dz_track.Value<long>("rank");
                track.Explicit = dz_track.Value<bool>("explicit_lyrics");

                feed.Entries.Add(track);
            }

            return feed;
        }

        [HttpGet, Route("albums")]
        public async Task<ActionResult<Feed<Album>>> Albums()
        {
            var dz_albums = await Deezer.GetChartDZAlbums();
            DateTime updated = DateTime.Now;

            Feed<Album> feed = new()
            {
                Id = "albums",
                Title = "Albums",
                Author = Deezer.DZ_AUTHOR,
                Updated = updated
            };

            foreach (var dz_album in dz_albums)
            {
                var mb_release = Deezer.GetMBReleaseByDZAlbum(dz_album);
                if (mb_release == null)
                    continue;

                var album = MusicBrainz.MBReleaseToAlbum(mb_release, updated: updated);
                album.Explicit = dz_album.Value<bool>("explicit_lyrics");

                feed.Entries.Add(album);
            }

            return feed;
        }
    }
}
