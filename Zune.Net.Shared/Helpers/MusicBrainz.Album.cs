using Atom.Xml;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using MetaBrainz.MusicBrainz.Interfaces.Searches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public static Album GetAlbumByRecordingId(Guid mbid)
        {
            var mb_rel = _query.LookupRecording(mbid, Include.Releases | Include.ArtistCredits);

            return MBReleaseToAlbum(mb_rel.Releases.First());
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
                album.Tracks = new();

                for (var mediaId = 0; mediaId < mb_rel.Media.Count; mediaId++)
                {
                    var mb_media = mb_rel.Media[mediaId];

                    if (mb_media.Tracks != null && mb_media.Tracks.Count > 0)
                    {
                        foreach (var mb_track in mb_media.Tracks)
                        {
                            album.Tracks.Add(MBTrackToTrack(mb_track, diskNumber: mediaId + 1, trackArtist: artist, updated: updated, includeRights: includeRights));
                        }
                    }
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

        public static async Task<List<Guid>> GetTracksByAlbumMbidAsync(Guid albumId)
        {
            var trackList = new List<Guid>();
            try
            {
                var album = await _query.LookupReleaseAsync(albumId, Include.Recordings);
                var medias = album.Media?.ToList() ?? new List<IMedium>();
                medias.ForEach(media => media.Tracks.ToList().ForEach(track => trackList.Add(track.Id)));
            }
            catch { }
            return trackList;
        }

        public static async Task<List<int>> GetGenreIdsByReleaseGroupMbidAsync(Guid releaseGroupId)
        {
            try
            {
                var genreIdList = new List<int>();
                var releaseGroup = await _query.LookupReleaseGroupAsync(releaseGroupId, Include.Tags | Include.Genres);
                foreach (var genre in releaseGroup.Genres?.ToList() ?? new List<IGenre>())
                {
                    var genreIndexId = Array.IndexOf(MusicBrainzGenreList.Genres, genre.Name);
                    if (genreIndexId > -1)
                    {
                        genreIdList.Add(genreIndexId);
                    }
                }

                foreach (var tag in releaseGroup.Tags?.ToList() ?? new List<ITag>())
                {
                    var genreIndexId = Array.IndexOf(MusicBrainzGenreList.Genres, tag.Name);
                    if (genreIndexId > -1)
                    {
                        genreIdList.Add(genreIndexId);
                    }
                }

                return genreIdList;
            } catch 
            {
                return new List<int>();
            }
        }

        public static async Task<List<int>> GetGenreIdsByAlbumMbidAsync(Guid albumId)
        {
            try
            {
                var album = await _query.LookupReleaseAsync(albumId, Include.ReleaseGroups);
                return await GetGenreIdsByReleaseGroupMbidAsync(album.ReleaseGroup.Id);
            }
            catch
            {
                return new List<int>();
            }
        }
    }
}
