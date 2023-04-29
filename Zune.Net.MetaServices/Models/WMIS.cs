using System.Collections.Concurrent;
using MetaBrainz.MusicBrainz;
using Zune.DB;
using Zune.Net.MetaServices.DomainModels.MDSR;

namespace Zune.Net.Helpers
{
    public class WMIS
    {
        public static readonly Query Query = new("Zune", "4.8", "https://github.com/xerootg/ZuneNetApi");
        // public static async Task<List<Album>> SearchForAlbums(string query)
        // {
        //     var results = await _query.FindReleasesAsync(query, simple: true, limit: 1);
        //     var releases = results.Results.Select(x => x.Item).ToList();
        //     var ret = new List<Album>();
        //     foreach (var album in releases)
        //     {
        //         var deepRelease = await _query.LookupReleaseAsync(album.Id, inc: Include.Labels | Include.DiscIds | Include.Recordings | Include.Genres | Include.ArtistCredits | Include.ReleaseGroups);
        //         var genre = "Unknown";
        //         if (deepRelease.Genres?.Count > 0)
        //         {
        //             genre = deepRelease.Genres[0].Name;
        //         }
        //         var releaseDate = DateTime.Now;
        //         if (deepRelease.Date != null)
        //         {
        //             releaseDate = new DateTime(deepRelease.Date.Year ?? DateTime.Now.Year, deepRelease.Date.Month ?? DateTime.Now.Month, deepRelease.Date.Day ?? DateTime.Now.Day);
        //         }
        //         try
        //         {
        //             ret.Add(new Album()
        //             {
        //                 Title = deepRelease.Title,
        //                 Id = 1,
        //                 AlbumArtist = deepRelease.ArtistCredit[0].Name,
        //                 Genre = genre,
        //                 Volume = 1,
        //                 ReleaseDate = releaseDate,
        //                 NumberOfTracks = deepRelease.Media[0].TrackCount,
        //                 BestMatch = (int)results.Results.Where(x => x.Item.Id == album.Id).First().Score == 100,
        //                 IsMultiDisk = deepRelease.Media.Count > 1,
        //                 CoverParams = string.Empty,
        //                 BuyNowParams = string.Empty
        //             });
        //         }
        //         catch { }
        //     }
        //     return ret;
        // }
        private readonly ZuneNetContext _database;
        private readonly ILogger _logger;


        public WMIS(ZuneNetContext database, ILogger<WMIS> logger)
        {
            _database = database;
            _logger = logger;
        }

        public async Task<MDSRCDMetadata> SearchAlbums(string query)
        {
            _logger.LogInformation($"Getting MDSR-CD results for AlbumSearch: {query}");
            var results = await Query.FindReleasesAsync(query, simple: true);
            var releases = results.Results.Select(x => x.Item).ToList();

            var resultList = new ConcurrentBag<Result>();

            await Parallel.ForEachAsync(releases, async (release, ct) =>
            {
                if(ct.IsCancellationRequested)
                {
                    return;
                }

                _logger.LogInformation($"Getting all data for MBID: {release.Id}");
                var deepRelease = await Query.LookupReleaseAsync(release.Id, inc: Include.Labels | Include.DiscIds | Include.Recordings | Include.Genres | Include.ArtistCredits | Include.ReleaseGroups);

                var label = string.Empty;
                var genre = "Unknown";
                var performerName = "Unknown Artist";
                var artistGuid = Guid.Empty;
                var releaseDate = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}";
                var albumArtMbid = "default";
                if (deepRelease.Genres?.Count > 0)
                {
                    genre = deepRelease.Genres[0].Name;
                }
                if (deepRelease.LabelInfo?.Count > 0)
                {
                    label = deepRelease.LabelInfo[0].Label?.Name ?? deepRelease.LabelInfo[0].CatalogNumber;
                }
                if (deepRelease.ArtistCredit?.Count > 0)
                {
                    performerName = deepRelease.ArtistCredit?[0].Artist.Name;
                    artistGuid = deepRelease.ArtistCredit[0].Artist.Id;
                }
                if (deepRelease.Date != null)
                {
                    
                    releaseDate = $"{deepRelease.Date.Year ?? DateTime.Now.Year}-{deepRelease.Date.Month ?? DateTime.Now.Month}-{deepRelease.Date.Day ?? DateTime.Now.Day}";
                }
                if(deepRelease.CoverArtArchive?.Front ?? false)
                {
                    _logger.LogInformation($"Release {release.Id} HAS ARTWORK");
                    albumArtMbid = release.Id.ToString();
                }

                //var id = _database.AddOrGetAlbumLookupRecordAsync(release.Id);

                resultList.Add(new Result()
                {
                    bestmatch = (int)results.Results.Where(x => x.Item.Id == deepRelease.Id).First().Score == 100,
                    album_id = 123456789,//int.Parse(release.Id.ToString("N")[0..6], System.Globalization.NumberStyles.HexNumber),
                    Volume = 1,
                    albumPerformer = performerName,
                    buyNowLink = "http://google.com",
                    albumReleaseDate = releaseDate,
                    albumGenre = genre,
                    numberOfTracks = deepRelease.Media != null && deepRelease.Media.Count > 0 ? deepRelease.Media[0].TrackCount : 0,
                    IsMultiDisk = deepRelease.Media != null && deepRelease.Media.Count > 0 ? deepRelease.Media.Count > 1 : false,
                    albumCover = albumArtMbid
                });
                _logger.LogInformation($"Finished building MDSR-CD result for MBID: {release.Id}");
            });

            _logger.LogInformation($"Found {resultList.Count} results");

            // How's that for a stackup?
            var ret = new MDSRCDMetadata()
            {
                mDSRcD = new MDSRCD()
                {
                    SearchResult = new SearchResult()
                    {
                        Results = resultList.ToList()
                    }
                }
            };

            return ret;
        }
    }
}