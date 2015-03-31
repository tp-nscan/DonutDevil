using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using MathLib;
using MathLib.Intervals;
using MathLib.NumericTypes;
using WpfUtils;
using WpfUtils.Utils;
using WpfUtils.ViewModels.Graphics;

namespace DonutDevilControls.ViewModel.Legend
{
    public class LinearHistogramVm : NotifyPropertyChanged, IHistogramVm
    {
        protected const int Colorsteps = 256;

        public LinearHistogramVm(string title, double minValue, double maxValue)
        {
            _title = title;
            _minValue = minValue;
            _maxValue = maxValue;
            _legendVm = new WbVerticalStripesVm(Colorsteps, 0.1);
            _histogramVm = new WbVerticalStripesVm(Colorsteps, 0.1);
            _histogramColorSequence = Colors.White.ToUniformColorSequence(Colorsteps);
        }

        private readonly IColorSequence _histogramColorSequence;

        public void DrawLegend(Func<float, float, Color> colorFunc)
        {
            throw new NotImplementedException();
        }

        public void DrawLegend(Func<float, Color> colorFunc)
        {
            LegendVm.AddValues
            (
                Enumerable.Range(0, Colorsteps)
                    .Select(i =>
                        new D1Val<Color>(i, colorFunc(i.FractionOf(Colorsteps)))
                        )
            );
        }

        private readonly WbVerticalStripesVm _legendVm;
        public WbVerticalStripesVm LegendVm
        {
            get { return _legendVm; }
        }

        private readonly WbVerticalStripesVm _histogramVm;
        public WbVerticalStripesVm HistogramVm
        {
            get { return _histogramVm; }
        }

        public void MakeHistogram(IEnumerable<float> values)
        {
            var histogram =
                values.ToScaledHistogram
                (
                   resolution: Colorsteps,
                   max: 1.0f
                );

            HistogramVm.AddValues
            (
                histogram.Select((val, index)
                    => new D1Val<Color>(index, _histogramColorSequence.ToUnitColor(val))
                )
            );

        }

        public void MakeHistogram(IEnumerable<float> xVals, IEnumerable<float> yVals)
        {
            throw new NotImplementedException();
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

        private double _minValue;
        public double MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                OnPropertyChanged("MinValue");
            }
        }

        private double _maxValue;
        public double MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                OnPropertyChanged("MaxValue");
            }
        }

        public DisplaySpaceType DisplaySpaceType
        {
            get { return DisplaySpaceType.Unit; }
        }
    }
}
