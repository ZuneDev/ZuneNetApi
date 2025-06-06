using System.Collections.Generic;
using System.Linq;
using Zune.Xml.Catalog;

namespace Zune.DataProviders.LastFM;

public class LastFMProvider() : ITrackChartProvider
{
    public async IAsyncEnumerable<Track> GetTrackChart()
    {
        var fmTracks = await LastFM.GetTopTracks();

        foreach (var fmTrack in fmTracks.Take(10))
        {
            var mb_recording = LastFM.GetMBRecordingByFMTrack(fmTrack);
            if (mb_recording == null)
                continue;

            var track = Net.Helpers.MusicBrainz.MBRecordingToTrack(mb_recording, includeRights: true);
            track.Popularity = fmTrack.Rank ?? 0;
            track.PlayCount = fmTrack.PlayCount ?? 0;

            yield return track;
        }
    }
}
