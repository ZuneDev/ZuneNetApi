using MetaBrainz.MusicBrainz;
using System;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Helpers
{
    public static partial class MusicBrainz
    {
        private static readonly Query _query = new("Zune", "4.8", "https://github.com/ZuneDev/ZuneNetApi");

        public static void AddDefaultRights<T>(ref T media) where T : Media
        {
            media.Rights ??= new();
            media.Rights.Add(new()
            {
                ProviderCode = "117767492:MP3_DOWNLOAD_UENC_256kb_075",
                Price = 800,
                CurrencyCode = PriceTypeEnum.Points,
                LicenseType = "Preview",
                LicenseRight = MediaRightsEnum.PreviewStream,
                AudioEncoding = AudioEncodingEnum.MP3,
                OfferId = Guid.Parse("9534a201-2102-11db-89ca-0019b92a3933"),
            });
        }
    }
}
