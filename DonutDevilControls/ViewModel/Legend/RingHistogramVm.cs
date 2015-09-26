using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using LibNode;
using MathLib;
using MathLib.Intervals;
using MathLib.NumericTypes;
using WpfUtils;
using WpfUtils.Utils;
using WpfUtils.ViewModels.Graphics;

namespace DonutDevilControls.ViewModel.Legend
{
    public class RingHistogramVm : NotifyPropertyChanged, IHistogramVm
    {
        private const int Colorsteps = 512;

        public RingHistogramVm(string title, double minValue, double maxValue)
        {
            _title = title;
            MinValue = minValue;
            MaxValue = maxValue;
            LegendVm = new WbRingPlotVm(80);
            HistogramVm = new WbRingPlotVm(165);

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
                Enumerable.Range(0, Functions.TrigFuncSteps)
                .Select(i =>
                    new D1Val<Color>(i, colorFunc(i.FractionOf(Functions.TrigFuncSteps))))
            );
        }

        public WbRingPlotVm LegendVm { get; }

        public WbRingPlotVm HistogramVm { get; }

        public void MakeHistogram(IEnumerable<float> values)
        {
            var histogram =
                values.ToScaledHistogram
                (
                   resolution: Functions.TrigFuncSteps,
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

        public string TitleX
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string TitleY
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double MinValue { get; set; }

        public double MaxValue { get; set; }

        public double MinValueX
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double MaxValueX
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double MinValueY
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public double MaxValueY
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public LegendType DisplaySpaceType { get; } = LegendType.Ring;
    }


}
