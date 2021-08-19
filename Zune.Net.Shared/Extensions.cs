using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Zune.Net.Shared;

namespace Zune.Net
{
    public static class Extensions
    {
        public static MvcOptions UseZestFormatters(this MvcOptions options)
        {
            options.OutputFormatters.Insert(0, new ZestOutputFormatter());
            options.InputFormatters.Insert(0, new ZestInputFormatter(options));

            return options;
        }

        public static IApplicationBuilder UseRequestBuffering(this IApplicationBuilder app)
        {
            return app.Use((ctx, next) =>
            {
                ctx.Request.EnableBuffering(); // this is dumb
                return next();
            });
        }
    }
}
