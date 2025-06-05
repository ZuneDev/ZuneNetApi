using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Zune.Net.Middleware;

namespace Zune.Net
{
    public static class Extensions
    {
        private static string _homeRouteHtml;

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

        public static IApplicationBuilder UseCommonRouting(this IApplicationBuilder app)
        {
            return app.UseRequestBuffering()
                .UseVersionStripping()
                .UseCultureStripping();
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

        public static RouteHandlerBuilder MapHomeRoute(this IEndpointRouteBuilder endpoints)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .Single(n => n.EndsWith(".api-landing.html"));
            using (var sourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                using StreamReader reader = new(sourceStream);
                _homeRouteHtml = reader.ReadToEnd();
            }

            return endpoints.MapGet("/", () =>
            {
                return Results.Content(_homeRouteHtml, "text/html");
            });
        }

        public static void UseMusicBrainzCache(this IWebHostEnvironment env)
        {
            // assume worst-case scenario, mix is getting hit at the same time as catalog. The
            // frontend cache, nginx, will serve up cached results, but this interleave of 1.5s
            // might be enough to avoid a race.
            MetaBrainz.MusicBrainz.Query.DelayBetweenRequests = 1.5;

            Helpers.MusicBrainz.Query.ConfigureClientCreation(delegate
            {
                // might be useful to put this as a shared resource between mix and cog
                var cachePath = Path.Combine(env.ContentRootPath, "bin", "cache");
                var cacheTime = TimeSpan.FromHours(1);
                return new System.Net.Http.HttpClient(new OwlCore.Net.Http.CachedHttpClientHandler(cachePath, cacheTime));
            });
        }

        public static JToken SerializeToJson(this IConfiguration config)
        {
            var obj = new JObject();
            foreach (var child in config.GetChildren())
            {
                if (child.Path.EndsWith(":0"))
                {
                    var arr = new JArray();

                    foreach (var arrayChild in config.GetChildren())
                    {
                        arr.Add(arrayChild.SerializeToJson());
                    }

                    return arr;
                }

                obj.Add(child.Key, child.SerializeToJson());
            }

            if (obj.HasValues || config is not IConfigurationSection section) return obj;

            // Allow for json that has been embeded as a string in a single key
            if (section.Value.StartsWith('{') && section.Value.EndsWith('}'))
            {
                obj = JObject.Parse(section.Value);
                return obj;
            }

            return ParseJValue(section.Value);

            JValue ParseJValue(string value)
            {
                if (bool.TryParse(value, out var boolean))
                    return new JValue(boolean);

                if (long.TryParse(value, out var integer))
                    return new JValue(integer);

                if (decimal.TryParse(value, out var real))
                    return new JValue(real);

                return new JValue(value);
            }
        }

        public static (uint A, ushort B, ulong C) GetGuidParts(this Guid guid)
        {
            byte[] bytes = guid.ToByteArray();
            return (
                BitConverter.ToUInt32(bytes),
                BitConverter.ToUInt16(bytes, sizeof(uint)),
                BitConverter.ToUInt64(bytes, sizeof(uint) + sizeof(ushort))
            );
        }
    }
}
