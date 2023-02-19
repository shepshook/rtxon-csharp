using RtxOn.Engine.Common;

namespace RtxOn.Engine.Tracer;

public static class Engine
{
    public static Color[,] Render(Scene scene)
    {
        var canvas = new Canvas(PerspectiveCamera.WidthPixels, PerspectiveCamera.HeightPixels);

        Parallel.ForEach(scene.Camera.Grid, tuple =>
        {
            var ray = Ray.CreateByTwoPoints(scene.Camera.Position, tuple.Vector);
            var pixel = scene.Trace(ray);
            canvas[tuple.X, tuple.Y] = pixel;
        });

        return canvas.ToArray2D();
    }
}
