using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Zune.Net.Middleware;

public partial class RouteCultureProvider(RequestCulture requestCulture) : IRequestCultureProvider
{
    private readonly CultureInfo _defaultCulture = requestCulture.Culture;
    private readonly CultureInfo _defaultUICulture = requestCulture.UICulture;

    public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
        // Parsing language from url path, which looks like "/en/home/index"
        PathString url = httpContext.Request.Path;

        // Test any culture in route
        if (url.ToString().Length <= 1)
        {
            // Set default Culture and default UICulture
            return Task.FromResult(new ProviderCultureResult(_defaultCulture.TwoLetterISOLanguageName, _defaultUICulture.TwoLetterISOLanguageName));
        }

        var parts = httpContext.Request.Path.Value.Split('/');
        var culture = parts[1];

        // Test if the culture is properly formatted
        if (!IsCulture(culture))
        {
            // Set default Culture and default UICulture
            return Task.FromResult(new ProviderCultureResult(_defaultCulture.TwoLetterISOLanguageName, _defaultUICulture.TwoLetterISOLanguageName));
        }

        // Set Culture and UICulture from route culture parameter
        return Task.FromResult(new ProviderCultureResult(culture, culture));
    }

    [GeneratedRegex(@"^[a-z]{2}(-[A-Z]{2})*$")]
    private static partial Regex CulturePattern();

    internal static bool IsCulture(string candidate) => CulturePattern().IsMatch(candidate);
}
