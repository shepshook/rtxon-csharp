using RtxOn.Engine.Common;

namespace RtxOn.Engine.Tracer;

public class PerspectiveCamera
{
    public const int WidthPixels = 1620;
    public const int HeightPixels = 1050;

    public Vector Position { get; private set; }
    public Vector Destination { get; private set; }

    public double Near { get; private set; }
    public double VerticalFov { get; private set; }
    public List<(int X, int Y, Vector Vector)> Grid { get; private set; }

    public PerspectiveCamera(Vector position, double near, double vFovDegrees)
    {
        Position = position;
        Near = near;
        VerticalFov = vFovDegrees * Math.PI / 180;
        Destination = Position.Sum(new Vector(0, 0, near));
        Grid = CalculateGrid();
    }

    private List<(int, int, Vector)> CalculateGrid()
    {
        var hFov = 2 * Math.Atan(Math.Tan(VerticalFov / 2) * Math.Pow((double)WidthPixels / HeightPixels, 1.8));

        var direction = Destination.Sub(Position);
        var left = direction.RotateY(hFov / 2).Sum(Position).X;
        var right = direction.RotateY(-hFov / 2).Sum(Position).X;

        var top = direction.RotateX(VerticalFov / 2).Sum(Position).Y;
        var bottom = direction.RotateX(-VerticalFov / 2).Sum(Position).Y;

        var grid = new List<(int, int, Vector)>();

        var xInt = 0;
        foreach (var x in SplitInto(left, right, WidthPixels))
        {
            var yInt = HeightPixels - 1;
            foreach (var y in SplitInto(bottom, top, HeightPixels))
            {
                grid.Add((xInt, yInt, new Vector(x, y, Destination.Z)));
                yInt--;
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
        if (end <= start)
        {
            (end, start) = (start, end);
        }

        var step = (end - start) / count;

        for (var i = 0; i < count; i++)
        {
            yield return start + step;
            start += step;
        }
    }
}
