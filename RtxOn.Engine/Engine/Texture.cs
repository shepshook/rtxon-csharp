using RtxOn.Engine.Common;
using SkiaSharp;

namespace RtxOn.Engine.Tracer
{
    public abstract class Texture
    {
        public abstract Color GetColor(double u, double v);
    }

    public class PlainColorTexture : Texture
    {
        private readonly Color _color;

        public PlainColorTexture(Color color)
        {
            _color = color;
        }

        public override Color GetColor(double u, double v)
        {
            return _color;
        }
    }

    public class ImageTexture : Texture
    {
        private readonly SKPixmap _pixels;

        public ImageTexture(string fileName)
        {
            var file = new SKFileStream(fileName);
            _pixels = SKBitmap.Decode(file).PeekPixels();
        }

        public override Color GetColor(double u, double v)
        {
            var x = (int)(u * _pixels.Width);
            var y = (int)(v * _pixels.Height);
            var c = _pixels.GetPixelColor(x, y);

            return new Color(c.Red, c.Green, c.Blue);
        }
    }
}