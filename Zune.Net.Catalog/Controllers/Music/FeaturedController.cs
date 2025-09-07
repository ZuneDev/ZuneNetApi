using System;
using System.Collections.Generic;
using System.Linq;
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
}