using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using MathLib.NumericTypes;
using WpfUtils.Views.Graphics;

namespace WpfUtils.ViewModels.Graphics
{
    public class WbRingPlotVm : WbImageVm
    {
        public const int NumCoords = 2000;
        private const int ImageSize = 500;
        private const float Center = ImageSize / 2;
        private const float RingWidth = 80;
        private static readonly float[,] scaffold;

        static WbRingPlotVm()
        {
            scaffold = new float[NumCoords, 2];

            for (var i = 0; i < NumCoords; i++)
            {
                var angle = (i/(double) NumCoords)*Math.PI*2.0;
                scaffold[i, 0] = (float) Math.Cos(angle);
                scaffold[i, 1] = (float) Math.Sin(angle);
            }
        }

        public WbRingPlotVm(int radius, Func<float, Color> colorMap)
            : base(ImageSize, ImageSize)
        {
            Radius = radius;
            _colorMap = colorMap;

            Coords = new int[NumCoords, 2, 2];
            for (var i = 0; i < NumCoords; i++)
            {
                Coords[i, 0, 0] = (int) (scaffold[i, 0] * Radius + Center);
                Coords[i, 0, 1] = (int) (scaffold[i, 1] * Radius + Center);
                Coords[i, 1, 0] = (int) (scaffold[i, 0] * (Radius + RingWidth) + Center);
                Coords[i, 1, 1] = (int) (scaffold[i, 1] * (Radius + RingWidth) + Center);
            }
        }

        private Func<float, Color> _colorMap;
        public Func<float, Color> ColorMap
        {
            get { return _colorMap; }
            set
            {
                _colorMap = value;
                RefreshPlotLines();
            }
        }

        public void AddValues(IEnumerable<D1Val<float>> ringVals)
        {
            _ringVals = ringVals.ToList();
            RefreshPlotLines();
        }

        public float Radius { get; set; }

        private readonly int[,,] Coords;

        private List<D1Val<float>> _ringVals;

        void RefreshPlotLines()
        {
            PlotRectangleList.Clear();

            PlotLineList = _ringVals.Select(

                gv =>
                    new PlotLine
                       (
                            x1: Coords[gv.Index, 0, 0],
                            y1: Coords[gv.Index, 0, 1],
                            x2: Coords[gv.Index, 1, 0],
                            y2: Coords[gv.Index, 1, 1],
                            color: ColorMap.Invoke(gv.Value)
                       )
                    ).ToList();

            OnPropertyChanged("PlotLines");

        }
    }
}
