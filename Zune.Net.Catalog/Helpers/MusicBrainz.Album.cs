using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;
using System.Linq;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Helpers
{
    public partial class MusicBrainz
    {
        public Album GetAlbumByMBID(Guid mbid)
        {
            var mb_rel = _query.LookupRelease(mbid, Include.Genres | Include.ArtistCredits | Include.Recordings | Include.Media);
            return MBReleaseToAlbum(mb_rel);
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


        public MiniAlbum MBReleaseToMiniAlbum(IRelease mb_rel)
        {
            return new()
            {
                Id = mb_rel.Id,
                Title = mb_rel.Title
            };
        }
    }
}
