using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Zune.DataProviders;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/music/chart/zune/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ChartController(AggregatedMediaProvider mediaProvider) : Controller
    {
        [HttpGet, Route("albums")]
        public async Task<ActionResult<Feed<Album>>> Albums()
        {
            return new Feed<Album>()
            {
                Id = "albums",
                Title = "Albums",
                Updated = DateTime.Now,
                Entries = await mediaProvider.GetAlbumChart().ToListAsync()
            };
        }

        [HttpGet, Route("artists")]
        public async Task<ActionResult<Feed<Artist>>> Artists()
        {
            return new Feed<Artist>()
            {
                Id = "artists",
                Title = "Artists",
                Updated = DateTime.Now,
                Entries = await mediaProvider.GetArtistChart().ToListAsync()
            };
        }

        [HttpGet, Route("tracks")]
        public async Task<ActionResult<Feed<Track>>> Tracks()
        {
            var tracks = mediaProvider.GetTrackChart();

            Feed<Track> feed = new()
            {
                Id = "tracks",
                Title = "Tracks",
                Updated = DateTime.Now,
                Entries = await tracks.ToListAsync()
            };

            return feed;
        }
    }
}
