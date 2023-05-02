using System.Collections.Concurrent;
using MetaBrainz.MusicBrainz;
using Zune.DB;
using Zune.Net.MetaServices.DomainModels.MdarCd;
using Zune.Net.MetaServices.DomainModels.MdsrCd;

namespace Zune.Net.Helpers
{
    public class WMIS
    {
        public readonly Query _query;
        private readonly ZuneNetContext _database;
        private readonly ILogger _logger;

        public WMIS(Query query, ZuneNetContext database, ILogger<WMIS> logger)
        {
            _database = database;
            _logger = logger;
            _query = query;
        }

        public async Task<MdsrAlbumRequestMetadata> SearchAlbumsAsync(string query, int limit)
        {
            _logger.LogInformation($"Getting MDSR-CD results for AlbumSearch: {query}");
            var results = await _query.FindReleasesAsync(query, simple: true, limit: 10);
            var releases = results.Results.Select(x => x.Item).ToList();

            var resultList = new ConcurrentBag<MdsrAlbum>();

            await Parallel.ForEachAsync(releases, async (release, ct) =>
            {
                try
                {
                    var albumResult = await GetMdsrAlbumByMbid(release.Id, ct, (int)results.Results.Where(x => x.Item.Id == release.Id).First().Score == 100);
                    if (albumResult != null)
                    {
                        resultList.Add(albumResult);
                        _logger.LogInformation($"Finished building MDSR-CD result for MBID: {release.Id}");
                    }
                    _logger.LogInformation($"No MDSR-CD result for MBID: {release.Id}");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Exception occured while processing request for MBID (album) {release.Id}");
                }
            });

            _logger.LogInformation($"Found {resultList.Count} results");

            // How's that for a stackup?
            return new MdsrAlbumRequestMetadata()
            {
                mDSRcD = new MdsrAlbumSearchResult()
                {
                    Results = resultList.ToList()
                }
            };
        }

        private async Task<MdsrAlbum?> GetMdsrAlbumByMbid(Guid guid, CancellationToken ct, bool bestmatch = false)
        {
            if (ct.IsCancellationRequested)
            {
                return null;
            }

            _logger.LogInformation($"Getting all data for MBID: {guid}");
            var deepRelease = await _query.LookupReleaseAsync(guid, inc: Include.Labels | Include.DiscIds | Include.Recordings | Include.Genres | Include.ArtistCredits | Include.ReleaseGroups);

            if (deepRelease.Title == null)
            {
                return null;
            }

            var genre = "Unknown";
            var performerName = "Unknown Artist";
            var releaseDate = DateTime.Now;
            var albumArtMbid = "default";
            if (deepRelease.Genres?.Count > 0)
            {
                genre = deepRelease.Genres?[0]?.Name;
            }
            if (deepRelease.ArtistCredit?.Count > 0)
            {
                performerName = deepRelease.ArtistCredit?[0]?.Artist.Name;
            }
            if (deepRelease.Date != null && !deepRelease.Date.IsEmpty)
            {

                releaseDate = deepRelease.Date.NearestDate;
            }
            if (deepRelease.CoverArtArchive?.Front ?? false)
            {
                _logger.LogInformation($"Release {guid} HAS ARTWORK");
                albumArtMbid = guid.ToString();
            }

            var recordId = await _database.CreateOrGetAlbumIdInt64Async(guid);

            return new MdsrAlbum()
            {
                Title = deepRelease.Title,
                BestMatch = bestmatch,
                Id = recordId,
                Volume = 1,
                AlbumArtist = performerName,
                BuyNowParms = deepRelease.Id.ToString(),
                ReleaseDate = releaseDate,
                Genre = genre,
                NumberOfTracks = deepRelease.Media != null && deepRelease.Media.Count > 0 ? deepRelease.Media[0].TrackCount : 0,
                IsMultiDisc = deepRelease.Media != null && deepRelease.Media.Count > 0 ? deepRelease.Media.Count > 1 : false,
                CoverParms = albumArtMbid
            };
        }

        public async Task<MdarCdRequestMetadata> GetMdarCdRequestFromInt64(Int64 albumId, int volume)
        {
            var mbid = await _database.GetAlbumIdRecordAsync(albumId);
            if (!mbid.HasValue)
            {
                throw new KeyNotFoundException($"Cannot locate a MBID for {albumId}, please start the FAI request over");
            }
            var deepRelease = await _query.LookupReleaseAsync(mbid.Value, inc: Include.Labels | Include.DiscIds | Include.Recordings | Include.Genres | Include.ArtistCredits | Include.ReleaseGroups);

            var tracks = new List<MdarTrack>();
            if (deepRelease.Media != null && deepRelease.Media.Count > 0)
            {
                foreach (var track in deepRelease.Media[0].Tracks)
                {
                    var trackTitle = track.Title ?? "Unknown Title";
                    var trackNumber = int.Parse(track.Number ?? "0");
                    var trackArtist = track.ArtistCredit?[0]?.Name ?? deepRelease.ArtistCredit?[0]?.Name ?? "Unknown Artist";

                    tracks.Add(new MdarTrack()
                    {
                        Title = trackTitle,
                        Performers = trackArtist,
                        TrackNumber = trackNumber
                    });
                }
            }

            return new MdarCdRequestMetadata()
            {
                MdarCd = new MdarCd()
                {
                    Title = deepRelease.Title,
                    AlbumId = albumId,
                    Volume = volume,
                    Items = tracks,

                }
            };
        }
    }
}