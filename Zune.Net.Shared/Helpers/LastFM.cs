using Atom.Xml;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zune.Xml.Catalog;

namespace Zune.Net.Shared.Helpers
{
    public static partial class LastFM
    {
        public static readonly LastfmClient _client = new(Constants.FM_API_KEY, Constants.FM_API_SECRET);

        public static readonly Author FM_AUTHOR = new()
        {
            Name = "Last.fm",
            Url = "https://last.fm"
        };

        public static async Task<Feed<Track>> GetSimilarTracksByMBID(Guid mbid)
        {
            var response = await _client.Track.GetSimilarByMbidAsync(mbid.ToString());
            var updated = DateTime.Now;

            Feed<Track> feed = new()
            {
                Title = "Similar",
                Author = FM_AUTHOR,
                Id = $"tag:mix.zune.net,2017-03-12:/track/{mbid}/similarTracks",
                Updated = updated,
            };

            foreach (var fmTrack in response)
            {
                if (fmTrack.Mbid == null) continue;

                feed.Entries.Add(FMTrackToTrack(fmTrack, updated));
            }

            return feed;
        }

        public static Track FMTrackToTrack(LastTrack fm_track, DateTime? updated = null, bool includeRights = false)
        {
            updated ??= DateTime.Now;

            MiniArtist trackArtist = new()
            {
                Id = new(fm_track.ArtistMbid),
                Title = fm_track.ArtistName,
            };

            Track track = new()
            {
                Id = fm_track.Mbid.ToString(),
                Title = fm_track.Name,
                PrimaryArtist = trackArtist,
                AlbumArtist = trackArtist,
                Artists = new() { trackArtist },
                Duration = fm_track.Duration ?? TimeSpan.Zero,
                Popularity = fm_track.Rank ?? 0,
                PlayCount = fm_track.PlayCount ?? 0,
                Album = new()
                {
                    Title = fm_track.AlbumName
                },
                Updated = updated.Value,
            };

            if (includeRights)
                MusicBrainz.AddDefaultRights(ref track);

            return track;
        }
    }
}
