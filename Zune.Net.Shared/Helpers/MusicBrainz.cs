using MetaBrainz.MusicBrainz;
using System;
using Zune.Xml.Catalog;

namespace Zune.Net.Shared.Helpers
{
    public static partial class MusicBrainz
    {
        public static readonly Query _query = new("Zune", "4.8", "https://github.com/ZuneDev/ZuneNetApi");

        public static void AddDefaultRights<T>(ref T media) where T : Media
        {
            media.Rights ??= new();
            media.Rights.Add(new()
            {
                MediaInstanceId = "3b871100-1000-11db-89ca-0019b92a3933",
                ProviderCode = "117767492:MP3_DOWNLOAD_UENC_256kb_075",
                Price = 0,
                CurrencyCode = PriceTypeEnum.None,
                LicenseType = "Download",
                LicenseRight = MediaRightsEnum.Download,
                AudioEncoding = AudioEncodingEnum.MP3,
                OfferId = Guid.Parse("9534a201-2102-11db-89ca-0019b92a3933"),
                FileSize = 500
            });
        }
    }
}
