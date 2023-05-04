using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using Zune.DB;
using Zune.Net.MetaServices.DomainModels.MdarCd;
using Zune.Net.MetaServices.DomainModels.MdqCd;
using Zune.Net.MetaServices.DomainModels.MdrCd;
using Zune.Net.MetaServices.DomainModels.MdsrCd;

namespace Zune.Net.Helpers
{
    public partial class WMIS
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
                    var albumResult = await GetMdsrAlbumByMbid(release.Id, ct, results.Results.Where(x => x.Item.Id == release.Id).First().Score == 100);
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
            var release = await _query.LookupReleaseAsync(guid, inc: Include.Labels | Include.DiscIds | Include.Recordings | Include.Genres | Include.ArtistCredits | Include.ReleaseGroups);

            if (release.Title == null)
            {
                return null;
            }

            var genre = "Unknown";
            var performerName = "Unknown Artist";
            var releaseDate = DateTime.Now;
            var albumArtMbid = "default";
            if (release.Genres?.Count > 0)
            {
                genre = release.Genres?[0]?.Name;
            }
            if (release.ArtistCredit?.Count > 0)
            {
                performerName = release.ArtistCredit?[0]?.Artist.Name;
            }
            if (release.Date != null && !release.Date.IsEmpty)
            {

                releaseDate = release.Date.NearestDate;
            }
            if (release.CoverArtArchive?.Front ?? false)
            {
                _logger.LogInformation($"Release {guid} HAS ARTWORK");
                albumArtMbid = guid.ToString();
            }

            var recordId = await _database.CreateOrGetAlbumIdInt64Async(guid);

            var numberOfTracks = 0;
            var isMultiDisc = false;
            if (release.Media != null && release.Media.Count > 0)
            {
                numberOfTracks = release.Media[0].TrackCount;
                isMultiDisc = release.Media.Count > 1;

                await Parallel.ForEachAsync(release.Media[0].Tracks, async (track, ct) =>
                {
                    // Add the tracks here
                });
            }

            return new MdsrAlbum()
            {
                Title = release.Title,
                BestMatch = bestmatch,
                Id = recordId,
                Volume = 1,
                AlbumArtist = performerName,
                BuyNowParms = release.Id.ToString(),
                ReleaseDate = releaseDate,
                Genre = genre,
                NumberOfTracks = numberOfTracks,
                IsMultiDisc = isMultiDisc,
                CoverParms = albumArtMbid
            };
        }

        private async Task<List<MdarTrack>> GetTracksFromIReleaseAsync(IRelease release, MdqRequestMetadata requestMetadata = null)
        {
            var tracks = new List<MdarTrack>();
            if (release.Media != null && release.Media.Count > 0)
            {
                foreach (var track in release.Media[0].Tracks)
                {
                    var trackTitle = track.Title ?? "Unknown Title";
                    var trackNumber = int.Parse(track.Number ?? "0");
                    var trackArtist = track.ArtistCredit?[0]?.Name ?? release.ArtistCredit?[0]?.Name ?? "Unknown Artist";
                    var trackMbid = track.Id;

                    if (requestMetadata != null)
                    {
                        // attempt to bind a trackid to a trackmbid
                        var thisTrackIntId = requestMetadata.MdqCd.Tracks.Where(x => x.TrackNumber == trackNumber).ToArray();

                        if (thisTrackIntId != null && thisTrackIntId.Any())
                        {
                            await _database.CreateTrackReverseLookupRecordAsync(release.Id, trackMbid, thisTrackIntId[0].trackRequestId, thisTrackIntId[0].TrackDurationMs);
                        }
                    }

                    tracks.Add(new MdarTrack()
                    {
                        Title = trackTitle,
                        Performers = trackArtist,
                        TrackNumber = trackNumber,
                        TrackWmid = trackMbid
                    });
                }
            }
            return tracks;
        }

        public async Task<MdarCdRequestMetadata> GetMdarCdRequestFromInt64(long albumId, string volume, MdqRequestMetadata requestMetadata)
        {
            var mbid = await _database.GetAlbumIdRecordAsync(albumId);
            if (!mbid.HasValue)
            {
                throw new KeyNotFoundException($"Cannot locate a MBID for {albumId}, please start the FAI request over");
            }
            var release = await _query.LookupReleaseAsync(mbid.Value, inc: Include.Labels | Include.DiscIds | Include.Recordings | Include.Genres | Include.ArtistCredits | Include.ReleaseGroups);

            var tracks = await GetTracksFromIReleaseAsync(release, requestMetadata);

            var intVolume = 1;
            if (int.TryParse(GetFirstInt().Match(volume).Value, out var tryVolume))
            {
                intVolume = tryVolume;
            }

            return new MdarCdRequestMetadata()
            {
                mdqRequestID = release.Id,
                MdarCd = new MdarCd()
                {
                    Title = release.Title,
                    AlbumId = albumId,
                    Volume = intVolume,
                    Items = tracks,
                    AlbumGroupMBID = release.Id,
                    AlbumMBID = release.Id
                }
            };
        }

        public async Task<MdrRequestMetadata> GetMdrCdRequestFromTrackIdAsync(int TrackRequestID, int trackDuration, Guid requestId)
        {
            var albumMbid = await _database.GetAlbumMbidFromTrackIdAndDurationAsync(TrackRequestID, trackDuration);
            var trackMbid = await _database.GetTrackMbidFromTrackIdAndDurationAsync(TrackRequestID, trackDuration);

            var release = await _query.LookupReleaseAsync(albumMbid.Value, inc: Include.Labels | Include.DiscIds | Include.Recordings | Include.Genres | Include.ArtistCredits | Include.ReleaseGroups);
            var track = release.Media[0].Tracks.Where(x => x.Id == trackMbid).ToList()[0];

            var performerName = string.Empty;
            if (release.ArtistCredit?.Count > 0)
            {
                performerName = release.ArtistCredit?[0]?.Artist.Name;
            }

            var trackPerformer = string.Empty;
            if (track.ArtistCredit?.Count > 0)
            {
                trackPerformer = track.ArtistCredit?[0]?.Artist.Name;
            }

            var period = string.Empty;

            if (release.Date != null && !release.Date.IsEmpty)
            {
                var yearString = release.Date.Year.Value.ToString();
                var decadeString = yearString[..^1];
                period = $"{decadeString}0's";
            }

            var genre = string.Empty;
            if (release.Genres?.Count > 0)
            {
                genre = release.Genres?[0]?.Name;
            }

            return new MdrRequestMetadata()
            {
                MdqRequestID = requestId,
                Backoff = new Backoff()
                {
                    Time = 0,
                },
                MDRCD = new MDRCD()
                {
                    MdqRequestID = requestId,
                    WMCollectionGroupID = release.Id,
                    WMCollectionID = release.Id,
                    Track = new List<Track>(){new Track()
                    {
                        UniqueFileID = trackMbid.Value.ToString("N"),
                        TrackRequestID = TrackRequestID,
                        WMContentID = trackMbid.Value.ToString(),
                        TrackTitle = track.Title,
                        TrackNumber = track.Number,
                        TrackPerformer = trackPerformer,
                        Period = period
                    }},
                    UniqueFileID = albumMbid.Value.ToString("N"),
                    Version = 1,
                    AlbumTitle = release.Title,
                    AlbumArtist = performerName,
                    ReleaseDate = release.Date.NearestDate,
                    Genre = genre,
                    LargeCoverAddress = release.Id.ToString(),
                    SmallCoverAddress = release.Id.ToString()
                }
            };
        }

        [GeneratedRegex("\\d+")]
        private static partial Regex GetFirstInt();
    }
}