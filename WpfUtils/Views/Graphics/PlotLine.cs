using System.Windows.Media;

namespace WpfUtils.Views.Graphics
{
    public class PlotLine
    {
        private readonly int _x1;
        private readonly int _y1;
        private readonly int _x2;
        private readonly int _y2;
        private readonly Color _color;

        public PlotLine(int x1, int y1, int x2, int y2, Color color)
        {
            _x1 = x1;
            _y1 = y1;
            _color = color;
            _x2 = x2;
            _y2 = y2;
        }

        public int X1
        {
            get { return _x1; }
        }

        public int Y1
        {
            get { return _y1; }
        }

        public int X2
        {
            get { return _x2; }
        }

        public int Y2
        {
            get { return _y2; }
        }

        public Color Color
        {
            get { return _color; }
        }
    }
}
