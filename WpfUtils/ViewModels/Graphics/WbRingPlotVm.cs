using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using MathLib;
using MathLib.NumericTypes;
using WpfUtils.Views.Graphics;

namespace WpfUtils.ViewModels.Graphics
{
    public class WbRingPlotVm : WbImageVm
    {
        private const int ImageSize = 500;
        private const float Center = ImageSize / 2;
        private const float RingWidth = 80;
        private static readonly float[,] scaffold;


        public WbRingPlotVm(int radius, Func<float, Color> colorMap)
            : base(ImageSize, ImageSize)
        {
            Radius = radius;
            _colorMap = colorMap;

            Coords = new int[Functions.TrigFuncSteps, 2, 2];
            for (var i = 0; i < Functions.TrigFuncSteps; i++)
            {
                Coords[i, 0, 0] = (int)(Functions.CosLu(((float)i) / Functions.TrigFuncSteps) * Radius + Center);
                Coords[i, 0, 1] = (int)(Functions.SinLu(((float)i) / Functions.TrigFuncSteps) * Radius + Center);
                Coords[i, 1, 0] = (int)(Functions.CosLu(((float)i) / Functions.TrigFuncSteps) * (Radius + RingWidth) + Center);
                Coords[i, 1, 1] = (int)(Functions.SinLu(((float)i) / Functions.TrigFuncSteps) * (Radius + RingWidth) + Center);
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
