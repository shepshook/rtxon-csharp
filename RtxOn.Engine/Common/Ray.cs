namespace RtxOn.Engine.Common;

public class Ray
{
    public Vector Start { get; }

    public Vector Direction { get; }

    public Ray(Vector start, Vector direction)
    {
        Start = start;
        Direction = direction;
    }

    public static Ray CreateByTwoPoints(Vector from, Vector to)
    {
        var direction = to.Sub(from).ToUnit();
        return new Ray(from.Sum(direction.Multiply(0.1)), direction);
    }
}