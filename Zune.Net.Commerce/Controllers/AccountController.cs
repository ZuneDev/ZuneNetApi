using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Zune.DB;
using Zune.DB.Models;
using Zune.Xml.Commerce;

namespace CommerceZuneNet.Controllers
{
    [Route("/{version}/{language}/account/{action=SignIn}")]
    [Route("/{language}/account/{action=SignIn}")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> SignIn()
        {
            string data = await new StreamReader(Request.Body).ReadToEndAsync();
            var xml = XDocument.Parse(data);
            
            var tunerInfo = xml.Descendants();
            string id = tunerInfo.First(e => e.Name.LocalName == "ID").Value;
            string name = tunerInfo.First(e => e.Name.LocalName == "Name").Value;
            string type = tunerInfo.First(e => e.Name.LocalName == "Type").Value;
            string version = tunerInfo.First(e => e.Name.LocalName == "Version").Value;

            string zuneId = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(id)));

            using var ctx = new ZuneNetContext();
            Member member = ctx.Members.Find(zuneId);
            SignInResponse response;

            if (member != null)
            {
                response = member.GetSignInResponse();
            }
            else
            {
                // TODO: Work out the error response format
                return NotFound();
                response = new SignInResponse
                {
                    AccountState = new AccountState
                    {
                        SignInErrorCode = 404
                    }
                };
            }

            // Serialize the response to XML
            XmlSerializer serializer = new XmlSerializer(typeof(SignInResponse));
            Stream body = new MemoryStream();
            serializer.Serialize(body, response);
            body.Flush();
            body.Position = 0;

            return File(body, "application/atom+xml");
        }
    }
}
