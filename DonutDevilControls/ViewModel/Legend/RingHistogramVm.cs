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

        public void DrawLegend(Func<float, Color> colorFunc )
        {
            LegendVm.AddValues
            (
                Enumerable.Range(0, Functions.TrigFuncSteps)
                .Select(i =>
                    new D1Val<Color>(i, colorFunc(i.FractionOf(Functions.TrigFuncSteps))))
            );
        }

        private readonly IColorSequence _histogramColorSequence;


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
                values.ToHistogram
                (
                    bins: RealInterval.UnitRange.SplitToEvenIntervals(Functions.TrigFuncSteps - 1).ToList(),
                    valuatorFunc: n => n
                );

            var colorDexer = (1.0) / histogram.Max(t => Math.Sqrt(t.Item2.Item));

            HistogramVm.AddValues
            (
                histogram.Select((bin, index)
                    => new D1Val<Color>(index, _histogramColorSequence.ToUnitColor((float)(colorDexer * Math.Sqrt(bin.Item2.Item))
                )))
            );

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

        public DisplaySpaceType DisplaySpaceType
        {
            get { return DisplaySpaceType.Ring; }
        }
    }


}
