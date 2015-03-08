using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using WpfUtils.Utils;

namespace WpfUtils.Views.Graphics
{
    public struct PlotPoint
    {
        private readonly int _x;
        private readonly int _y;
        private readonly Color _color;

        public PlotPoint(int x, int y, Color color) : this()
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

    public static class PlotPointEx
    {
        public static IReadOnlyList<PlotPoint> TestSequence()
        {
            var x = 0;

            return ColorSequence.TriPolar(128)
                .Colors
                .Select(
                    c => new PlotPoint
                        (
                        x: x % 128,
                        y: (x++) / 128,
                        color: c
                        )).ToList();
        }

    }

}
