using RtxOn.Engine.Common;
using RtxOn.Engine.Tracer;

namespace RtxOn.Engine.Objects;

public class CompoundObject : Object3D
{
    private readonly List<Object3D> _objects;

    public CompoundObject(IEnumerable<Object3D> objects)
    {
        _objects = objects.ToList();
    }

    public override Color GetColor(TraceResult trace)
    {
        if (trace.Object == this)
        {
            throw new InvalidOperationException("Infinite loop operation");
        }

        return trace.Object.GetColor(trace);
    }

    public override Vector Norm(TraceResult trace)
    {
        return trace.Object.Norm(trace);
    }

    public override TraceResult Trace(Ray ray)
    {
        var traceResults = new List<TraceResult>();

        foreach (var @object in _objects)
        {
            var objectTraceResult = @object.Trace(ray);
            if (!objectTraceResult.IsHit) continue;

            // todo: add collision conditions
            if (ray.Direction.Cos(objectTraceResult.HitPoint.Sub(ray.Start)) > 0
                && objectTraceResult.HitPoint.Sub(ray.Start).Length > 1)
            {

                traceResults.Add(objectTraceResult);
            }
        }

        return traceResults.Any()
            ? traceResults.MinBy(x => x.Distance)
            : TraceResult.NoHit();
    }

    public override void Transform(double[,] transformation)
    {
        foreach (var @object in _objects)
        {
            @object.Transform(transformation);
        }
    }
}
