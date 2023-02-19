using RtxOn.Engine.Common;
using RtxOn.Engine.Objects;

namespace RtxOn.Engine.Tracer;

public class TraceResult
{
    public bool IsHit { get; init; }

    public Vector HitPoint { get; init; }

    public double Distance { get; init; }

    public Object3D Object { get; init; }

    public Color Color { get; init; }

    public Vector Norm { get; init; }

    private TraceResult()
    { }

    public static TraceResult Hit(Object3D @object, Vector hitPoint, double distance) =>
        new TraceResult
        {
            IsHit = true,
            HitPoint = hitPoint,
            Distance = distance,
            Object = @object
        };

    public static TraceResult NoHit() => new TraceResult { IsHit = false };
}
