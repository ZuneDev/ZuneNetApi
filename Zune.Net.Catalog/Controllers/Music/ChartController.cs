using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using MetaBrainz.ListenBrainz;
using MetaBrainz.ListenBrainz.Interfaces;
using Zune.Net.Features;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music
{
    [Route("/music/chart/zune/")]
    [Produces(Atom.Constants.ATOM_MIMETYPE)]
    public class ChartController(ListenBrainz listenBrainz) : Controller
    {
        private static readonly Author LB_AUTHOR = new()
        {
            Name = "ListenBrainz",
            Url = "https://www.listenbrainz.org"
        };
        
        [HttpGet, Route("tracks")]
        public async Task<ActionResult<Feed<Track>>> Tracks([FromQuery] int? chunkSize = null)
        {
            var siteStats = await listenBrainz.GetRecordingStatisticsAsync();
            Guard.IsNotNull(siteStats);

            Feed<Track> feed = new()
            {
                Id = "tracks",
                Title = "Tracks",
                Author = LB_AUTHOR,
                Updated = siteStats.LastUpdated.DateTime,
            };

            var lb_recordings = siteStats.Recordings ?? Enumerable.Empty<IRecordingInfo>();
            if (chunkSize is not null)
                lb_recordings = lb_recordings.Take(chunkSize.Value);
            
            foreach (var lb_recording in lb_recordings)
            {
                Track track = new()
                {
                    Id = lb_recording.Id.ToString(),
                    Title = lb_recording.Name,
                    Album = new MiniAlbum
                    {
                        Id = lb_recording.ReleaseId!.Value,
                        Title = lb_recording.ReleaseName,
                    },
                    PrimaryArtist = new MiniArtist
                    {
                        Title = lb_recording.ArtistName
                    }
                };

                if (lb_recording.ArtistIds is { Count: > 0 })
                {
                    track.PrimaryArtist.Id = lb_recording.ArtistIds[0];
                    track.Artists ??= [];
                    
                    foreach (var artistMbid in lb_recording.ArtistIds)
                    {
                        track.Artists.Add(new MiniArtist
                        {
                            Id = artistMbid,
                        });
                    }
                }
                
                feed.Entries.Add(track);
            }

            return feed;
        }

        [HttpGet, Route("albums")]
        public async Task<ActionResult<Feed<Album>>> Albums([FromQuery] int? chunkSize = null)
        {
            var cultureFeature = HttpContext.Features.Get<ICultureFeature>();
            var culture = cultureFeature?.CultureString ?? "en-US";
        
            var apiVersionFeature = HttpContext.Features.Get<IApiVersionFeature>();
            var apiVersion = apiVersionFeature?.Version ?? "3.2";
            
            var siteStats = await listenBrainz.GetReleaseStatisticsAsync();
            Guard.IsNotNull(siteStats);

            Feed<Album> feed = new()
            {
                Id = "albums",
                Title = "Albums",
                Author = LB_AUTHOR,
                Updated = siteStats.LastUpdated.DateTime,
            };

            var lb_releases = siteStats.Releases ?? Enumerable.Empty<IReleaseInfo>();
            if (chunkSize is not null)
                lb_releases = lb_releases.Take(chunkSize.Value);
            
            foreach (var lb_release in lb_releases)
            {
                Album album = new()
                {
                    Id = lb_release.Id.ToString(),
                    Title = lb_release.Name,
                    PrimaryArtist = new MiniArtist
                    {
                        Title = lb_release.ArtistName
                    }
                };

                if (lb_release.ArtistIds is { Count: > 0 })
                {
                    album.PrimaryArtist.Id = lb_release.ArtistIds[0];
                    album.Artists ??= [];
                    
                    foreach (var artistMbid in lb_release.ArtistIds)
                    {
                        album.Artists.Add(new MiniArtist
                        {
                            Id = artistMbid,
                        });
                    }
                }

                if (lb_release.CoverArtReleaseId is not null)
                {
                    var imageId = lb_release.CoverArtReleaseId.Value;
                    
                    album.Images =
                    [
                        new Image
                        {
                            Id = imageId,
                            Instances =
                            [
                                new ImageInstance
                                {
                                    Id = imageId,
                                    Url = $"http://image.catalog.zunes.me/v{apiVersion}/{culture}/image/{imageId}"
                                }
                            ]
                        }
                    ];
                }
                
                feed.Entries.Add(album);
            }

            return feed;
        }
    }
}
