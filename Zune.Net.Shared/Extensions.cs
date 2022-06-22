using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
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

        public static IHostBuilder ConfigureZuneDB(this IHostBuilder host)
        {
            return host.ConfigureServices((ctx, s) =>
            {
                BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
                s.Configure<DB.ZuneNetContextSettings>(ctx.Configuration.GetSection("ZuneNetContext"));
                s.AddSingleton<DB.ZuneNetContext>();
            });
        }
    }
}
