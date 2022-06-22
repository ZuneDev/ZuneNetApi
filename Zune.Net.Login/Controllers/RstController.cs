using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Zune.DB;

namespace Zune.Net.Login.Controllers
{
    [Route("/RST2.srf")]
    [ApiController]
    public class RstController : ControllerBase
    {
        const string LOGIN_LIVE_COM = "login.live.com";

        private readonly ZuneNetContext _database;
        public RstController(ZuneNetContext database)
        {
            _database = database;
        }

        [HttpPost]
        public async Task Post()
        {
            string destPath = $"https://{LOGIN_LIVE_COM}{Request.Path}";
            using HttpClient client = new();

            StreamContent content = new(Request.Body);
            HttpRequestMessage request = new(HttpMethod.Post, destPath)
            {
                Content = content
            };
            
            foreach (var header in Request.Headers)
                request.Headers.TryAddWithoutValidation(header.Key, (IEnumerable<string>)header.Value);
            // Ensure that SSL connection can be made
            request.Headers.Host = LOGIN_LIVE_COM;

            var response = await client.SendAsync(request);
            Response.StatusCode = (int)response.StatusCode;
            Response.ContentLength = response.Content.Headers.ContentLength;
            Response.ContentType = Atom.Constants.SOAP_XML_MIMETYPE + "; charset=utf-8";

            // Copy body into response
            await response.Content.ReadAsStream().CopyToAsync(Response.Body);
            
            await Response.CompleteAsync();
        }
    }
}