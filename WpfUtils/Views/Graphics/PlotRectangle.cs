using System.Windows.Media;

namespace WpfUtils.Views.Graphics
{
    public class PlotRectangle
    {
        private readonly int _x;
        private readonly int _y;
        private readonly int _width;
        private readonly int _height;
        private readonly Color _color;

        public PlotRectangle(int x, int y, int width, int height, Color color)
        {
            _x = x;
            _y = y;
            _color = color;
            _width = width;
            _height = height;
        }

        public int X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }


        public Color Color
        {
            get { return _color; }
        }

    }
}
