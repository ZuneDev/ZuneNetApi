using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Zune.Commerce.Xml;

namespace CommerceZuneNet.Controllers
{
    [Route("/{version}/{language}/account/{action=SignIn}")]
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

            var response = new SignInResponse
            {
                AccountState = new AccountState
                {
                    AcceptedTermsOfService = false,
                    AccountSuspended = false,
                    BillingUnavailable = true,
                    SignInErrorCode = 0,
                    SubscriptionLapsed = true,
                    TagChangeRequired = false
                },
                AccountInfo = new AccountInfo
                {
                    UsageCollectionAllowed = false,
                    ExplicitPrivilege = false,
                    Lightweight = false,
                    Locale = System.Globalization.CultureInfo.CurrentCulture.Name,
                    ParentallyControlled = false,
                    ZuneTag = "YoshiAsk",
                    Xuid = zuneId
                },
                Balances = new Balances
                {
                    PointsBalance = 0.0,
                    SongCreditBalance = 0.0,
                    SongCreditRenewalDate = DateTime.Now.AddDays(1).ToString()
                },
                SubscriptionInfo = new SubscriptionInfo
                {
                    BillingInstanceId = "6cba2616-c59a-4dd5-bc9e-d41a45215cfa",

                },
                TunerRegisterInfo = new TunerRegisterInfo()
            };

            // Serialize the response to XML
            XmlSerializer serializer = new XmlSerializer(typeof(SignInResponse));
            Stream body = new MemoryStream();
            serializer.Serialize(body, response);
            body.Flush();
            body.Position = 0;

            return File(body, "application/xml");
        }
    }
}
