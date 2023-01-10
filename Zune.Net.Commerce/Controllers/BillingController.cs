using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;
using System.Text;
using Zune.DB;
using Zune.Xml.Commerce;

namespace CommerceZuneNet.Controllers
{
    [Route("/{version}/{language}/billing/{action}")]
    [Route("/{language}/billing/{action}")]
    [ApiController]
    public class BillingController : ControllerBase
    {
        private readonly ZuneNetContext _database;
        public BillingController(ZuneNetContext database)
        {
            _database = database;
        }

        [HttpPost]
        public ActionResult<Feed> PurchaseHistory()
        {
            return new Feed
            {
                Entries =
                {
                    new Entry
                    {
                        Id = "06d4ec5e-a1b2-4895-9a09-ca3e8451bcc7",
                        Content = "Purchased something from Chuck Berry",
                        Links =
                        {
                            new("http://catalog.zune.net/v3.2/en-US/music/album/06d4ec5e-a1b2-4895-9a09-ca3e8451bcc7")
                        },
                        Title = "Oh Baby Doll / Lajaunda (espanol)"
                    }
                }
            };
        }

        [HttpPost]
        public ActionResult<EnumeratePointsBundlesResponse> EnumeratePointsBundles()
        {
            var id = Guid.NewGuid().ToString();
            return new EnumeratePointsBundlesResponse
            {
                PointsBundleOffers = new()
                {
                    new()
                    {
                        OfferId = id,
                        OfferName = "Some streaming service",
                        IsTrial = false,
                        WholePrice = 800,
                        FractionalPrice = 10,
                        NumPoints = 800,
                        PriceText = "800pt",
                        TaxType = "VAT",
                        UserIsSubscribed = false,

                        Media = id,
                        PromoPoints = 900,
                        Subscription = false,
                        Trial = false,
                    }
                }
            };
        }
    }
}
