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
    public class RingHistogramVm : NotifyPropertyChanged, IHistogramVm
    {
        private const int Colorsteps = 512;

        public RingHistogramVm(string title)
        {
            _title = title;
            _legendVm = new WbRingPlotVm(80);
            _histogramVm = new WbRingPlotVm(165);

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

        private readonly WbRingPlotVm _legendVm;
        public WbRingPlotVm LegendVm
        {
            get { return _legendVm; }
        }

        private readonly WbRingPlotVm _histogramVm;
        public WbRingPlotVm HistogramVm
        {
            get { return _histogramVm; }
        }

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

        public LegendType DisplaySpaceType
        {
            get { return LegendType.Ring; }
        }
    }


}
