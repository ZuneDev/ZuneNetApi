using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Zune.DB;

namespace Zune.Net.Login.Controllers
{
    [Route("/RST2.srf")]
    [ApiController]
    public class RstController : ControllerBase
    {
        const string MSNMSGR_ESCARGOT_CHAT = "msnmsgr.escargot.chat";
        const string LOGIN_LIVE_COM = "login.live.com";
        const string CREDPROP_XPATH = "//psf:credProperty[@Name=\"{0}\"]";

        private readonly ZuneNetContext _database;
        private readonly HttpClient _client = new();

        public RstController(ZuneNetContext database)
        {
            _database = database;
        }

        [HttpPost, Route("/NotRST.srf")]
        public async Task<IActionResult> NotRST2()
        {
            // Get username from request
            if (!Request.Headers.TryGetValue("X-User", out var userNames))
                goto fail;
            string? userName = userNames.FirstOrDefault();
            if (userName == null)
                goto fail;

            // Forward request to Escargot servers
            var response = await ForwardRequest(MSNMSGR_ESCARGOT_CHAT, Request, Response);

            // Get token from response
            if (!response.Headers.TryGetValues("X-Token", out var tokens))
                goto fail;
            string? token = tokens.FirstOrDefault();
            if (token == null)
                goto fail;

            // Save token to DB
            await _database.AddToken(token, userName);

            return Ok();

        fail:
            return Unauthorized();
        }

        [HttpPost, Route("/RST2.srf")]
        public async Task RST2()
        {
            var response = await ForwardRequest(LOGIN_LIVE_COM, Request, Response);

            // Save relevant information to database
            string xml = await response.Content.ReadAsStringAsync();
            var doc = new XPathDocument(new StringReader(xml));
            XPathNavigator nav = doc.CreateNavigator();
            XmlNamespaceManager namespaceResolver = new(nav.NameTable);
            namespaceResolver.AddNamespace("psf", "http://schemas.microsoft.com/Passport/SoapServices/SOAPFault");
            namespaceResolver.AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");

            var tokenElem = nav.SelectSingleNode("//wsse:BinarySecurityToken[@Id=\"Compact1\"]", namespaceResolver);
            var userNameElem = nav.SelectSingleNode(string.Format(CREDPROP_XPATH, "AuthMembername"), namespaceResolver);
            string? token = tokenElem?.Value;
            string? userName = userNameElem?.Value;
            if (token != null && userName != null)
                await _database.AddToken(token, userName);
        }

        [NonAction]
        public async Task<HttpResponseMessage> ForwardRequest(string destHost, Microsoft.AspNetCore.Http.HttpRequest aspRequest, Microsoft.AspNetCore.Http.HttpResponse aspResponse)
        {
            string destPath = $"https://{destHost}{aspRequest.Path}";

            StreamContent content = new(aspRequest.Body);
            HttpRequestMessage request = new(new HttpMethod(aspRequest.Method), destPath)
            {
                Content = content
            };

            foreach (var header in aspRequest.Headers)
                request.Headers.TryAddWithoutValidation(header.Key, (IEnumerable<string>)header.Value);
            // Ensure that SSL connection can be made
            request.Headers.Host = destHost;

            var response = await _client.SendAsync(request);

            aspResponse.StatusCode = (int)response.StatusCode;
            aspResponse.ContentLength = response.Content.Headers.ContentLength;
            aspResponse.ContentType = response.Content.Headers.ContentType?.ToString() ?? $"{Atom.Constants.SOAP_XML_MIMETYPE}; charset=utf-8";
            foreach (var header in response.Headers)
                aspResponse.Headers.TryAdd(header.Key, header.Value.ToArray());
            await response.Content.CopyToAsync(Response.Body);

            await aspResponse.CompleteAsync();

            return response;
        }
    }
}