using RtxOn.Engine.Common;
using RtxOn.Engine.Tracer;
using RtxOn.Engine.Loader;

namespace RtxOn.Engine.Objects;

public class Triangle : Object3D
{
    protected Vector _a;
    protected Vector _b;
    protected Vector _c;
    protected Vector _norm;
    private double _d;

    protected readonly Material _material;

    public Triangle(Vector a, Vector b, Vector c, Material material)
    {
        _a = a;
        _b = b;
        _c = c;
        _material = material;
        _norm = c.Sub(a).Cross(b.Sub(c));
        _d = -1 * _norm.Dot(_a);
    }

    public static Triangle Create(IEnumerable<Vector> points, Material material)
    {
        return new Triangle(points.First(), points.Second(), points.Third(), material);
    }

    public override Color GetColor(TraceResult trace) => _material.DiffuseColor;

    public override Vector Norm(TraceResult trace) => _norm;

    public override TraceResult Trace(Ray ray)
    {
        const double Eps = 1e-5;

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
