using RtxOn.Engine.Common;
using RtxOn.Engine.Tracer;
using RtxOn.Engine.Loader;
using RtxOn.Engine.Objects;
using SkiaSharp;

var Height = 450;
var Width = 800;

var objects = new Object3D[]
{
    new Sphere(new Vector(200, 175, 500), 70, new Material { DiffuseColor = Color.CreateRed() }),
    new Sphere(new Vector(400, 175, 500), 70, new Material { DiffuseColor = Color.CreateGreen() }),
    new Sphere(new Vector(600, 175, 500), 70, new Material { DiffuseColor = Color.CreateBlue() }),
    //new Sphere(new Vector(1200, 900, 2500), 500, new Material { DiffuseColor = new Color(150, 50, 50) }),
    //new Triangle(new Vector(100, 100, 500), new Vector(600, 200, 500), new Vector(100, 300, 600), new Material { DiffuseColor = new Color(231, 84, 128) }),
};
var plane = new Plane(new Vector(0, 0, 0), 300, 300, new Material { DiffuseColor = new Color(150, 50, 50) });
plane.Transform(new double[4, 4]
{
    { 0.847098126349221, -0.0513003496140538, 0.5289546657938797, 0  },
    { 0.4114467568802741, 0.6932743571590214, -0.5916774729178474, 0 },
    { -0.3363574446750715, 0.7188455604893317, 0.6083787714708944, 0 },
    { 400, 225, 500, 1 }
});
var loader = new ObjectLoaderFactory().CreateLoader("Resources/pyramid.obj");
var mesh = loader.LoadMesh();
mesh.ForEach(x => x.Transform(new double[4, 4]
{
    { 200, 0, 0, 0 },
    { 0, 200, 0, 0 },
    { 0, 0, 200, 0 },
    { 50, -50, 200, 1 }
}));

var lights = new PointLight[]
{
    new PointLight(new Vector(100, 5000, -500), 1e6),
    new PointLight(new Vector(840, 600, 300), 7e5)
};

var cameraPosition = new Vector(400, 225, 0);
var scene = new Scene(mesh, lights, new PerspectiveCamera(cameraPosition, 10, 50));

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
using var stream = File.OpenWrite("pyramid.png");
data.SaveTo(stream);
