using Microsoft.AspNetCore.Mvc;
using RtxOn.Engine.Tracer;
using RtxOn.Engine.Common;
using RtxOn.Engine.Loader;
using SkiaSharp;

namespace RtxOn.Mvc.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RenderController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(byte[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> Render(ImportScene importScene)
    {
        var loader = new JsonSceneLoader();
        var scene = loader.LoadScene(importScene);

        var pixels = Engine.Tracer.Engine.Render(scene);
        return File(ToImage(pixels), "image/png");
    }

    private byte[] ToImage(Color[,] pixels)
    {
        var width = pixels.GetLength(0);
        var height = pixels.GetLength(1);

        var bitmap = new SKBitmap(width, height);
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var color = pixels[x, y];
                bitmap.SetPixel(x, y, new SKColor((byte)color.Red, (byte)color.Green, (byte)color.Blue));
            }
        }

        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }
}
