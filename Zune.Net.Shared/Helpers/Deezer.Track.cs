using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using Zune.Xml.Catalog;

namespace Zune.Net.Shared.Helpers
{
    public partial class Deezer
    {
        public static IRecording GetMBRecordingByDZTrack(JToken dz_track)
        {
            JToken dz_artist = dz_track["artist"];
            JToken dz_album = dz_track["album"];
            var results = MusicBrainz._query.FindAllRecordings(
                $"artistname:{dz_artist.Value<string>("name")} AND release:{dz_album.Value<string>("title")} " +
                $"AND recording:{dz_track.Value<string>("title")}", simple: false);

            return results.FirstOrDefault()?.Item;
        }

        public static Track DZTrackToTrack(JToken dz_track, DateTime? updated = null, bool includeRights = true)
        {
            updated ??= DateTime.Now;

            Track track = new()
            {
                Id = dz_track.Value<long>("id").ToString(),
                Title = dz_track.Value<string>("title"),
                Popularity = dz_track.Value<long>("rank"),
                Explicit = dz_track.Value<bool>("is_explicit"),
                Updated = updated.Value,
            };

            if (includeRights)
                MusicBrainz.AddDefaultRights(ref track);

            return track;
        }
    }
}
