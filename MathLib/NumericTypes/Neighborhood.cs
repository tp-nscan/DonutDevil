using System;
using System.Collections.Generic;

namespace MathLib.NumericTypes
{
    public static class NeighborhoodExt
    {
        public static void SetRing<T>(this Neighborhood<T> neighborhood, int ringRadius, T value)
        {
            neighborhood.SetValue(ringRadius, 0, value);

            neighborhood.SetValue(-ringRadius, 0, value);

            neighborhood.SetValue(0, ringRadius, value);

            neighborhood.SetValue(0, -ringRadius, value);

            for (var i = 1; i <= ringRadius; i++)
            {
                neighborhood.SetValue(ringRadius,  i, value);
                neighborhood.SetValue(ringRadius, -i, value);

                neighborhood.SetValue(-ringRadius,  i, value);
                neighborhood.SetValue(-ringRadius, -i, value);

                neighborhood.SetValue( i, ringRadius, value);
                neighborhood.SetValue(-i, ringRadius, value);

                neighborhood.SetValue( i, -ringRadius, value);
                neighborhood.SetValue(-i, -ringRadius, value);
            }
        }

        public static Neighborhood<T> ApplyRadiusFunc<T>(this Neighborhood<T> neighborhood, Func<double, T> radiusFunc)
        {
             for(var i=0; i< neighborhood.Radius + 1; i++)
            {
                 for(var j=0; j< neighborhood.Radius + 1; j++)
                 {
                     var fVal = radiusFunc(Math.Sqrt(i*i + j*j));

                     neighborhood.SetValue( i,  j, fVal);
                     neighborhood.SetValue(-i,  j, fVal);
                     neighborhood.SetValue( i, -j, fVal);
                     neighborhood.SetValue(-i, -j, fVal);

                     neighborhood.SetValue( j,  i, fVal);
                     neighborhood.SetValue(-j,  i, fVal);
                     neighborhood.SetValue( j, -i, fVal);
                     neighborhood.SetValue(-j, -i, fVal);
                 }
            }
            return neighborhood;
        }

        public static IEnumerable<D2Val<T>> ToPairStream<T>(this Neighborhood<T> neighborhood)
        {
            for (var i = 0; i < neighborhood.Radius * 2 + 1; i++)
            {
                for (var j = 0; j < neighborhood.Radius * 2 + 1; j++)
                {
                    yield return new D2Val<T>(i, j, neighborhood.GetValue(i -neighborhood.Radius,j- neighborhood.Radius));
                }
            }
        }

        public static Neighborhood<double> CircularGlauber(int radius, double freq, double decay)
        {
            return new Neighborhood<double>(radius).ApplyRadiusFunc(r=>Math.Exp(-decay*r)*Math.Cos(freq*r));
        }
    }

    public class Neighborhood<T>
    {
        public Neighborhood(int radius)
        {
            _radius = radius;
            _values = new T[radius*2+1, radius*2+1];
        }

        private readonly int _radius;
        public int Radius
        {
            get { return _radius; }
        }

        public void SetValue(int xOff, int yOff, T value)
        {
            _values[Radius + xOff, Radius + yOff] = value;
        }

        public T GetValue(int xOff, int yOff)
        {
           return _values[Radius + xOff, Radius + yOff];
        }

        public IEnumerable<T> ToReadingOrder
        {
            get { return _values.ToReadingOrder(); }
        }

        private readonly T[,] _values;
    }
}
