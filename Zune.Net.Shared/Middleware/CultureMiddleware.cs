// https://gist.github.com/fernando-almeida/2b1f59e5f7f99a2f31d95471b895f625#file-urlsegmentapiversionstripmiddleware

using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Zune.Net.Middleware
{

    /// <summary>
    /// Rewrite URL cultute segment if it exists
    /// </summary>
    public class CultureMiddleware
    {
        private readonly RequestDelegate _next;

        public CultureMiddleware(RequestDelegate next)
        {
            ArgumentNullException.ThrowIfNull(next);
            _next = next;
        }

        public Task InvokeAsync(HttpContext httpContext)
        {
            var request = httpContext.Request;
            var uri = new Uri($"{request.Scheme}://{request.Host}{request.Path.ToUriComponent()}{request.QueryString}");

            var cultureSegment = uri.Segments.Skip(1).FirstOrDefault();
            if (cultureSegment is not null)
            {
                if (cultureSegment[^1] == '/')
                    cultureSegment = cultureSegment[..^1];

                if (RouteCultureProvider.IsCulture(cultureSegment))
                {
                    var newPath = string.Join("", uri.Segments.Where(s => !s.StartsWith(cultureSegment)));
                    httpContext.Request.Path = new PathString(newPath);

                    // TODO: Add feature
                    httpContext.Items.Add("Culture", cultureSegment);
                }
            }

            return _next(httpContext);
        }
    }

    public static class CultureMiddlewareExtensions
    {
        public static IApplicationBuilder UseCultureStripping(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CultureMiddleware>();
        }
    }
}