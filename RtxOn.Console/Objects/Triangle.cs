using RtxOn.Console.Common;
using RtxOn.Console.Engine;
using RtxOn.Console.Loader;

namespace RtxOn.Console.Objects;

public class Triangle : Object3D
{
    protected Vector _a;
    protected Vector _b;
    protected Vector _c;
    protected Vector _norm;
    private double _d;

    private readonly Color _color;

    public Triangle(Vector a, Vector b, Vector c, Color color = default)
    {
        _a = a;
        _b = b;
        _c = c;
        _color = color;
        _norm = c.Sub(a).Cross(b.Sub(c));
        _d = -1 * _norm.Dot(_a);
    }

    public static Triangle Create(IEnumerable<Vector> points, Material material)
    {
        return new Triangle(points.First(), points.Second(), points.Third(), material.DiffuseColor);
    }

    public override Color GetColor(TraceResult trace) => _color;

    public override Vector Norm(TraceResult trace) => _norm;

    public override TraceResult Trace(Ray ray)
    {
        const double Eps = 1e-3;

        var dot = _norm.Dot(ray.Direction);
        if (Math.Abs(dot) < Eps)
        {
            return TraceResult.NoHit();
        }

        var lambda = -1 * (_d + _norm.Dot(ray.Start)) / dot;
        var hitPoint = ray.Start.Sum(ray.Direction.Multiply(lambda));

        if (IsFrontFacingRay(hitPoint))
        {
            var distance = hitPoint.Sub(ray.Start).Length;
            return TraceResult.Hit(this, hitPoint, distance);
        }

        return TraceResult.NoHit();
    }

    public override void Transform(double[,] transformation)
    {
        _a = _a.Transform(transformation);
        _b = _b.Transform(transformation);
        _c = _c.Transform(transformation);

        _norm = _c.Sub(_a).Cross(_b.Sub(_c));
        _d = -1 * _norm.Dot(_a);
    }

    private bool IsFrontFacingRay(Vector hitPoint) =>
        hitPoint.Sub(_a).Cross(_b.Sub(_a)).Dot(_norm) >= 0
        && hitPoint.Sub(_b).Cross(_c.Sub(_b)).Dot(_norm) >= 0
        && hitPoint.Sub(_c).Cross(_a.Sub(_c)).Dot(_norm) >= 0;
}
