using MathLib.NumericTypes;

namespace MathLib.Intervals
{
    public class RectFloat
    {
        private readonly float _top;
        private readonly float _left;
        private readonly float _right;
        private readonly float _bottom;

        public RectFloat(float top, float left, float bottom, float right)
        {
            _top = top;
            _left = left;
            _bottom = bottom;
            _right = right;
        }

        public float Top
        {
            get { return _top; }
        }

        public float Left
        {
            get { return _left; }
        }

        public float Right
        {
            get { return _right; }
        }

        public float Bottom
        {
            get { return _bottom; }
        }
    }

    public static class RectExt
    {
        public static float Width(this RectFloat rectFloat)
        {
            return rectFloat.Right - rectFloat.Left;
        }

        public static float Height(this RectFloat rectFloat)
        {
            return rectFloat.Top - rectFloat.Bottom;
        }

        public static PointFlt UpLeft(this RectFloat rectFloat)
        {
            return new PointFlt(rectFloat.Left, rectFloat.Top);
        }

        public static PointFlt BottomRight(this RectFloat rectFloat)
        {
            return new PointFlt(rectFloat.Right, rectFloat.Bottom);
        }

    }
}
