﻿// https://gist.github.com/fernando-almeida/2b1f59e5f7f99a2f31d95471b895f625#file-urlsegmentapiversionstripmiddleware

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
    /// Rewrite URL cultute segment if it exists
    /// </summary>
    public partial class CultureMiddleware
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

                if (IsCulture(cultureSegment))
                {
                    var newPath = string.Join("", uri.Segments.Where(s => !s.StartsWith(cultureSegment)));
                    httpContext.Request.Path = new PathString(newPath);

                    CultureFeature feature = new(cultureSegment);
                    httpContext.Features.Set<ICultureFeature>(feature);
                }
            }

            return _next(httpContext);
        }

        internal static bool IsCulture(string candidate) => CulturePattern().IsMatch(candidate);

        [GeneratedRegex(@"^[a-z]{2}(-[A-Z]{2})*$")]
        private static partial Regex CulturePattern();
    }

    public static class CultureMiddlewareExtensions
    {
        public static IApplicationBuilder UseCultureStripping(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CultureMiddleware>();
        }
    }
}