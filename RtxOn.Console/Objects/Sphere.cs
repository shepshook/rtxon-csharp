using RtxOn.Console.Common;
using RtxOn.Console.Engine;

namespace RtxOn.Console.Objects;

public class Sphere : Object3D
{
    private Vector _center;
    private double _radius;
    private Color _color;

    public Sphere(Vector center, double radius, Color color)
    {
        _center = center;
        _radius = radius;
        _color = color;
    }

    public override Color GetColor(Vector point) => _color;

    public override Vector Norm(Vector point)
    {
        var pointToCenter = point.Sub(_center);
        return pointToCenter.ToUnit();
    }

    public override TraceResult Trace(Ray ray)
    {
        const double Eps = 1e-5;

        var l = ray.Direction.X;
        var m = ray.Direction.Y;
        var n = ray.Direction.Z;

        var k = (ray.Start.X - _center.X);
        var p = (ray.Start.Y - _center.Y);
        var f = (ray.Start.Z - _center.Z);

        var a = l * l + m * m + n * n;
        var b = 2 * (k * l + p * m + f * n);
        var c = k * k + p * p + f * f - _radius * _radius;

        var D = b * b - 4 * a * c;

        if (D < 0) return TraceResult.NoHit();

        var sqrt_D = Math.Sqrt(D);
        var t1 = (-b + sqrt_D) / (2 * a);
        var t2 = (-b - sqrt_D) / (2 * a);

        var min_t = (t1 < t2) ? t1 : t2;
        var max_t = (t1 > t2) ? t1 : t2;

        var t = (min_t > Eps) ? min_t : max_t;

        if (t < Eps) return TraceResult.NoHit();

        var point = new Vector(l * t + ray.Start.X, m * t + ray.Start.Y, n * t + ray.Start.Z);
        var distance = point.Sub(ray.Start).Length;

        return TraceResult.Hit(this, point, distance);
    }
}