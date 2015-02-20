using System.Windows.Media;

namespace WpfUtils.BitmapGraphics
{
    public struct GraphicsInfo
    {
        private readonly int _x;
        private readonly int _y;
        private readonly Color _color;

        public GraphicsInfo(int x, int y, Color color) : this()
        {
            _x = x;
            _y = y;
            _color = color;
        }

        public int X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }

        public Color Color
        {
            get { return _color; }
        }

    }

    public static class GraphicsEx
    {
        public static uint ToUint(this Color c)
        {
            return (uint) c.A << 24 | ((uint) c.R << 16) | ((uint) c.G << 8) | c.B;
        }

        public static int ToInt(this Color c)
        {
            return c.A << 24 | (c.R << 16) | (c.G << 8) | c.B;
        }
    }
}
