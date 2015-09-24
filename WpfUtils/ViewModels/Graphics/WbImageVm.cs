using System.Collections.Generic;
using System.Windows.Media;
using LibNode;
using WpfUtils.Views.Graphics;

namespace WpfUtils.ViewModels.Graphics
{
    public class WbImageVm : NotifyPropertyChanged
    {
        public WbImageVm(int imageWidth, int imageHeight)
        {
            ImageWidth = imageWidth;
            ImageHeight = imageHeight;
        }

        public int ImageWidth { get; }

        public int ImageHeight { get; }

        protected List<D2Val<Color>> PlotPointList = new List<D2Val<Color>>();
        public IReadOnlyList<D2Val<Color>> PlotPoints => PlotPointList;

        protected List<PlotRectangle> PlotRectangleList = new List<PlotRectangle>();
        public IReadOnlyList<PlotRectangle> PlotRectangles => PlotRectangleList;

        protected List<PlotLine> PlotLineList = new List<PlotLine>();
        public IReadOnlyList<PlotLine> PlotLines => PlotLineList;

    }
}
