using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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
        const string LOGIN_LIVE_COM = "login.live.com";
        const string CREDPROP_XPATH = "//psf:credProperty[@Name=\"{0}\"]";

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

            // Save relevant information to database
            string xml = await response.Content.ReadAsStringAsync();
            var doc = new XPathDocument(new StringReader(xml));
            XPathNavigator nav = doc.CreateNavigator();
            XmlNamespaceManager namespaceResolver = new(nav.NameTable);
            namespaceResolver.AddNamespace("psf", "http://schemas.microsoft.com/Passport/SoapServices/SOAPFault");
            namespaceResolver.AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");

            var tokenElem = nav.SelectSingleNode("//wsse:BinarySecurityToken[@Id=\"Compact1\"]", namespaceResolver);
            var cidElem = nav.SelectSingleNode(string.Format(CREDPROP_XPATH, "CID"), namespaceResolver);
            string? token = tokenElem?.Value;
            string? cid = cidElem?.Value;
            if (token != null && cid != null)
                await _database.AddToken(token, cid);

            // Copy body into response
            await response.Content.CopyToAsync(Response.Body);
            await Response.CompleteAsync();
        }
    }
}