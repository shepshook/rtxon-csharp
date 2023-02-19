using RtxOn.Engine.Common;

namespace RtxOn.Engine.Tracer;

public class PointLight
{
    public Vector Position { get; private set; }
    public double Power { get; private set; }

    public PointLight(Vector position, double power)
    {
        Position = position;
        Power = power;
    }

    public void Transform(double[,] transformation)
    {
        Position = Position.Transform(transformation);
    }
}
