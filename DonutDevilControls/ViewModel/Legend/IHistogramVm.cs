using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace DonutDevilControls.ViewModel.Legend
{
    public interface IHistogramVm
    {
        DisplaySpaceType DisplaySpaceType { get; }
        void DrawLegend(Func<float, float, Color> colorFunc);
        void DrawLegend(Func<float, Color> colorFunc);

        void MakeHistogram(IEnumerable<float> values);
        void MakeHistogram(IEnumerable<float> xVals, IEnumerable<float> yVals);
    }
}