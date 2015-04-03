using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace DonutDevilControls.ViewModel.Legend
{
    public interface IHistogramVm
    {
        LegendType DisplaySpaceType { get; }
        void DrawLegend(Func<float, float, Color> colorFunc);
        void DrawLegend(Func<float, Color> colorFunc);

        void MakeHistogram(IEnumerable<float> values);
        void MakeHistogram(IEnumerable<float> xVals, IEnumerable<float> yVals);

        string Title { get; set; }
        string TitleX { get; set; }
        string TitleY { get; set; }

        double MinValue { get; set; }
        double MaxValue { get; set; }

        double MinValueX { get; set; }
        double MaxValueX { get; set; }
        double MinValueY { get; set; }
        double MaxValueY { get; set; }
    }
}