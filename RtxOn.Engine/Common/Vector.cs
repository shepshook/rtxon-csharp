namespace RtxOn.Engine.Common;

public record Vector(double X, double Y, double Z)
{
    public static Vector Create(double[] source)
    {
        var x = source[0];
        var y = source[1];
        var z = source[2];

        return new Vector(x, y, z);
    }

    public double Length => (double)Math.Sqrt(X * X + Y * Y + Z * Z);
}
