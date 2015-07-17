using System.Collections.Generic;
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

        protected List<PlotPoint> PlotPointList = new List<PlotPoint>();
        public IReadOnlyList<PlotPoint> PlotPoints => PlotPointList;

        protected List<PlotRectangle> PlotRectangleList = new List<PlotRectangle>();
        public IReadOnlyList<PlotRectangle> PlotRectangles => PlotRectangleList;

        protected List<PlotLine> PlotLineList = new List<PlotLine>();
        public IReadOnlyList<PlotLine> PlotLines => PlotLineList;

    }
}
