using Atom.Xml;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zune.Net.Catalog.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/v3.2/{culture}/music/artist/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ArtistController : Controller
    {
        [HttpGet, Route("")]
        public ActionResult<Feed<Artist>> Search()
        {
            if (!Request.Query.TryGetValue("q", out var queries) || queries.Count != 1)
                return BadRequest();

            return MusicBrainz.SearchArtists(queries[0], Request.Path);
        }

        [HttpGet, Route("{mbid}")]
        public async Task<ActionResult<Artist>> Details(Guid mbid)
        {
            (var dc_artist, var mb_artist) = await Discogs.GetDCArtistByMBID(mbid);
            Artist artist = MusicBrainz.MBArtistToArtist(mb_artist);
            artist.BackgroundImageId = mbid;
            artist.Images = new() { new() { Id = mbid.ToString() } };
            artist.Biography = dc_artist.Value<string>("profile");
            artist.Links.Add(new(Request.Path.Value + "biography", relation: "zune://artist/biography"));

            return artist;
        }

        [HttpGet, Route("{mbid}/tracks")]
        public ActionResult<Feed<Track>> Tracks(Guid mbid)
        {
            if (!Request.Query.TryGetValue("chunkSize", out var chunkSizeStrs) || chunkSizeStrs.Count != 1)
                return BadRequest();

            return MusicBrainz.GetArtistTracksByMBID(mbid, Request.Path, int.Parse(chunkSizeStrs[0]));
        }

        [HttpGet, Route("{mbid}/albums")]
        public ActionResult<Feed<Album>> Albums(Guid mbid)
        {
            return MusicBrainz.GetArtistAlbumsByMBID(mbid, Request.Path);
        }

        [HttpGet, Route("{mbid}/primaryImage")]
        public async Task<ActionResult> PrimaryImage(Guid mbid)
        {
            (var dc_artist, var mb_artist) = await Discogs.GetDCArtistByMBID(mbid);

            string imgUrl = dc_artist["images"].First(i => i.Value<string>("type") == "primary").Value<string>("uri");
            var imgResponse = await imgUrl.GetAsync();
            if (imgResponse.StatusCode != 200)
                return StatusCode(imgResponse.StatusCode);
            return File(await imgResponse.GetStreamAsync(), "image/jpeg");
        }

        [HttpGet, Route("{mbid}/biography")]
        public async Task<ActionResult<Entry>> Biography(Guid mbid)
        {
            (var dc_artist, var mb_artist) = await Discogs.GetDCArtistByMBID(mbid);
            DateTime updated = DateTime.Now;

            return new Entry
            {
                Id = $"tag:catalog.zune.net,1900-01-01:/music/artist/{mbid}/biography",
                Title = mb_artist.Name,
                Links = { new(Request.Path) },
                Content = Discogs.DCProfileToBiographyContent(dc_artist.Value<string>("profile")),
                Updated = updated,
            };
        }

        [HttpGet, Route("{mbid}/images")]
        public async Task<ActionResult<Feed<Image>>> Images(Guid mbid)
        {
            (var dc_artist, var mb_artist) = await Discogs.GetDCArtistByMBID(mbid);
            DateTime updated = DateTime.Now;
            // NOTE: Yes, SHA1 would be better, but this is not a security-critical
            // application, and MD5 has the benefit of being exactly the same length
            // as a GUID.
            using var md5 = MD5.Create();

            return new Feed<Image>
            {
                Id = $"tag:catalog.zune.net,1900-01-01:/music/artist/{mbid}/images",
                Title = mb_artist.Name,
                Links = { new(Request.Path) },
                Entries = dc_artist.Value<JToken>("images").Select(j =>
                {
                    // Embed key parts of image URL in ID
                    string url = j.Value<string>("uri");
                    Regex rx = new(@"^https?:\/\/i\.discogs\.com\/(.*)\/rs:fit\/g:sm\/q:90\/(.*)\.jpe?g", RegexOptions.IgnoreCase | RegexOptions.Compiled);
                    string imgId = "artist-" + rx.Replace(url, "$1,$2");

                    return new Image
                    {
                        Id = imgId,
                        Instances = new()
                        {
                            new()
                            {
                                Id = imgId,
                                Url = url,
                                Format = "jpg",
                                Width = j.Value<int>("width"),
                                Height = j.Value<int>("height"),
                            }
                        }
                    };
                }).ToList(),
                Updated = updated,
            };
        }
    }
}
