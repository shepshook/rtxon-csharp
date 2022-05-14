using System.Dynamic;
using System.Text.Json;
using RtxOn.Console.Common;
using RtxOn.Console.Engine;
using RtxOn.Console.Extensions;
using RtxOn.Console.Objects;

namespace RtxOn.Console.Loader;

public class JsonSceneLoader
{
    private IEnumerable<Object3D> _objects;
    private PerspectiveCamera _camera;
    private Dictionary<Guid, Material> _materials = new();
    private Dictionary<Guid, List<Guid>> _objectMaterialsMap = new();
    private List<Light> _lights = new();
    private Dictionary<Guid, double[,]> _transformations = new();

    public Scene LoadScene(string json)
    {
        var importScene = JsonSerializer.Deserialize<ImportScene>(json);
        LoadHierarchy(importScene.Object);

        _materials = importScene.Materials.Select(LoadMaterial).ToDictionary(k => k.Item1, v => v.Item2);
        _objects = importScene.Geometries.Select(LoadGeometry);

        return new Scene(_objects, _lights, _camera);
    }

    private (Guid, Material) LoadMaterial(dynamic item)
    {
        var material = new Material
        {
            DiffuseColor = Color.FromInt(item.color)
        };

        return (Guid.Parse(item.uuid as string), material);
    }

    private void LoadHierarchy(dynamic hierarchy)
    {
        var queue = new Queue<dynamic>();
        queue.Enqueue(hierarchy);

        while (queue.Count > 0)
        {
            var item = queue.Dequeue();

            if (DynamicExtensions.DoesPropertyExist(item, "children"))
            {
                var children = item.children as IEnumerable<dynamic>;
                queue.Enqueue(children);
            }

            var id = Guid.Parse(item.uuid as string);
            var transformArray = item.matrix as IEnumerable<double>;
            var transformation = new double[4, 4];

            var i = 0;
            foreach (var chunk in transformArray.Chunk(4))
            {
                var list = chunk.ToList();
                transformation[i, 0] = list[0];
                transformation[i, 1] = list[1];
                transformation[i, 2] = list[2];
                transformation[i, 3] = list[3];
            }

            _transformations[id] = transformation;

            if (item.Type == "PointLight")
            {
                _lights.Add(LoadLight(item));
            }

            if (item.Type == "PerspectiveCamera")
            {
                _camera = LoadCamera(item);
            }

            if (item.Type == "Mesh")
            {
                if (item.material is string materialId)
                {
                    _objectMaterialsMap.Add(id, new List<Guid> { Guid.Parse(materialId) });
                }

                var materialIds = item.material as IEnumerable<string>;
                _objectMaterialsMap.Add(id, materialIds.Select(Guid.Parse).ToList());
            }
        }
    }

    private Light LoadLight(dynamic item)
    {
        throw new NotImplementedException();
    }

    private PerspectiveCamera LoadCamera(dynamic item)
    {
        var id = Guid.Parse(item.uuid as string);

        var position = new Vector(1, 1, 1);

        var camera = new PerspectiveCamera(position, (double)item.near, (double)item.fov);

        camera.Transform(_transformations[id]);
        return camera;
    }

    private Object3D LoadGeometry(dynamic geometry)
    {
        string type = geometry.type;
        return type switch
        {
            "SphereGeometry" => LoadSphere(geometry),
            "PlaneGeometry" => LoadPlane(geometry),
            "BufferGeometry" => LoadComplexObject(geometry)
        };
    }

    private Sphere LoadSphere(dynamic geometry)
    {
        throw new NotImplementedException();
    }

    // todo
    private Object3D LoadPlane(dynamic geometry)
    {
        throw new NotImplementedException();
    }

    private CompoundObject LoadComplexObject(dynamic geometry)
    {
        var id = Guid.Parse(geometry.uuid as string);

        var verticesCoords = geometry.data.attributes.position.array as IEnumerable<double>;
        var vertices = verticesCoords.Chunk(3).Select(chunk => new Vector(chunk.First(), chunk.Second(), chunk.Third()));

        var normalsCoords = geometry.data.attributes.normal.array as IEnumerable<double>;
        var normals = normalsCoords.Chunk(3).Select(chunk => new Vector(chunk.First(), chunk.Second(), chunk.Third()));

        var triangleGroups = geometry.data.groups as IEnumerable<(int start, int count, int materialIndex)>;

        var triangles = new List<Triangle>();
        foreach (var group in triangleGroups)
        {
            var materialIds = _objectMaterialsMap[id];
            var material = _materials[materialIds[group.materialIndex]];
            triangles.AddRange(vertices.Skip(group.start).Take(group.count).Chunk(3).Select(chunk => Triangle.Create(chunk, material)));
        }

        var transformation = _transformations[id];

        var @object = new CompoundObject(triangles.Cast<Object3D>());
        @object.Transform(transformation);

        return @object;
    }
}

public static class LinqExtensions
{
    public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
    {
        while (source.Any())
        {
            yield return source.Take(chunkSize);
            source = source.Skip(chunkSize);
        }
    }

    public static T Second<T>(this IEnumerable<T> source) => source.Skip(1).First();

    public static T Third<T>(this IEnumerable<T> source) => source.Skip(2).First();
}

public class ImportScene
{
    public IEnumerable<dynamic> Geometries { get; set; }

    public IEnumerable<dynamic> Materials { get; set; }

    public dynamic Object { get; set; }
}
