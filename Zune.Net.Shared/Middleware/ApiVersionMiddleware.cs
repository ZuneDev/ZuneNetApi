// https://gist.github.com/fernando-almeida/2b1f59e5f7f99a2f31d95471b895f625#file-urlsegmentapiversionstripmiddleware

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zune.Net.Features;

namespace Zune.Net.Middleware
{

    /// <summary>
    /// Rewrite URL API version segment if it exists
    /// </summary>
    public class ApiVersionMiddleware
    {
        private static readonly string DEFAULT_API_VERSION_PREFIX = "v";

        private readonly string _apiVersionVersion;
        private readonly RequestDelegate _next;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next">Next request delegate</param>
        /// <param name="apiVersionPrefix">API version prefix (optional)</param>
        public ApiVersionMiddleware(RequestDelegate next, string apiVersionPrefix = null)
        {
            ArgumentNullException.ThrowIfNull(next);
            _apiVersionVersion = Regex.Escape(apiVersionPrefix ?? DEFAULT_API_VERSION_PREFIX);
            _next = next;
        }

        public Task InvokeAsync(HttpContext httpContext)
        {
            // https://github.com/Microsoft/aspnet-api-versioning/wiki/Version-Format
            // [Version Group.]<Major>.<Minor>[-Status]
            // <Version Group>[.<Major>[.Minor]][-Status]
            var urlSegmentApiVersionRegexes = new Regex[] {
                new($@"{_apiVersionVersion}(?<apiVersion>(?:(?<group>\d{4}-\d{2}-\d{2})\.)?(?<major>\d+)(?:\.(?<minor>\d+))?(?:-(?<status>\w+))?)/?$"),
                new($@"{_apiVersionVersion}(?<apiVersion>(?<group>\d{4}-\d{2}-\d{2})(?:(?:\.(?<major>\d+)(?:\.(?<minor>\d+))))?(?:-(?<status>\w+))?)  /?$"),
            };

            var request = httpContext.Request;
            var uri = new Uri($"{request.Scheme}://{request.Host}{request.Path.ToUriComponent()}");

            var apiVersionSegment = uri.Segments
                .Where(segment => urlSegmentApiVersionRegexes.Any(regex => regex.Match(segment).Success))
                .FirstOrDefault();

            if (apiVersionSegment != null)
            {
                var newPath = string.Join("", uri.Segments.Where(segment => segment != apiVersionSegment));
                httpContext.Request.Path = new PathString(newPath);

                var match = urlSegmentApiVersionRegexes.Select(regex => regex.Match(apiVersionSegment))
                    .Where(regex => regex.Success)
                    .First();
                var rawApiVersion = match.Groups["apiVersion"].Value;

                ApiVersionFeature apiVersionFeature = new(rawApiVersion);
                httpContext.Features.Set<IApiVersionFeature>(apiVersionFeature);
            }
            return _next(httpContext);
        }
    }

    public static class ApiVersionMiddlewareExtensions
    {
        public static IApplicationBuilder UseVersionStripping(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiVersionMiddleware>();
        }
    }
}