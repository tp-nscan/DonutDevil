using System;
using System.Collections.Generic;
using System.Windows.Media;
using DonutDevilControls.ViewModel.Common;
using MathLib.Intervals;
using MathLib.NumericTypes;
using WpfUtils;
using WpfUtils.Utils;

namespace DonutDevilControls.ViewModel.Legend
{
    public class TorusHistogramVm : NotifyPropertyChanged, IHistogramVm
    {
        private const int Colorsteps = 512;

        public TorusHistogramVm(string title, int size)
        {
            _title = title;
            _legendVm = new Plot2DVm(size, size)
            {
                Title = title,
                MinValueX = 0.0,
                MaxValueX = 0.0,
                MinValueY = 1.0,
                MaxValueY = 1.0
            };

            _histogramVm = new Plot2DVm(size, size)
            {
                Title = title,
                MinValueX = 0.0,
                MaxValueX = 0.0,
                MinValueY = 1.0,
                MaxValueY = 1.0
            };

            _showHistogramSliderVm = new SliderVm(RealInterval.Make(0.0, 1.0), 0.1, "0.0") 
                { Title = "Show Histogram", Value = 1.0 };

            _histogramColorSequence = Colors.White.ToUniformColorSequence(Colorsteps);
        }

        private readonly IColorSequence _histogramColorSequence;


        public void DrawLegend(Func<float, float, Color> colorFunc)
        {
            var gridSize = LegendVm.WbUniformGridVm.CellDimX;

            LegendVm.WbUniformGridVm.AddValues
            (
                gridSize.ToSquareD2Array(
                    (i, j) => colorFunc(i.FractionOf(gridSize), j.FractionOf(gridSize))
                )
            );
        }

        public void DrawLegend(Func<float, Color> colorFunc)
        {
            throw new NotImplementedException();
        }

        public void MakeHistogram(IEnumerable<float> values)
        {
            throw new NotImplementedException();
        }

        public void MakeHistogram(IEnumerable<float> xVals, IEnumerable<float> yVals)
        {
            var gridSize = LegendVm.WbUniformGridVm.CellDimX;
            var hist = xVals.ToPoints(yVals)
                            .ToScaledHistogram(gridSize, 1.0f, 1.0f);

            HistogramVm.WbUniformGridVm.AddValues
                (
                    gridSize.ToSquareD2Array(
                        (i, j) => _histogramColorSequence.ToUnitColor(hist[i,j])
                    )
                );
        }

        public DisplaySpaceType DisplaySpaceType
        {
            get { return DisplaySpaceType.Torus; }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }


        private readonly Plot2DVm _legendVm;
        public Plot2DVm LegendVm
        {
            get { return _legendVm; }
        }

        private readonly Plot2DVm _histogramVm;
        public Plot2DVm HistogramVm
        {
            get { return _histogramVm; }
        }

        private readonly SliderVm _showHistogramSliderVm;
        public SliderVm ShowHistogramSliderVm
        {
            get { return _showHistogramSliderVm; }
        }
    }
}
