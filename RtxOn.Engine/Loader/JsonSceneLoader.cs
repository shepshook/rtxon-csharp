using RtxOn.Engine.Common;
using RtxOn.Engine.Tracer;
using RtxOn.Engine.Objects;
using System.Dynamic;

namespace RtxOn.Engine.Loader;

public class JsonSceneLoader
{
    private IEnumerable<Object3D> _objects;
    private PerspectiveCamera _camera;
    private Dictionary<Guid, Material> _materials = new();
    private Dictionary<Guid, List<Guid>> _objectMaterialsMap = new();
    private List<PointLight> _lights = new();
    private Dictionary<Guid, double[,]> _transformations = new();

    public Scene LoadScene(ImportScene importScene)
    {
        LoadHierarchy(importScene.Object);

        _materials = importScene.Materials.Select(LoadMaterial).ToDictionary(k => k.Item1, v => v.Item2);
        _objects = importScene.Geometries.Select(LoadGeometry).Where(x => x != null);

        return new Scene(_objects, _lights, _camera);
    }

    private (Guid, Material) LoadMaterial(dynamic item)
    {
        var material = new Material
        {
            DiffuseColor = Color.FromInt((int) item.color)
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

            if (((IDictionary<string, object>)item).ContainsKey("children"))
            {
                var children = item.children as IEnumerable<dynamic>;
                children.ToList().ForEach(queue.Enqueue);
            }

            var id = Guid.Parse(item.uuid as string);
            var transformArray = (item.matrix as IEnumerable<object>).Select(Convert.ToDouble);
            var transformation = new double[4, 4];

            var i = 0;
            foreach (var chunk in transformArray.Chunk(4))
            {
                var list = chunk.ToList();
                transformation[0, i] = list[0];
                transformation[1, i] = list[1];
                transformation[2, i] = list[2];
                transformation[3, i] = list[3];
                i++;
            }

            var type = item.type as string;
            if (type == "Mesh") 
            { 
                id = Guid.Parse(item.geometry as string); 
            }

            _transformations[id] = transformation;


            if (type == "PointLight")
            {
                _lights.Add(LoadLight(item));
            }

            if (type == "PerspectiveCamera")
            {
                _camera = LoadCamera(item);
            }

            if (type == "Mesh")
            {
                if (item.material is string materialId)
                {
                    _objectMaterialsMap.Add(id, new List<Guid> { Guid.Parse(materialId) });
                }
                else
                {
                    var materialIds = (item.material as IEnumerable<object>).Select(x => x.ToString());
                    _objectMaterialsMap.Add(id, materialIds.Select(Guid.Parse).ToList());
                }
            }
        }
    }

    private PointLight LoadLight(dynamic item)
    {
        var id = Guid.Parse(item.uuid as string);
        double intensity = item.intensity;
        var position = new Vector(0, 0, 0);

        var light = new PointLight(position, intensity * 10);
        light.Transform(_transformations[id]);

        return light;
    }

    private PerspectiveCamera LoadCamera(dynamic item)
    {
        var id = Guid.Parse(item.uuid as string);

        var position = new Vector(0, 0, 0);

        var camera = new PerspectiveCamera(position, -2 * ((double)item.near), (double)item.fov);

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
            "BufferGeometry" => LoadComplexObject(geometry),
            _ => null
        };
    }

    private Sphere LoadSphere(dynamic geometry)
    {
        var id = Guid.Parse(geometry.uuid as string);

        var center = new Vector(0, 0, 0);
        double radius = geometry.radius;

        var material = _materials[_objectMaterialsMap[id].First()];

        var transformation = _transformations[id];
        var sphere = new Sphere(center, radius, material);
        sphere.Transform(transformation);

        return sphere;
    }

    private Plane LoadPlane(dynamic geometry)
    {
        var id = Guid.Parse(geometry.uuid as string);

        var center = new Vector(0, 0, 0);
        double width = geometry.width;
        double height = geometry.height;

        var material = _materials[_objectMaterialsMap[id].First()];

        var transformation = _transformations[id];
        var plane = new Plane(center, width, height, material);
        plane.Transform(transformation);

        return plane;
    }

    private CompoundObject LoadComplexObject(dynamic geometry)
    {
        var id = Guid.Parse(geometry.uuid as string);

        var verticesCoords = (geometry.data.attributes.position.array as IEnumerable<object>).Select(Convert.ToDouble);
        var vertices = verticesCoords.Chunk(3).Select(chunk => new Vector(chunk.First(), chunk.Second(), chunk.Third())).ToList();

        var normalsCoords = (geometry.data.attributes.normal.array as IEnumerable<object>).Select(Convert.ToDouble);
        var normals = normalsCoords.Chunk(3).Select(chunk => new Vector(chunk.First(), chunk.Second(), chunk.Third())).ToList();

        var triangleGroups = new List<(int start, int count, int materialIndex)>();
        if (!((IDictionary<string, object>) geometry.data).ContainsKey("groups"))
        {
            triangleGroups.Add((0, vertices.Count(), 0));
        }
        else
        {
            triangleGroups = (geometry.data.groups as List<dynamic>).Select(x => ((int)x.start, (int)x.count, (int)x.materialIndex)).ToList();
        }

        var triangles = new List<Triangle>();
        foreach (var group in triangleGroups)
        {
            var materialIds = _objectMaterialsMap[id];
            var material = _materials[materialIds[group.materialIndex]];
            triangles.AddRange(vertices.Skip(group.start).Take(group.count).Chunk(3).Select(chunk => Triangle.Create(chunk.Reverse(), material)));
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
    public IEnumerable<ExpandoObject> Geometries { get; set; }

    public IEnumerable<ExpandoObject> Materials { get; set; }

    public ExpandoObject Object { get; set; }
}
