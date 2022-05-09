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

        public static async Task<Feed<Artist>> GetSimilarArtistsByMBID(Guid mbid)
        {
            var response = await _client.Artist.GetSimilarByMbidAsync(mbid.ToString());
            var updated = DateTime.Now;

            Feed<Artist> feed = new()
            {
                Title = "Similar",
                Author = FM_AUTHOR,
                Id = $"tag:catalog.zune.net,2017-03-12:/music/artist/{mbid}/similarArtists",
                Updated = updated,
            };

            foreach (var fmArtist in response)
            {
                if (fmArtist.Mbid == null) continue;

                feed.Entries.Add(FMArtistToArtist(fmArtist, updated));
            }

            return feed;
        }

        public static Track FMTrackToTrack(LastTrack fm_track, DateTime? updated = null, bool includeRights = true)
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

        public static Artist FMArtistToArtist(LastArtist fm_artist, DateTime? updated = null)
        {
            updated ??= DateTime.Now;

            Artist artist = new()
            {
                Id = fm_artist.Mbid,
                Title = fm_artist.Name,
                ShortBiography = fm_artist.Bio?.Summary,
                Biography = fm_artist.Bio?.Content,
                BiographyLink = fm_artist.Url?.ToString(),
                PlayCount = fm_artist.PlayCount ?? 0,
                Updated = updated.Value,
            };

            return artist;
        }

        public static Album FMAlbumToAlbum(LastAlbum fm_album, DateTime? updated = null, bool includeRights = true)
        {
            updated ??= DateTime.Now;

            MiniArtist albumArtist = new()
            {
                Id = new(fm_album.ArtistMbid),
                Title = fm_album.ArtistName,
            };

            Album album = new()
            {
                Id = fm_album.Mbid.ToString(),
                Title = fm_album.Name,
                PrimaryArtist = albumArtist,
                Artists = new() { albumArtist },
                Images = new()
                {
                    new() { Id = fm_album.Mbid }
                },
                Popularity = fm_album.PlayCount ?? 0,
                Summary = fm_album.Wiki?.Summary,
                Updated = updated.Value,
            };

            if (fm_album.ReleaseDateUtc.HasValue)
                album.ReleaseDate = fm_album.ReleaseDateUtc.Value.DateTime;

            if (includeRights)
                MusicBrainz.AddDefaultRights(ref album);

            return album;
        }
    }
}
