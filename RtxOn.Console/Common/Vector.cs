namespace RtxOn.Console.Common;

public record Vector(double X, double Y, double Z)
{
    public double Length => (double)Math.Sqrt(X * X + Y * Y + Z * Z);
}

public static class VectorExtensions
{
    public static Vector Sum(this Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vector Sub(this Vector a, Vector b) => new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static double Dot(this Vector a, Vector b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    public static Vector Cross(this Vector a, Vector b) =>
        new Vector(a.Y * b.Z - a.Z * b.Y, -a.X * b.Z + a.Z * b.X, a.X * b.Y - a.Y * b.X);

    public static Vector Multiply(this Vector vector, double coefficient) => 
        new Vector(vector.X * coefficient, vector.Y * coefficient, vector.Z * coefficient);

    public static double Cos(this Vector a, Vector b) =>
        Dot(a, b) / a.Length / b.Length;

    public static Vector ToUnit(this Vector vector)
    {
        var length = vector.Length;
        return new Vector(vector.X / length, vector.Y / length, vector.Z / length);
    }

    public static Vector Reflect(this Vector vector, Vector norm)
    {
        var normalizedNorm = norm.ToUnit();
        return vector.Sub(norm.Multiply(2 * Dot(vector, normalizedNorm)));
    }
}