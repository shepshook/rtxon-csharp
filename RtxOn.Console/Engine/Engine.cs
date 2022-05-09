using RtxOn.Console.Common;
using SkiaSharp;

namespace RtxOn.Console.Engine;

public sealed class Engine
{
    public static Color[,] Render(Scene scene)
    {
        var canvas = new Canvas(scene.Width, scene.Height);

        Parallel.For(0, scene.Width, x =>
            Parallel.For(0, scene.Height, y =>
            {
                var ray = Ray.CreateByTwoPoints(scene.CameraPosition, new Vector(x, y, scene.FocusDistance));
                var pixel = scene.Trace(ray);

                canvas[x, y] = pixel;
            })
        );

        return canvas.ToArray2D();
    }
}
