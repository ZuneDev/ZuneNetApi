﻿using MetaBrainz.MusicBrainz;
using Microsoft.AspNetCore.Hosting;
using OwlCore.Net.Http;
using System;
using System.Net.Http;
using Zune.Xml.Catalog;

namespace Zune.Net.Helpers
{
    public static partial class MusicBrainz
    {
        public static readonly Query _query = new("Zune", "4.8", "https://github.com/ZuneDev/ZuneNetApi");

        public static void Initialize(IWebHostEnvironment env)
        {
            // assume worst-case scenario, mix is getting hit at the same time as catalog. The
            // frontent cache, nginx, will serve up cached results, but this interleave of 1.5s
            // might be enought to avoid a race.
            Query.DelayBetweenRequests = 1.5;

            _query.ConfigureClientCreation(delegate
            {
                // might be useful to put this as a shared resource between mix and cog
                var cachePath = System.IO.Path.Combine(env.ContentRootPath, "bin", "cache");
                var cacheTime = TimeSpan.FromHours(1);
                return new HttpClient(new CachedHttpClientHandler(cachePath, cacheTime));
            });
        }

        public static void AddDefaultRights<T>(ref T media) where T : Media
        {
            media.Rights ??= new();
            media.Rights.Add(new()
            {
                MediaInstanceId = "3b871100-1000-11db-89ca-0019b92a3933",
                ProviderCode = "117767492:MP3_DOWNLOAD_UENC_256kb_075",
                Price = 0,
                CurrencyCode = null,
                LicenseType = "Download",
                LicenseRight = MediaRightsEnum.Download,
                AudioEncoding = AudioEncodingEnum.MP3,
                OfferId = Guid.Parse("9534a201-2102-11db-89ca-0019b92a3933"),
                FileSize = 500
            });

            if (media is Album album)
                AddDefaultAlbumRights(ref album);
            else if (media is Track track)
                AddDefaultTrackRights(ref track);

            media.Actionable = true;
        }

        public static void AddDefaultAlbumRights(ref Album album)
        {
            album.Rights ??= new();
            album.Rights.Add(new()
            {
                MediaInstanceId = "3b871100-1000-11db-89ca-0019b92a3936",
                ProviderCode = "117767492:MP3_DOWNLOAD_UENC_256kb_075",
                Price = 0,
                CurrencyCode = "MPT",
                LicenseType = "Download",
                LicenseRight = MediaRightsEnum.AlbumPurchase,
                AudioEncoding = AudioEncodingEnum.MP3,
                OfferId = Guid.Parse("9534a201-2102-11db-89ca-0019b92a3936"),
            });
            album.Actionable = true;
        }

        public static void AddDefaultTrackRights(ref Track track)
        {
            track.Rights ??= new();
            track.Rights.Add(new()
            {
                MediaInstanceId = "3b871100-1000-11db-89ca-0019b92a3934",
                ProviderCode = "117767492:MP3_DOWNLOAD_UENC_256kb_075",
                Price = 0,
                CurrencyCode = "MPT",
                LicenseType = "MP3 Purchase",
                LicenseRight = MediaRightsEnum.Purchase,
                AudioEncoding = AudioEncodingEnum.MP3,
                OfferId = Guid.Parse("9534a201-2102-11db-89ca-0019b92a3934"),
            });
            track.Rights.Add(new()
            {
                MediaInstanceId = "3b871100-1000-11db-89ca-0019b92a3935",
                ProviderCode = "117767492:MP3_DOWNLOAD_UENC_256kb_075",
                Price = 0,
                CurrencyCode = "MPT",
                LicenseType = "WMA Preview",
                LicenseRight = MediaRightsEnum.Preview,
                AudioEncoding = AudioEncodingEnum.WMA,
                OfferId = Guid.Parse("9534a201-2102-11db-89ca-0019b92a3935"),
            });
        }
    }
}
