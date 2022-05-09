using Atom.Xml;
using Flurl;
using Flurl.Http;
using System.Globalization;

namespace Zune.Net.Shared.Helpers.AppleMusic
{
    public static partial class AppleMusicClient
    {
        public static readonly Author AM_AUTHOR = new()
        {
            Name = "Apple Inc.",
            Url = "https://apple.com"
        };

        public static readonly string AM_HOST_BASE = "https://amp-api.podcasts.apple.com";
        public static readonly string AM_HOST_BASE_V1 = AM_HOST_BASE.AppendPathSegment("v1");
        public static readonly string IT_HOST_BASE = "https://itunes.apple.com";

        public static IFlurlRequest GetBase()
        {
            return AM_HOST_BASE_V1.WithOAuthBearerToken(Constants.AM_TOKEN);
        }

        public static void GetCultureStrings(CultureInfo culture, out string regionStr, out string localeStr)
        {
            RegionInfo region = culture != null ? new(culture.LCID) : RegionInfo.CurrentRegion;
            culture ??= CultureInfo.CurrentCulture;

            regionStr = region.TwoLetterISORegionName;
            localeStr = culture.Name;
        }
    }
}
