using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

// Credit: https://markb.uk/asp-net-core-read-raw-request-body-as-string.html
namespace CommerceZuneNet.Helpers;
public static class RequestBodyHelpers
{
    public static async Task<string> GetRawBodyAsync(
    this HttpRequest request,
    Encoding encoding = null)
{
    if (!request.Body.CanSeek)
    {
        // We only do this if the stream isn't *already* seekable,
        // as EnableBuffering will create a new stream instance
        // each time it's called
        request.EnableBuffering();
    }

    request.Body.Position = 0;

    var reader = new StreamReader(request.Body, encoding ?? Encoding.UTF8);

    var body = await reader.ReadToEndAsync().ConfigureAwait(false);

    request.Body.Position = 0;

    return body;
}
}