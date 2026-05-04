using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Zune.Net.Helpers;

public static class ImageResizer
{
    /// <summary>
    /// Loads an image from the provided URL and produces a <see cref="FileContentResult"/>
    /// with the resized and re-encoded image.
    /// </summary>
    public static async Task<IActionResult> GetAndResizeImageAsync(this Controller controller,
        string imageUrl, bool resize, int? width, string targetContentType, CancellationToken token = default)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return controller.StatusCode(404);
        
        try
        {
            var imgResponse = await imageUrl.GetAsync(token);
            if (imgResponse.StatusCode is not 200)
            {
                controller.NotFound();
            }

            var image = await Image.LoadAsync(await imgResponse.GetStreamAsync(), token);
            if (width.HasValue && resize && image.Size.Width > width.Value)
            {
                // Passing height: 0 preserves aspect ratio
                image.Mutate(x => x.Resize(width.Value, 0));
            }

            using var stream = new MemoryStream();

            await image.SaveAsync(stream, GetEncoderForContentType(targetContentType), token);

            return controller.File(stream.ToArray(), targetContentType);
        }
        catch
        {
            return controller.NotFound();
        }
    }

    private static IImageEncoder GetEncoderForContentType(string contentType)
    {
        contentType = contentType.ToLowerInvariant();
            
        if (contentType.Contains("jpeg") || contentType.Contains("jpg"))
            return new JpegEncoder();

        if (contentType.Contains("bmp"))
            return new BmpEncoder();
        
        if (contentType.Contains("png"))
            return new PngEncoder();

        throw new NotSupportedException($"Unsupported image type '{contentType}'");
    }
}