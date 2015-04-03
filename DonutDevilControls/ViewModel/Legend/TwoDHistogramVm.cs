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
    public class TwoDhistogramVm : NotifyPropertyChanged, IHistogramVm
    {
        private const int Colorsteps = 512;

        public TwoDhistogramVm(string title, int size)
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
            LegendVm.WbUniformGridVm.AddValues
            (
                Colorsteps.ToSquareD2Array(
                    (i, j) => colorFunc(i.FractionOf(Colorsteps), j.FractionOf(Colorsteps))
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
            var hist = xVals.ToPoints(yVals)
                            .ToScaledHistogram(Colorsteps, 1.0f, 1.0f);

            HistogramVm.WbUniformGridVm.AddValues
                (
                    Colorsteps.ToSquareD2Array(
                        (i, j) => _histogramColorSequence.ToUnitColor(hist[i,j])
                    )
                );
        }

        public LegendType DisplaySpaceType
        {
            get { return LegendType.Torus; }
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

        public string TitleX
        {
            get { return HistogramVm.TitleX; }
            set
            {
                LegendVm.TitleX = value;
                HistogramVm.TitleX = value;
            }
        }

        public string TitleY
        {
            get { return HistogramVm.TitleY; }
            set
            {
                LegendVm.TitleY = value;
                HistogramVm.TitleY = value;
            }
        }

        public double MinValue
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double MaxValue
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double MinValueX
        {
            get { return LegendVm.MinValueX; }
            set
            {
                LegendVm.MinValueX = value;
                HistogramVm.MinValueX = value;
            }
        }

        public double MaxValueX
        {
            get { return LegendVm.MaxValueX; }
            set
            {
                LegendVm.MaxValueX = value;
                HistogramVm.MaxValueX = value;
            }
        }

        public double MinValueY
        {
            get { return LegendVm.MinValueY; }
            set
            {
                LegendVm.MinValueY = value;
                HistogramVm.MinValueY = value;
            }
        }

        public double MaxValueY
        {
            get { return LegendVm.MaxValueY; }
            set
            {
                LegendVm.MaxValueY = value;
                HistogramVm.MaxValueY = value;
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
