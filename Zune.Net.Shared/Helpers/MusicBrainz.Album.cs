using Atom.Xml;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using MetaBrainz.MusicBrainz.Interfaces.Searches;
using System;
using System.Collections.Generic;
using System.Linq;
using Zune.Xml.Catalog;

namespace Zune.Net.Helpers
{
    public partial class MusicBrainz
    {
        public static Feed<Album> SearchAlbums(string query, string requestPath)
        {
            var results = _query.FindAllReleases(query, simple: true);
            var updated = DateTime.Now;
            Feed<Album> feed = new()
            {
                Id = "albums",
                Title = "Albums",
                Links = { new(requestPath) },
                Updated = updated,
                Entries = ((IEnumerable<ISearchResult<IRelease>>)results)
                    .Take(40).Select(mb_rel => MBReleaseToAlbum(mb_rel.Item, updated: updated)).ToList(),
            };

            return feed;
        }

        public static Album GetAlbumByMBID(Guid mbid)
        {
            var mb_rel = _query.LookupRelease(mbid, Include.Genres | Include.ArtistCredits | Include.Recordings | Include.Media);
            return MBReleaseToAlbum(mb_rel);
        }

        public static Uri GetAlbumArtByMBID(Guid mbid)
        {
            var mb_rel = _query.LookupRelease(mbid, Include.Genres | Include.ArtistCredits | Include.Recordings | Include.Media);
            if(mb_rel.CoverArtArchive.Front)
            {
                new Uri($"http://coverartarchive.org/release/{mbid}/front");
            }
            return null;
        }


        public static Album MBReleaseToAlbum(IRelease mb_rel, DateTime? updated = null, bool includeRights = true)
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
                Images = new()
                {
                    new() { Id = mb_rel.Id }
                },
                Updated = updated.Value,
            };

            if (mb_rel.Genres != null && mb_rel.Genres.Count > 0)
                album.PrimaryGenre = MBGenreToGenre(mb_rel.Genres[0]);

            if (mb_rel.Date != null)
                album.ReleaseDate = mb_rel.Date.NearestDate;

            if (mb_rel.Media != null && mb_rel.Media.Count > 0)
            {
                var mb_media = mb_rel.Media[0];
                if (mb_media.Tracks != null && mb_media.Tracks.Count > 0)
                {
                    album.Tracks = new();
                    foreach (var mb_track in mb_media.Tracks)
                        album.Tracks.Add(MBTrackToTrack(mb_track, trackArtist: artist, updated: updated, includeRights: includeRights));
                }
            }

            if (includeRights)
                MusicBrainz.AddDefaultRights(ref album);

            return album;
        }


        public static MiniAlbum MBReleaseToMiniAlbum(IRelease mb_rel)
        {
            return new()
            {
                Id = mb_rel.Id,
                Title = mb_rel.Title
            };
        }
    }
}
