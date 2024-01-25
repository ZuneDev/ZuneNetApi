using Atom.Xml;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using MetaBrainz.MusicBrainz.Interfaces.Searches;
using MetaBrainz.MusicBrainz.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Zune.Xml.Catalog;
using System.Collections.Concurrent;

namespace Zune.Net.Helpers
{
    public partial class MusicBrainz
    {

        private static readonly ConcurrentDictionary<String, Album> mbAlbumCache = new();
        public static Feed<Album> SearchAlbums(string query, string requestPath)
        {
            IReadOnlyList<ISearchResult<IReleaseGroup>> results = _query.FindReleaseGroups(query, 40,0,true).Results;
            var updated = DateTime.Now;
            Feed<Album> feed = new()
            {
                Id = "albums",
                Title = "Albums",
                Links = { new(requestPath) },
                Updated = updated,
                Entries = results.Select(mb_rel => MBReleaseGroupToAlbum(mb_rel.Item, updated: updated)).ToList(),
            };

            return feed;
        }

        public static Album GetAlbumByMBID(Guid mbid)
        {
            IReleaseGroup mb_rel = _query.LookupReleaseGroup(mbid, Include.Genres | Include.ArtistCredits);
            return MBReleaseGroupToAlbum(mb_rel);
        }

        public static Album MBReleaseGroupToAlbum(IReleaseGroup mb_rel_grp, DateTime? updated = null, bool includeRights = true)
        {
            if (mbAlbumCache.ContainsKey(mb_rel_grp.Id.ToString()))
            {
                return mbAlbumCache[mb_rel_grp.Id.ToString()];
            }
            IRelease mb_rel = _query.BrowseAllReleaseGroupReleases(mb_rel_grp.Id, 1, 0, Include.Recordings | Include.Media).First();
            updated ??= DateTime.Now;
            var mb_artist = mb_rel_grp.ArtistCredit[0].Artist;
            var artist = MBArtistToMiniArtist(mb_artist);
            
            Album album = new()
            {
                Id = mb_rel_grp.Id.ToString(),
                Title = mb_rel.Title,
                PrimaryArtist = artist,
                Artists = mb_rel_grp.ArtistCredit.Select(mb_credit => MBNameCreditToMiniArtist(mb_credit)).ToList(),
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

            mbAlbumCache.AddOrUpdate(album.Id, _ => album, (_, _) => album);
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
