using MetaBrainz.MusicBrainz;
using Zune.Net.MetaServices.DomainModels.MDSR;

namespace Zune.Net.Helpers
{
    public static class WMIS
    {
        public static readonly Query _query = new("Zune", "4.8", "https://github.com/xerootg/ZuneNetApi");
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

        public static async Task<MDSRCDMetadata> SearchAlbums(string query)
        {
            var results = await _query.FindReleasesAsync(query, simple: true, limit: 10);
            var releases = results.Results.Select(x => x.Item).ToList();
            var resultList = new List<Result>();
            foreach (var release in releases)
            {
                var deepRelease = await _query.LookupReleaseAsync(release.Id, inc: Include.Labels | Include.DiscIds | Include.Recordings | Include.Genres | Include.ArtistCredits | Include.ReleaseGroups);

                var label = string.Empty;
                var genre = string.Empty;
                var performerName = string.Empty;
                var artistGuid = Guid.Empty;
                var releaseDate = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}";
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

                var result = new Result()
                {
                    bestmatch = (int)results.Results.Where(x => x.Item.Id == deepRelease.Id).First().Score == 100,
                    album_id = int.Parse(release.Id.ToString("N")[0..6], System.Globalization.NumberStyles.HexNumber),
                    Volume = 1,
                    albumPerformer = performerName,
                    buyNowLink = "http://google.com",
                    albumReleaseDate = releaseDate,
                    albumGenre = genre,
                    numberOfTracks = deepRelease.Media != null && deepRelease.Media.Count > 0 ? deepRelease.Media[0].TrackCount : 0,
                    IsMultiDisk = deepRelease.Media != null && deepRelease.Media.Count > 0 ? deepRelease.Media.Count > 1 : false,
                    albumCover = release.Id.ToString()
                };
                resultList.Add(result);
            }

            // How's that for a stackup?
            var ret = new MDSRCDMetadata()
            {
                mDSRcD = new MDSRCD()
                {
                    SearchResult = new SearchResult()
                    {
                        Results = resultList
                    }
                }
            };

            return ret;
        }
    }
}