using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppleMusicSharp;
using Atom.Xml;
using Microsoft.AspNetCore.Mvc;
using Zune.Net.Helpers;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Controllers.Music;

[Route("/music/featured/")]
[Produces(Atom.Constants.ATOM_MIMETYPE)]
public class FeaturedController : Controller
{
    [HttpGet, Route("albums")]
    public async Task<ActionResult<Feed<Album>>> Albums([FromServices] AppleMusicClient amClient)
    {
        var room = await amClient.GetRoomAsync("6751999710");
        var am_albums = room.Data.First().Relationships.Contents.Data;
        
        List<Album> albums = new(20);

        foreach (var am_album in am_albums)
        {
            if (albums.Count >= 5)
                break;
            
            var album = await MusicBrainz.GetAlbumByBarcodeAsync(am_album.Attributes.Upc);
            
            // MusicBrainz failed to lookup by barcode
            if (album is null)
            {
                Console.WriteLine($"Failed to lookup album '{am_album.Attributes.Name}' {am_album.Id} by barcode");
                continue;
                
                var results = await MusicBrainz._query.FindReleasesAsync(
                    $"artistname:{am_album.Attributes.ArtistName} AND release:{am_album.Attributes.Name}",
                    limit: 1, simple: false);

                var release = results.Results.FirstOrDefault()?.Item;
                if (release is null)
                    continue;
                
                album = MusicBrainz.MBReleaseToAlbum(release);
            }
            else
            {
                Console.WriteLine($"Found album '{am_album.Attributes.Name}' {am_album.Id} as {album.Id}");
            }
            
            albums.Add(album);
        }
        
        return new Feed<Album>
        {
            Author = new Author
            {
                Name = "Apple Music",
                Url = "https://music.apple.com"
            },
            Entries = albums.OrderByDescending(a => a.ReleaseDate).ToList()
        };
    }

    private static async Task AlbumsAsync(IEnumerable<AppleMusicSharp.Models.Album> am_albums)
    {
        ConcurrentBag<Album> albums = [];
        CancellationTokenSource cts = new();
        await Parallel.ForEachAsync(am_albums, cts.Token, async (am_album, token) =>
        {
            var album = await MusicBrainz.GetAlbumByBarcodeAsync(am_album.Attributes.Upc);
            token.ThrowIfCancellationRequested();
            
            // MusicBrainz failed to lookup by barcode
            if (album is null)
            {
                Console.WriteLine($"Failed to lookup album '{am_album.Attributes.Name}' {am_album.Id} by barcode");
                
                var results = await MusicBrainz._query.FindReleasesAsync(
                    $"artistname:{am_album.Attributes.ArtistName} AND release:{am_album.Attributes.Name}",
                    limit: 1, simple: false,
                    cancellationToken: token);
                token.ThrowIfCancellationRequested();

                var release = results.Results.FirstOrDefault()?.Item;
                album = MusicBrainz.MBReleaseToAlbum(release);
            }
            else
            {
                Console.WriteLine($"Found album '{am_album.Attributes.Name}' {am_album.Id} as {album.Id}");
            }
            
            token.ThrowIfCancellationRequested();
            albums.Add(album);
            
            if (albums.Count >= 20)
                await cts.CancelAsync();
        });
    }
    
    [HttpGet, Route("albums2")]
    public async Task<ActionResult<Feed<Album>>> Albums2()
    {
        string[] albumMbids = ["3229ddb3-f289-41b2-af89-ca3ce6a7541d"];

        ConcurrentBag<Album> albums = [];

        Parallel.ForEach(albumMbids, albumMbid =>
        {
            var album = MusicBrainz.GetAlbumByMBID(new Guid(albumMbid));
            albums.Add(album);
        });

        Feed<Album> feed = new()
        {
            Id = "albums",
            Title = "Albums",
            Author = new Author
            {
                Name = "Joshua Askharoun",
                Url = "https://josh.askharoun.com"
            },
            Updated = DateTime.Parse("2025-09-06T13:38:41.7821268-05:00"),
            Entries = albums.OrderByDescending(t => t.Popularity).ToList()
        };

        return feed;
    }
}