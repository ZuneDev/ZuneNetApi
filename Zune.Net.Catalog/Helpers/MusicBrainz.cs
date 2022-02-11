using Atom.Xml;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;
using System.Collections.Generic;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Helpers
{
    public class MusicBrainz
    {
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

        public Track GetTrackByMBID(Guid mbid)
        {
            var mb_rec = _query.LookupRecording(mbid, Include.Genres | Include.ArtistCredits | Include.Releases);
            return MBRecordingToTrack(mb_rec, includeRights: true);
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
                ArtistName = artist.Title,
                PrimaryArtist = artist,
                AlbumArtist = artist,
                Duration = mb_rec.Length ?? TimeSpan.Zero,
                Artists = new List<MiniArtist>(mb_rec.ArtistCredit.Count),
                Updated = updated.Value,
            };

            var mb_album = mb_rec.Releases?[0];
            if (mb_album != null)
            {
                track.AlbumId = mb_album.Id;
                track.AlbumTitle = mb_album.Title;
            }

            foreach (var mb_credit in mb_rec.ArtistCredit)
                track.Artists.Add(MBNameCreditToMiniArtist(mb_credit));

            if (includeRights)
            {
                track.Rights = new()
                {
                    new()
                    {
                        ProviderCode = "117767492:MP3_DOWNLOAD_UENC_256kb_075",
                        Price = 800,
                        CurrencyCode = PriceTypeEnum.Points,
                        LicenseType = "Preview",
                        LicenseRight = MediaRightsEnum.PreviewStream,
                        AudioEncoding = AudioEncodingEnum.MP3,
                        OfferId = Guid.Parse("9534a201-2102-11db-89ca-0019b92a3933"),
                    }
                };
            }

            return track;
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

        public Genre MBGenreToGenre(IGenre mb_genre)
        {
            return new()
            {
                Id = mb_genre.Id.ToString(),
                Title = mb_genre.Name
            };
        }
    }
}
