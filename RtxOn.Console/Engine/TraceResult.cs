using RtxOn.Console.Common;
using RtxOn.Console.Objects;

namespace RtxOn.Console.Engine;

public class TraceResult
{
    public bool IsHit { get; init; }

    public Vector HitPoint { get; init; }

    public double Distance { get; init; }

    public Object3D Object { get; init; }

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
