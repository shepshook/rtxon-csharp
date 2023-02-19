using RtxOn.Engine.Common;
using RtxOn.Engine.Tracer;

namespace RtxOn.Engine.Objects;

public abstract class Object3D
{
    public abstract TraceResult Trace(Ray ray);

    public abstract Color GetColor(TraceResult trace);

    public abstract Vector Norm(TraceResult trace);

    public abstract void Transform(double[,] transformation);
}
