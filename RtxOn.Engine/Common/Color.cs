namespace RtxOn.Engine.Common;

using System;

public class Color
{
    private const int MaxValue = 255;

    private int _red;
    private int _green;
    private int _blue;

    public int Red { get => _red; private set => _red = ConstrainRgb(value); }
    public int Green { get => _green; private set => _green = ConstrainRgb(value); }
    public int Blue { get => _blue; private set => _blue = ConstrainRgb(value); }

    // From 0 to 255
    public Color(int red = 0, int green = 0, int blue = 0)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }

    // From 0 to 255
    public Color(double red, double green, double blue)
    {
        Red = (int)red;
        Green = (int)green;
        Blue = (int)blue;
    }

    // From 0 to 1
    public static Color FromNormalized(double red, double green, double blue) =>
        new Color
        {
            Red = (int)(red * MaxValue),
            Green = (int)(green * MaxValue),
            Blue = (int)(blue * MaxValue)
        };

    public static Color FromInt(int color)
    {
        var bytes = BitConverter.GetBytes(color);
        return new Color(bytes[2], bytes[1], bytes[0]);
    }

    public static Color CreateRed() =>   new Color(200, 20, 20);
    public static Color CreateGreen() => new Color(20, 200, 20);
    public static Color CreateBlue() =>  new Color(20, 20, 200);

    public Color Multiply(double factor) =>
        new Color(_red * factor, _green * factor, _blue * factor);

    public Color Sum(Color other) =>
        new Color(_red + other.Red, _green + other.Green, _blue + other.Blue);

    private static int ConstrainRgb(int value) => Math.Min(MaxValue, Math.Max(0, value));
}