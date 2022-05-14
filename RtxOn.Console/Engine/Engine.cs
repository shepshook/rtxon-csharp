using RtxOn.Console.Common;

namespace RtxOn.Console.Engine;

public sealed class Engine
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

public class PerspectiveCamera
{
    public const int WidthPixels = 1280;
    public const int HeightPixels = 768;

    public Vector Position { get; private set; }
    public Vector Destination { get; private set; }

    public double Near { get; private set; }
    public double HorizontalFov { get; private set; }
    public List<(int X, int Y, Vector Vector)> Grid { get; private set; }

    public PerspectiveCamera(Vector position, double near, double hFov)
    {
        Position = position;
        Near = near;
        HorizontalFov = hFov * Math.PI / 180;
        Destination = Position.Sum(new Vector(0, 0, near));
        Grid = CalculateGrid();
    }

    private List<(int, int, Vector)> CalculateGrid()
    {
        var vFov = 2 * Math.Atan(Math.Tan(HorizontalFov * Math.PI / 360) * HeightPixels / WidthPixels);

        var direction = Destination.Sub(Position);
        var left = direction.RotateY(-HorizontalFov / 2).Sum(Position).X;
        var right = direction.RotateY(HorizontalFov / 2).Sum(Position).X;

        var top = direction.RotateX(-vFov / 2).Sum(Position).Y;
        var bottom = direction.RotateX(vFov / 2).Sum(Position).Y;

        var grid = new List<(int, int, Vector)>();

        var xInt = 0;
        foreach (var x in SplitInto(left, right, WidthPixels))
        {
            var yInt = 0;
            foreach (var y in SplitInto(top, bottom, HeightPixels))
            {
                grid.Add((xInt, yInt, new Vector(x, y, Destination.Z)));
                yInt++;
            }
            xInt++;
        }

        return grid;
    }

    public void Transform(double[,] transformation)
    {
        Position = Position.Transform(transformation);
        Destination = Destination.Transform(transformation);

        Grid = Grid.Select(tuple => (tuple.X, tuple.Y, tuple.Vector.Transform(transformation))).ToList();
    }

    private IEnumerable<double> SplitInto(double start, double end, int count)
    {
        if (end <= start) throw new ArgumentException("End must be greater than Start");

        var step = (end - start) / count;
        
        for (var i = 0; i < count; i++)
        {
            yield return start + step;
            start += step;
        }
    }
}
