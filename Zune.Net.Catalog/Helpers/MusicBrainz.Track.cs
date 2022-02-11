using Atom.Xml;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;
using System.Linq;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Helpers
{
    public partial class MusicBrainz
    {
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
                PrimaryArtist = artist,
                AlbumArtist = artist,
                Duration = mb_rec.Length ?? TimeSpan.Zero,
                Artists = mb_rec.ArtistCredit?.Select(mb_credit => MBNameCreditToMiniArtist(mb_credit)).ToList(),
                Updated = updated.Value,
            };

            if (mb_rec.Releases != null && mb_rec.Releases.Count > 0)
                track.Album = MBReleaseToMiniAlbum(mb_rec.Releases[0]);

            if (includeRights)
                AddDefaultRights(ref track);

            return track;
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
    }
}
