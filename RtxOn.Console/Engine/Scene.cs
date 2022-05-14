using RtxOn.Console.Common;
using RtxOn.Console.Objects;

namespace RtxOn.Console.Engine;

public class Scene
{
    public List<Object3D> Objects { get; set; }
    public List<Light> Lights { get; set; }
    public int HeightPixels { get; } = 1280;
    public int WidthPixels { get; } = 768;
    public PerspectiveCamera Camera { get; set; }
    public Color BackgroundColor { get; set; } = new Color(20, 20, 30);

    public Scene(IEnumerable<Object3D> objects, IEnumerable<Light> lights, PerspectiveCamera camera)
    {
        Objects = objects.ToList();
        Lights = lights.ToList();
        Camera = camera;
    }

    public Color Trace(Ray ray, double power = 1)
    {
        var result = TraceObjects(ray);
        if (!result.IsHit) return BackgroundColor;
        
        var surfaceColor = ComputeSurfaceColor(ray, result);
        var reflectionColor = TraceReflection(ray, result, power);

        return surfaceColor.Sum(reflectionColor).Multiply(power);
    }

    public Color TraceReflection(Ray ray, TraceResult trace, double power)
    {
        if (power < 0.1)
        {
            return new Color(0, 0, 0);
        }

        var reflectedDirection = ray.Direction.Reflect(trace.Object.Norm(trace));
        var reflectedRay = new Ray(trace.HitPoint, reflectedDirection);

        return Trace(reflectedRay, 0.1 * power);
    }

    private Color ComputeSurfaceColor(Ray ray, TraceResult trace)
    {
        var sumLights = 0d;
        foreach (var light in Lights)
        {
            if (IsInShadow(trace, light)) continue;

            const int lightPowerFactor = 1_000_000;
            var lightPower = light.Power / (trace.Distance * trace.Distance) * lightPowerFactor;

            var lightToHitPoint = light.Position.Sub(trace.HitPoint);
            var hitPointNorm = trace.Object.Norm(trace);
            var lightAngleCos = lightToHitPoint.Cos(hitPointNorm);
            sumLights += lightPower * lightAngleCos * 0.5;

            var hitPointToLightDirection = trace.HitPoint.Sub(light.Position).ToUnit();
            var reflection = hitPointToLightDirection.Reflect(hitPointNorm).ToUnit();
            var phongCos = -1 * reflection.Dot(ray.Direction);

            if (phongCos > 0)
            {
                var phongFactor = Math.Pow(phongCos, 20);
                sumLights += lightPower * phongFactor;
            }
        }

        var color = trace.Object.GetColor(trace);
        return color.Multiply(sumLights);
    }

    private bool IsInShadow(TraceResult trace, Light light)
    {
        var ray = Ray.CreateByTwoPoints(light.Position, trace.HitPoint);

        var shadowTrace = TraceObjects(ray);
        // if (!shadowTrace.IsHit || shadowTrace.Object == trace.Object) return false;

        const double pointsEqualityThreshold = 0.1d;
        return !shadowTrace.IsHit || shadowTrace.HitPoint.Sub(trace.HitPoint).Length > pointsEqualityThreshold;
    }

    // todo
    private TraceResult TraceObjects(Ray ray)
    {
        var traceResults = new List<TraceResult>();

        foreach (var @object in Objects)
        {
            var objectTraceResult = @object.Trace(ray);
            if (!objectTraceResult.IsHit) continue;

            // todo: add collision conditions
            if (ray.Direction.Cos(objectTraceResult.HitPoint.Sub(ray.Start)) > 0
                && objectTraceResult.HitPoint.Sub(ray.Start).Length > 1)
            {

                traceResults.Add(objectTraceResult);
            }
        }

        return traceResults.Any() 
            ? traceResults.MinBy(x => x.Distance) 
            : TraceResult.NoHit();
    }
}
