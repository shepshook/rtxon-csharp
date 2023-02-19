using RtxOn.Engine.Common;
using RtxOn.Engine.Tracer;

namespace RtxOn.Engine.Objects;

public class Plane : Object3D
{
    private readonly Triangle _a;
    private readonly Triangle _b;

    public Plane(Triangle a, Triangle b)
    {
        _a = a;
        _b = b;
    }

    public Plane(Vector center, double width, double height, Material material)
    {
        var a = new Vector(center.X - width / 2, center.Y + height / 2, center.Z);
        var b = new Vector(center.X + width / 2, center.Y + height / 2, center.Z);
        var c = new Vector(center.X + width / 2, center.Y - height / 2, center.Z);
        var d = new Vector(center.X - width / 2, center.Y - height / 2, center.Z);

        _a = new Triangle(a, b, c, material);
        _b = new Triangle(a, c, d, material);
    }

    public override Color GetColor(TraceResult trace)
    {
        if (trace.Object == this) throw new InvalidOperationException("Infinite loop operation");
        return trace.Object.GetColor(trace);
    }

    public override Vector Norm(TraceResult trace)
    {
        if (trace.Object == this) throw new InvalidOperationException("Infinite loop operation");
        return trace.Object.Norm(trace);
    }

    public override TraceResult Trace(Ray ray)
    {
        var traceA = _a.Trace(ray);
        if (traceA.IsHit) return traceA;

        return _b.Trace(ray);
    }

    public override void Transform(double[,] transformation)
    {
        _a.Transform(transformation);
        _b.Transform(transformation);
    }
}
