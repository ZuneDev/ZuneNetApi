using AppleMusicSharp.Models;
using Flurl;
using Flurl.Http;

namespace AppleMusicSharp;

public class AppleMusicClient
{
    public string Storefront { get; set; } = "us";
    
    public string? Language { get; set; }
    
    public string Token { get; set; } = Constants.AM_TOKEN;

    public string? Origin { get; set; } = Constants.AM_ORIGIN;

    public static string GetImageUrl(string urlTemplate, int width, int height, string format = "jpeg", string c = "")
    {
        return urlTemplate
            .Replace("{w}", width.ToString("r"))
            .Replace("{h}", height.ToString("r"))
            .Replace("{f}", format)
            .Replace("{c}", c);
    }

    public async Task<Root<Room>> GetRoomAsync(string roomId, CancellationToken ct = default)
    {
        var url = GetEditorialBaseUrl()
            .AppendPathSegments("rooms", roomId);
        var request = BuildRequest(url);
        return await request.GetJsonAsync<Root<Room>>(cancellationToken: ct);
    }

    private Url GetBaseUrl(string area) => $"{Constants.BASEURL_AMPAPIEDGE}/v1/{area}/{Storefront}";
    
    private Url GetCatalogBaseUrl() => GetBaseUrl("catalog");
    
    private Url GetEditorialBaseUrl() => GetBaseUrl("editorial");
    
    private IFlurlRequest BuildRequest(Url url)
    {
        var request = url.WithOAuthBearerToken(Token);

        if (!string.IsNullOrWhiteSpace(Origin))
            request = request.WithHeader("Origin", Origin);

        if (!string.IsNullOrWhiteSpace(Language)) 
            request = request.SetQueryParam("l", Language);

        return request;
    }
}