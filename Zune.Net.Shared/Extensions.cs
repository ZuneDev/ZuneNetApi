using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using Zune.Net;

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

        public static IHostBuilder ConfigureZuneDB(this IHostBuilder host, bool skipRegisterContext = false)
        {
            return host.ConfigureServices((ctx, s) =>
            {
                BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
                s.Configure<DB.ZuneNetContextSettings>(ctx.Configuration.GetSection("ZuneNetContext"));
                if(!skipRegisterContext)
                {
                    s.AddSingleton<DB.ZuneNetContext>();
                }
            });
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
