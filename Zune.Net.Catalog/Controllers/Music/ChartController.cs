using Atom.Xml;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Zune.Net.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/v3.2/{culture}/music/chart/zune/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ChartController : Controller
    {
        private const bool useDeezer = false;

        [HttpGet, Route("tracks")]
        public async Task<ActionResult<Feed<Track>>> Tracks()
        {
            Feed<Track> feed;

            if (useDeezer)
            {
                var dz_tracks = await Deezer.GetChartDZTracks();
                DateTime updated = DateTime.Now;

                feed = new()
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
                IReleaseGroup mb_release = Deezer.GetMBReleaseGroupByDZAlbum(dz_album);
                if (mb_release == null)
                    continue;

                var album = MusicBrainz.MBReleaseGroupToAlbum(mb_release, updated: updated);
                album.Explicit = dz_album.Value<bool>("explicit_lyrics");

                feed.Entries.Add(album);
            }

            return feed;
        }
    }
}
