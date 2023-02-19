namespace RtxOn.Engine.Common;

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
        a.Dot(b) / a.Length / b.Length;

    public static Vector ToUnit(this Vector vector)
    {
        var length = vector.Length;
        return new Vector(vector.X / length, vector.Y / length, vector.Z / length);
    }

    public static Vector Reflect(this Vector vector, Vector norm)
    {
        var normalizedNorm = norm.ToUnit();
        return vector.Sub(norm.Multiply(2 * vector.Dot(normalizedNorm)));
    }

    public static Vector RotateX(this Vector vector, double angleRad)
    {
        var sin = Math.Sin(angleRad);
        var cos = Math.Cos(angleRad);

        var x = vector.X;
        var y = vector.Y * cos - vector.Z * sin;
        var z = vector.Y * sin + vector.Z * cos;

        return new Vector(x, y, z);
    }

    public static Vector RotateY(this Vector vector, double angleRad)
    {
        var sin = Math.Sin(angleRad);
        var cos = Math.Cos(angleRad);

        var x = vector.X * cos + vector.Z * sin;
        var y = vector.Y;
        var z = -vector.X * sin + vector.Z * cos;

        return new Vector(x, y, z);
    }

    public static Vector Transform(this Vector v, double[,] t)
    {
        var array = new double[3];
        for (var i = 0; i < 3; i++)
        {
            array[i] = t[i, 0] * v.X + t[i, 1] * v.Y + t[i, 2] * v.Z + t[i, 3];
        }

        var translation = new Vector(t[3, 0], t[3, 1], t[3, 2]);

        var result = Vector.Create(array);
        return result.Sum(translation);
    }
}