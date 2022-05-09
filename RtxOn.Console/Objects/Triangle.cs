using RtxOn.Console.Common;
using RtxOn.Console.Engine;

namespace RtxOn.Console.Objects;

public class Triangle : Object3D
{
    protected readonly Vector _a;
    protected readonly Vector _b;
    protected readonly Vector _c;
    protected readonly Vector _norm;
    private readonly double _d;

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

    public override Color GetColor(Vector point) => _color;

    public override Vector Norm(Vector point) => _norm;

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

    private bool IsFrontFacingRay(Vector hitPoint) => 
        hitPoint.Sub(_a).Cross(_b.Sub(_a)).Dot(_norm) >= 0
        && hitPoint.Sub(_b).Cross(_c.Sub(_b)).Dot(_norm) >= 0
        && hitPoint.Sub(_c).Cross(_a.Sub(_c)).Dot(_norm) >= 0;

    private bool IsSameClockDirection(Vector a, Vector b, Vector norm)
    {
        var normAB = b.Cross(a);
        return normAB.Dot(norm) >= 0;
    }
}
