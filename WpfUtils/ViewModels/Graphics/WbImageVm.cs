using System.Collections.Generic;
using WpfUtils.Views.Graphics;

namespace WpfUtils.ViewModels.Graphics
{
    public class WbImageVm : NotifyPropertyChanged
    {
        public WbImageVm(int imageWidth, int imageHeight)
        {
            _imageWidth = imageWidth;
            _imageHeight = imageHeight;
        }

        private readonly int _imageWidth;
        public int ImageWidth
        {
            get { return _imageWidth; }
        }

        private readonly int _imageHeight;
        public int ImageHeight
        {
            get { return _imageHeight; }
        }

        protected List<PlotPoint> PlotPointList = new List<PlotPoint>();
        public IReadOnlyList<PlotPoint> PlotPoints
        {
            get { return PlotPointList; }
        }

        protected List<PlotRectangle> PlotRectangleList = new List<PlotRectangle>();
        public IReadOnlyList<PlotRectangle> PlotRectangles
        {
            get { return PlotRectangleList; }
        }

        protected List<PlotLine> PlotLineList = new List<PlotLine>();
        public IReadOnlyList<PlotLine> PlotLines
        {
            get { return PlotLineList; }
        }
    }
}
