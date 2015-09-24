using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using LibNode;
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


        public WbRingPlotVm(int radius)
            : base(ImageSize, ImageSize)
        {
            Radius = radius;

            _coords = new int[Functions.TrigFuncSteps, 2, 2];
            for (var i = 0; i < Functions.TrigFuncSteps; i++)
            {
                _coords[i, 0, 0] = (int)(Functions.CosLu(((float)i) / Functions.TrigFuncSteps) * Radius + Center);
                _coords[i, 0, 1] = (int)(Functions.SinLu(((float)i) / Functions.TrigFuncSteps) * Radius + Center);
                _coords[i, 1, 0] = (int)(Functions.CosLu(((float)i) / Functions.TrigFuncSteps) * (Radius + RingWidth) + Center);
                _coords[i, 1, 1] = (int)(Functions.SinLu(((float)i) / Functions.TrigFuncSteps) * (Radius + RingWidth) + Center);
            }
        }

        public void AddValues(IEnumerable<D1Val<Color>> ringVals)
        {
            PlotLineList.Clear();

            PlotLineList = ringVals.Select(

                gv =>
                    new PlotLine
                       (
                            x1: _coords[gv.Index, 0, 0],
                            y1: _coords[gv.Index, 0, 1],
                            x2: _coords[gv.Index, 1, 0],
                            y2: _coords[gv.Index, 1, 1],
                            color: gv.Val
                       )
                    ).ToList();

            OnPropertyChanged("PlotLines");
        }

        public float Radius { get; set; }

        private readonly int[,,] _coords;

    }

}
