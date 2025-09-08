using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using Zune.Xml.Catalog;

namespace Zune.Net.Helpers;

public static class ListenBrainz
{
    public const string API_URL = "https://api.listenbrainz.org";

    public delegate string GetImageUrl(string caaReleaseMbid);

    public static async Task<List<Album>> ExploreFreshReleases(int days, string sort, bool future, int limit, GetImageUrl getImageUrl)
    {
        var response = await $"{API_URL}/1/explore/fresh-releases"
            .SetQueryParam("days", days)
            .SetQueryParam("sort", sort)
            .SetQueryParam("future", future)
            .WithHeader("User-Agent", "Zune/4.8")
            .GetJsonAsync<JObject>();

        var lb_releases = response["payload"]!["releases"]!;
        List<Album> albums = new(limit);

        foreach (var lb_release in lb_releases.Take(limit))
        {
            var artistName = lb_release.Value<string>("artist_credit_name");
            var artistMbids = lb_release["artist_mbids"]!.ToObject<List<Guid>>();
            var releaseMbid = lb_release.Value<string>("release_mbid");
            var releaseName = lb_release.Value<string>("release_name");
            var caaReleaseMbid = lb_release.Value<string>("caa_release_mbid");
            var releaseDate = lb_release.Value<DateTime>("release_date");
            var listenCount = lb_release.Value<int>("listen_count");
            
            // ListenBrainz combines all credited artists into the name field in alphabetical order,
            // which doesn't necessarily match the order of the artist MBIDs

            Album album = new()
            {
                Title = releaseName,
                Id = releaseMbid,
                ReleaseDate = releaseDate,
                PrimaryArtist = new MiniArtist
                {
                    Id = artistMbids[0],
                    Title = artistName,
                },
                Popularity = listenCount,
            };

            if (caaReleaseMbid is not null)
            {
                album.Images =
                [
                    new Image
                    {
                        Id = new Guid(caaReleaseMbid),
                        Instances =
                        [
                            new ImageInstance
                            {
                                Id = new Guid(caaReleaseMbid),
                                Url = getImageUrl(caaReleaseMbid)
                            }
                        ]
                    }
                ];
            }
            
            albums.Add(album);
        }

        return albums;
    }
    
    public static async Task<Dictionary<string, int>> GetRecordingPopularity(IEnumerable<string> recordingMbids)
    {
        var requestBody = new
        {
            recording_mbids = recordingMbids.ToList()
        };
        
        var httpResponse = await $"{API_URL}/1/popularity/recording"
            .WithHeader("User-Agent", "Zune/4.8")
            .PostJsonAsync(requestBody);
        
        var response = await httpResponse.GetJsonAsync<List<JObject>>();

        Dictionary<string, int> popularities = new();

        foreach (var entry in response)
        {
            var totalListenCount = entry["total_listen_count"]?.Value<int?>();
            if (totalListenCount is null)
                continue;
            
            var recordingMbid = entry["recording_mbid"]!.Value<string>();
            popularities[recordingMbid] = totalListenCount.Value;
        }

        return popularities;
    }
    
    public static async Task<Dictionary<string, int>> GetReleasePopularity(IEnumerable<string> releaseMbids)
    {
        var requestBody = new
        {
            release_mbids = releaseMbids.ToList()
        };
        
        var httpResponse = await $"{API_URL}/1/popularity/release"
            .WithHeader("User-Agent", "Zune/4.8")
            .PostJsonAsync(requestBody);
        
        var response = await httpResponse.GetJsonAsync<List<JObject>>();

        Dictionary<string, int> popularities = new();

        foreach (var entry in response)
        {
            var totalListenCount = entry["total_listen_count"]?.Value<int?>();
            if (totalListenCount is null)
                continue;
            
            var releaseMbid = entry["release_mbid"]!.Value<string>();
            popularities[releaseMbid] = totalListenCount.Value;
        }

        return popularities;
    }
}