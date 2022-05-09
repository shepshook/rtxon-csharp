using RtxOn.Console.Common;

namespace RtxOn.Console.Engine;

public class Canvas
{
    private readonly Color[,] _array;
    private int _pixelsSet = 0;

    public Color this[int x, int y]
    {
        get
        {
            return _array[x, y];
        }
        set
        {
            _array[x, y] = value;
            _pixelsSet++;
            if (_pixelsSet % (_array.Length / 10) == 0)
            {
                var percents = _pixelsSet / (double) _array.Length * 100d;
                System.Console.WriteLine($"{percents:0.0}% completed");
            }
        }
    }

    public Canvas(int width, int height)
    {
        _array = new Color[width, height];
    }

    public Color[,] ToArray2D() => _array;
}
