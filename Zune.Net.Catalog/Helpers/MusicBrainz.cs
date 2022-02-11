using Atom.Xml;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Helpers
{
    public class MusicBrainz
    {
        private static readonly Guid ARTIST_VARIOUSARTISTS = new("89ad4ac3-39f7-470e-963a-56509c546377");
        private static readonly Guid ARTIST_NOARTIST = new("eec63d3c-3b81-4ad4-b1e4-7c147d4d2b61");

        private readonly Query _query = new("Zune", "4.8", "https://github.com/ZuneDev/ZuneNetApi");

        public Feed<Track> SearchTracks(string query, string requestPath)
        {
            var results = _query.FindAllRecordings(query, simple: true);
            var updated = DateTime.Now;
            Feed<Track> feed = new()
            {
                Id = "tracks",
                Title = "tracks",
                Links = { new(requestPath) },
                Updated = updated,
                Entries = new()
            };

            // Add results to feed
            foreach (var result in results)
            {
                if (feed.Entries.Count == 40) break;

                var mb_rec = result.Item;
                feed.Entries.Add(MBRecordingToTrack(mb_rec, updated: updated));
            }

            return feed;
        }

        public Feed<Artist> SearchArtists(string query, string requestPath)
        {
            var results = _query.FindAllArtists(query, simple: true);
            var updated = DateTime.Now;
            Feed<Artist> feed = new()
            {
                Id = "artists",
                Title = "artists",
                Links = { new(requestPath) },
                Updated = updated,
                Entries = new()
            };

            // Add results to feed
            foreach (var result in results)
            {
                if (feed.Entries.Count == 40) break;

                var mb_rec = result.Item;
                feed.Entries.Add(MBArtistToArtist(mb_rec, updated: updated));
            }

            return feed;
        }

        public Track GetTrackByMBID(Guid mbid)
        {
            var mb_rec = _query.LookupRecording(mbid, Include.Genres | Include.ArtistCredits | Include.Releases);
            return MBRecordingToTrack(mb_rec, includeRights: true);
        }

        public Artist GetArtistByMBID(Guid mbid)
        {
            var mb_artist = _query.LookupArtist(mbid, Include.Releases | Include.Recordings);
            return MBArtistToArtist(mb_artist);
        }

        public Album GetAlbumByMBID(Guid mbid)
        {
            var mb_rel = _query.LookupRelease(mbid, Include.Genres | Include.ArtistCredits | Include.Recordings | Include.Media);
            return MBReleaseToAlbum(mb_rel);
        }

        public Feed<Track> GetArtistTracksByMBID(Guid mbid, string requestPath, int chunkSize)
        {
            var results = _query.BrowseAllArtistRecordings(mbid, pageSize: chunkSize, inc: Include.ArtistCredits);
            var updated = DateTime.Now;
            Feed<Track> feed = new()
            {
                Id = mbid.ToString(),
                Title = "tracks",
                Links = { new(requestPath) },
                Updated = updated,
                Entries = new(chunkSize)
            };

            // Add results to feed
            foreach (var mb_rec in results)
            {
                if (feed.Entries.Count == chunkSize) break;

                feed.Entries.Add(MBRecordingToTrack(mb_rec, updated: updated, includeRights: true));
            }

            return feed;
        }

        public Feed<Album> GetArtistAlbumsByMBID(Guid mbid, string requestPath)
        {
            var results = _query.BrowseAllArtistReleases(mbid, inc: Include.ArtistCredits);
            var updated = DateTime.Now;
            Feed<Album> feed = new()
            {
                Id = mbid.ToString(),
                Title = "albums",
                Links = { new(requestPath) },
                Updated = updated,
                Entries = new()
            };

            // Add results to feed
            foreach (var mb_release in results)
            {
                //if (feed.Entries.Count == chunkSize) break;

                feed.Entries.Add(MBReleaseToAlbum(mb_release, updated: updated));
            }

            return feed;
        }

        public Track MBRecordingToTrack(IRecording mb_rec, DateTime? updated = null, bool includeRights = false)
        {
            updated ??= DateTime.Now;
            var mb_artist = mb_rec.ArtistCredit[0].Artist;
            var artist = MBArtistToMiniArtist(mb_artist);

            Track track = new()
            {
                Id = mb_rec.Id.ToString(),
                Title = mb_rec.Title,
                PrimaryArtist = artist,
                AlbumArtist = artist,
                Duration = mb_rec.Length ?? TimeSpan.Zero,
                Artists = new List<MiniArtist>(mb_rec.ArtistCredit.Count),
                Updated = updated.Value,
            };

            if (mb_rec.Releases != null && mb_rec.Releases.Count > 0)
                track.Album = MBReleaseToMiniAlbum(mb_rec.Releases[0]);

            foreach (var mb_credit in mb_rec.ArtistCredit)
                track.Artists.Add(MBNameCreditToMiniArtist(mb_credit));

            if (includeRights)
                AddDefaultRights(ref track);

            return track;
        }

        public Artist MBArtistToArtist(IArtist mb_artist, DateTime? updated = null)
        {
            updated ??= DateTime.Now;
            Artist artist = new()
            {
                Id = mb_artist.Id.ToString(),
                Title = mb_artist.Name,
                SortTitle = mb_artist.SortName,
                IsVariousArtist = mb_artist.Id == ARTIST_VARIOUSARTISTS,
                BiographyLink = "https://bing.com",
                Updated = updated.Value,
            };

            if (mb_artist.Genres != null && mb_artist.Genres.Count > 0)
            {
                var mb_genre = mb_artist.Genres[0];
                if (mb_genre != null)
                    artist.PrimaryGenre = MBGenreToGenre(mb_genre);

                artist.Genres ??= new();
                foreach (var genre in mb_artist.Genres)
                    artist.Genres.Add(MBGenreToGenre(genre));
            }

            return artist;
        }

        public Album MBReleaseToAlbum(IRelease mb_rel, DateTime? updated = null)
        {
            updated ??= DateTime.Now;
            var mb_artist = mb_rel.ArtistCredit[0].Artist;
            var artist = MBArtistToMiniArtist(mb_artist);

            Album album = new()
            {
                Id = mb_rel.Id.ToString(),
                Title = mb_rel.Title,
                PrimaryArtist = artist,
                Artists = mb_rel.ArtistCredit.Select(mb_credit => MBNameCreditToMiniArtist(mb_credit)).ToList(),
                ReleaseDate = mb_rel.Date?.NearestDate ?? default,
                Updated = updated.Value,
            };

            if (mb_rel.Genres != null && mb_rel.Genres.Count > 0)
                album.PrimaryGenre = MBGenreToGenre(mb_rel.Genres[0]);

            if (mb_rel.Date != null)
                album.ReleaseDate = mb_rel.Date.NearestDate;

            if (mb_rel.CoverArtArchive != null && mb_rel.CoverArtArchive.Artwork)
            {
                album.Images = new();
                if (mb_rel.CoverArtArchive.Front)
                    album.Images.Add(new()
                    {
                        Instances = new()
                        {
                            new() { Id = album.Id }
                        }
                    });
            }

            if (mb_rel.Media != null && mb_rel.Media.Count > 0)
            {
                var mb_media = mb_rel.Media[0];
                if (mb_media.Tracks != null && mb_media.Tracks.Count > 0)
                {
                    album.Tracks = new();
                    foreach (var mb_track in mb_media.Tracks)
                        album.Tracks.Add(MBTrackToTrack(mb_track, trackArtist: artist, updated: updated, includeRights: true));
                }
            }

            return album;
        }

        public MiniArtist MBNameCreditToMiniArtist(INameCredit mb_credit)
        {
            return new()
            {
                Id = mb_credit.Artist.Id,
                Title = mb_credit.Name
            };
        }

        public MiniArtist MBArtistToMiniArtist(IArtist mb_artist)
        {
            return new()
            {
                Id = mb_artist.Id,
                Title = mb_artist.Name
            };
        }

        public MiniAlbum MBReleaseToMiniAlbum(IRelease mb_rel)
        {
            return new()
            {
                Id = mb_rel.Id,
                Title = mb_rel.Title
            };
        }

        public Track MBTrackToTrack(ITrack mb_track, MiniArtist trackArtist, DateTime? updated = null, bool includeRights = false)
        {
            updated ??= DateTime.Now;

            Track track = new()
            {
                Id = mb_track.Id.ToString(),
                Title = mb_track.Title,
                PrimaryArtist = trackArtist,
                AlbumArtist = trackArtist,
                Artists = mb_track.ArtistCredit?.Select(mb_credit => MBNameCreditToMiniArtist(mb_credit)).ToList(),
                Duration = mb_track.Length ?? TimeSpan.Zero,
                TrackNumber = mb_track.Position ?? 0,
                Updated = updated.Value,
            };

            if (includeRights)
                AddDefaultRights(ref track);

            return track;
        }

        public Genre MBGenreToGenre(IGenre mb_genre)
        {
            return new()
            {
                Id = mb_genre.Id.ToString(),
                Title = mb_genre.Name
            };
        }

        public void AddDefaultRights<T>(ref T media) where T : Media
        {
            media.Rights ??= new();
            media.Rights.Add(new()
            {
                ProviderCode = "117767492:MP3_DOWNLOAD_UENC_256kb_075",
                Price = 800,
                CurrencyCode = PriceTypeEnum.Points,
                LicenseType = "Preview",
                LicenseRight = MediaRightsEnum.PreviewStream,
                AudioEncoding = AudioEncodingEnum.MP3,
                OfferId = Guid.Parse("9534a201-2102-11db-89ca-0019b92a3933"),
            });
        }
    }
}
