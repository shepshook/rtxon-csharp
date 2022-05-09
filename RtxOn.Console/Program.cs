using RtxOn.Console.Common;
using RtxOn.Console.Engine;
using RtxOn.Console.Loader;
using RtxOn.Console.Objects;
using SkiaSharp;

const int Width = 1280;
const int Height = 768;

var objects = new Object3D[]
{
    new Sphere(new Vector(Width / 2 - 700, 600, 2000), 500, new Color(30, 30, 200)),
    new Sphere(new Vector(1200, 900, 2500), 500, new Color(150, 50, 50)),
    new Triangle(new Vector(300, 300, 1300), new Vector(0, -100, 2200), new Vector(1500, -100, 2600), new Color(231, 84, 128))
};

var loader = new LoaderFactory().CreateLoader("Resources/Burger.obj");
var mesh = loader.LoadMesh();

var lights = new Light[] 
{ 
    new Light(new Vector(100, -5000, -500), 1),
    new Light(new Vector(640, 384, 0), 0.7) 
};

var scene = new Scene(Width, Height, mesh.Cast<Object3D>().ToArray(), lights);

var now = DateTime.Now;
var pixels = Engine.Render(scene);
Console.WriteLine($"Render took {(DateTime.Now - now).TotalSeconds}s");

var bitmap = new SKBitmap(Width, Height);        
for (var x = 0; x < Width; x++)
{
    for (var y = 0; y < Height; y++)
    {
        var color = pixels[x, y];
        bitmap.SetPixel(x, y, new SKColor((byte)color.Red, (byte)color.Green, (byte)color.Blue));
    }
}

using var image = SKImage.FromBitmap(bitmap);
using var data = image.Encode(SKEncodedImageFormat.Png, 100);
using var stream = File.OpenWrite("image.png");
data.SaveTo(stream); 