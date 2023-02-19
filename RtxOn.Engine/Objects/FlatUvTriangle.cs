using RtxOn.Engine.Common;
using RtxOn.Engine.Tracer;

namespace RtxOn.Engine.Objects
{
    public class FlatUvTriangle : Triangle
    {
        private readonly (double U, double V) _aMap;
        private readonly (double U, double V) _bMap;
        private readonly (double U, double V) _cMap;

        public FlatUvTriangle(Vector[] vertices, (double, double)[] uvs, Material material)
            : base(vertices[0], vertices[1], vertices[2], material)
        {
            _aMap = uvs[0];
            _bMap = uvs[1];
            _cMap = uvs[2];
        }

        public override TraceResult Trace(Ray ray)
        {
            const double Eps = 10e-3;

            var sa = _a.Sub(ray.Start);
            var ab = _b.Sub(_a);
            var ac = _c.Sub(_a);

            var denom = CalculateDeterminant3(new double[,]
            {
                { ab.X, ac.X, ray.Direction.X },
                { ab.Y, ac.Y, ray.Direction.Y },
                { ab.Z, ac.Z, ray.Direction.Z }
            });

            var ca = ac.Multiply(-1);
            var det1 = CalculateDeterminant3(new double[,]
            {
                { sa.X, ca.X, ray.Direction.X },
                { sa.Y, ca.Y, ray.Direction.Y },
                { sa.Z, ca.Z, ray.Direction.Z }
            });
            var alpha = det1 / denom;

            if (alpha < 0)
            {
                return TraceResult.NoHit();
            }

            var ba = ab.Multiply(-1);
            var det2 = CalculateDeterminant3(new double[,]
            {
                { ba.X, sa.X, ray.Direction.X },
                { ba.Y, sa.Y, ray.Direction.Y },
                { ba.Z, sa.Z, ray.Direction.Z }
            });
            var beta = det2 / denom;

            if (beta < 0 || alpha + beta > 1)
            {
                return TraceResult.NoHit();
            }

            var det3 = CalculateDeterminant3(new double[,]
            {
                { ba.X, ca.X, sa.X },
                { ba.Y, ca.Y, sa.Y },
                { ba.Z, ca.Z, sa.Z }
            });
            var t = det3 / denom;

            if (t < Eps)
            {
                return TraceResult.NoHit();
            }

            var hitPoint = ray.Start.Sum(ray.Direction.Multiply(t));

            return TraceResult.Hit(this, hitPoint, t);
        }

        public override Color GetColor(TraceResult trace)
        {
            var uvPoint = InterpolatePoint(trace.HitPoint);
            return _material.Texture.GetColor(uvPoint.Item1, uvPoint.Item2);
        }

        private (double U, double V) InterpolatePoint(Vector point)
        {
            var ab = _b.Sub(_a);
            var bc = _c.Sub(_b);
            var ac = _c.Sub(_a);

            var n = _norm.ToUnit();
            var area = 1d / 2d * ab.Cross(bc).Dot(n);

            var alpha = 1d / 2d * bc.Cross(point.Sub(_b)).Dot(n) / area;
            var beta = 1d / 2d * ac.Cross(point.Sub(_c)).Dot(n) / area;

            var u = alpha * _aMap.U + beta * _bMap.U + (1 - alpha - beta) * _cMap.U;
            var v = alpha * _aMap.V + beta * _bMap.V + (1 - alpha - beta) * _cMap.V;

            return (u, v);
        }

        private double CalculateDeterminant3(double[,] m)
        {
            var det1 = m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1];
            var det2 = m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0];
            var det3 = m[1, 0] * m[2, 1] - m[2, 0] * m[1, 1];

            return m[0, 0] * det1 - m[0, 1] * det2 + m[0, 2] * det3;
        }
    }
}