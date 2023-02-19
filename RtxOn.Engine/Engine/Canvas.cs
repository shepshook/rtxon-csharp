using RtxOn.Engine.Common;

namespace RtxOn.Engine.Tracer;

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
                var percents = _pixelsSet / (double)_array.Length * 100d;
                Console.WriteLine($"{percents:0.0}% completed");
            }
        }
    }

    public Canvas(int width, int height)
    {
        _array = new Color[width, height];
    }

    public Color[,] ToArray2D() => _array;
}
