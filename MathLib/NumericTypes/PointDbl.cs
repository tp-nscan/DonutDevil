using MathLib.Intervals;

namespace MathLib.NumericTypes
{
    /// <summary>
    /// The point class
    /// </summary>
    public struct PointDbl
    {
        private readonly double _y;
        private readonly double _x;

        public PointDbl(double x, double y)
            : this()
        {
            _y = y;
            _x = x;
        }

        public double Y
        {
            get { return _y; }
        }

        public double X
        {
            get { return _x; }
        }

        static readonly PointDbl origin = new PointDbl(0, 0);
        public static PointDbl Origin { get { return origin; } }
    }

    public static class PointDblExt
    {

        public static PointDbl Add(this PointDbl lhs, PointDbl rhs)
        {
            return new PointDbl
            (
                x: (lhs.X + rhs.X),
                y: (lhs.Y + rhs.Y)
            );
        }

        public static PointDbl Subtract(this PointDbl lhs, PointDbl rhs)
        {
            return new PointDbl
            (
                y: (lhs.Y - rhs.Y),
                x: (lhs.X - rhs.X)
            );
        }

    }

}
