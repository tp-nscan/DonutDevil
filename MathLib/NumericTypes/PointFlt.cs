using System;
using System.Collections.Generic;
using System.Linq;

namespace MathLib.NumericTypes
{
    public class PointFlt
    {
        private readonly float _y;
        private readonly float _x;

        public PointFlt(float x, float y)
        {
            _y = y;
            _x = x;
        }

        public float Y
        {
            get { return _y; }
        }

        public float X
        {
            get { return _x; }
        }

        static readonly PointDbl origin = new PointDbl(0.0, 0.0);

        public static PointDbl Origin { get { return origin; } }
    }

    public static class PointFext
    {
        public static IEnumerable<PointFlt> ToPoints(this IEnumerable<float> xVals, IEnumerable<float> yVals)
        {
            return xVals.Zip(yVals, (x, y) => new PointFlt(x, y));
        }

        public static float[] ModEuclideanForce(this PointFlt target, PointFlt draw, float tentParam)
        {
            var dX = target.X.MfDelta(draw.X);
            var dY = target.Y.MfDelta(draw.Y);

            var dSquared = (dX*dX + dY*dY);
            var pull = dSquared.Tent(tentParam);

            if (Math.Abs(dSquared) < Mf.Epsilon)
            {
                return new[] { 0.0f, 0.0f };
            }

            return new[] { dX * Math.Abs(dX) * pull / dSquared, dY * Math.Abs(dY) * pull / dSquared };
        }


        public static PointFlt FieldUpdate(this PointFlt target, IEnumerable<PointFlt> draws, float step)
        {
            var wh = draws.Select
            (
                d =>
                {
                    var vDiff = target.ModEuclideanForce
                        (
                            draw: d, 
                            tentParam: 0.15f
                        );

                   return vDiff;
                }
            
            ).ToList();

            return new PointFlt
                (
                    x: target.X.MfAdd(wh.Sum(d => d[0] * step)),
                    y: target.Y.MfAdd(wh.Sum(d => d[1] * step))
                );
        }
    }


}
