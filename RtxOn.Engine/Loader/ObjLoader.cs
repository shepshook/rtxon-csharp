using RtxOn.Engine.Common;
using RtxOn.Engine.Tracer;
using RtxOn.Engine.Objects;

namespace RtxOn.Engine.Loader;

public class ObjLoader : IObjectLoader
{
    private readonly string _fileName;
    private readonly List<Vector> _vertices;
    private readonly List<Triangle> _triangles;
    private readonly List<(double U, double V)> _textureMaps;
    private Dictionary<string, Material> _materials;
    private string _currentMaterial;

    public ObjLoader(string fileName)
    {
        _fileName = fileName;
        _vertices = new();
        _triangles = new();
        _textureMaps = new();
    }

    public List<Triangle> LoadMesh()
    {
        foreach (var line in File.ReadLines(_fileName))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var splitString = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var command = splitString.First();
            var parameters = splitString.Skip(1).ToList();

            switch (command)
            {
                case "v":
                    _vertices.Add(LoadVertex(parameters));
                    break;

                case "vt":
                    _textureMaps.Add((Convert.ToDouble(parameters[0]), Convert.ToDouble(parameters[1])));
                    break;

                case "f":
                    var triangle = CreateTriangle(parameters);
                    _triangles.Add(triangle);
                    break;

                case "#":
                    break;

                case "mtllib":
                    var materials = LoadMaterials(parameters[0]);
                    _materials = materials.ToDictionary(k => k.Name);
                    break;

                case "usemtl":
                    _currentMaterial = parameters[0];
                    break;

                default:
                    break;
            }
        }

        Console.WriteLine($"Total triangles: {_triangles.Count}");
        return _triangles;
    }

    private Vector LoadVertex(IEnumerable<string> coordinates)
    {
        var castCoordinates = coordinates.Select(x => Convert.ToDouble(x)).ToList();
        return new Vector(
            castCoordinates[0],
            castCoordinates[1],
            castCoordinates[2]);
            //.Multiply(200).Sum(new Vector(250, 0, 500));
    }

    private Triangle CreateTriangle(IEnumerable<string> parameters)
    {
        var paramsSplit = parameters.Select(x => x.Split('/').Select(i =>
            !string.IsNullOrEmpty(i)
                ? (int?)Convert.ToInt32(i)
                : default)
            .ToList());

        var verticesIndices = paramsSplit.Select(x => x[0].Value).ToList();

        var vertices = new Vector[]
        {
            _vertices[verticesIndices[0] - 1],
            _vertices[verticesIndices[1] - 1],
            _vertices[verticesIndices[2] - 1]
        };

        var hasTextureMaps = paramsSplit.First().Count() >= 2 && paramsSplit.First()[1] != null;
        var hasNormals = paramsSplit.First().Count() == 3;

        if (hasTextureMaps)
        {
            var textureIndices = paramsSplit.Select(x => x[1].Value).ToList();

            var textureUvs = new (double, double)[]
            {
                _textureMaps[textureIndices[0] - 1],
                _textureMaps[textureIndices[1] - 1],
                _textureMaps[textureIndices[2] - 1]
            };

            return new FlatUvTriangle(vertices, textureUvs, _materials[_currentMaterial]);
        }

        return new Triangle(
            _vertices[verticesIndices[0] - 1],
            _vertices[verticesIndices[1] - 1],
            _vertices[verticesIndices[2] - 1],
            //_materials[_currentMaterial]);
            new Material { DiffuseColor = Color.CreateRed() });
    }

    private List<Material> LoadMaterials(string path)
    {
        var materials = new List<Material>(5);

        foreach (var line in File.ReadLines(path))
        {
            if (string.IsNullOrEmpty(line)) continue;

            var splitString = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var command = splitString.First();
            var parameters = splitString.Skip(1).ToList();

            switch (command)
            {
                case "newmtl":
                    materials.Add(new Material { Name = parameters[0] });
                    break;
                case "Kd":
                    var rgb = parameters.Select(Convert.ToDouble).ToList();
                    materials.Last().DiffuseColor = Color.FromNormalized(rgb[0], rgb[1], rgb[2]);
                    break;
                case "map_Kd":
                    materials.Last().Texture = new ImageTexture(parameters.First());
                    break;
                default:
                    break;
            }
        }

        return materials;
    }
}

