using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfUtils.Views.Graphics
{
    public class WbImage : System.Windows.Controls.Image
    {
        private WriteableBitmap _writeableBmp;

        [Category("Custom Properties")]
        public IReadOnlyList<PlotPoint> PlotPoints
        {
            get { return (IReadOnlyList<PlotPoint>)GetValue(PlotPointsProperty); }
            set { SetValue(PlotPointsProperty, value); }
        }

        public static readonly DependencyProperty PlotPointsProperty =
            DependencyProperty.Register("PlotPoints", typeof(IReadOnlyList<PlotPoint>), typeof(WbImage),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,
                OnPlotPointsChanged));

        private static void OnPlotPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var graphicsInfo = (IReadOnlyList<PlotPoint>)e.NewValue;
            if ((graphicsInfo == null) || (graphicsInfo.Count == 0))
            {
                return;
            }

            var gridImage = d as WbImage;
            if ((gridImage == null)) return;
            gridImage.DoPlotPoints();
        }

        void DoPlotPoints()
        {
            if (_writeableBmp == null)
            {
                _writeableBmp = BitmapFactory.New(ImageWidth, ImageHeight);
                Source = _writeableBmp;
            }

            var rectWidth = (ImageWidth / (PlotPoints.Max(g => g.X) + 1));
            var rectHeight = (ImageHeight / (PlotPoints.Max(g => g.Y) + 1));

            using (_writeableBmp.GetBitmapContext())
            {
                _writeableBmp.Clear();

                foreach (var pixelInfo in PlotPoints)
                {
                    _writeableBmp.SetPixel(
                        pixelInfo.X * rectWidth,
                        pixelInfo.Y * rectHeight,
                        pixelInfo.Color);
                }

            } // Invalidates on exit of using block

        }


        [Category("Custom Properties")]
        public IReadOnlyList<PlotRectangle> PlotRectangles
        {
            get { return (IReadOnlyList<PlotRectangle>)GetValue(PlotRectanglesProperty); }
            set { SetValue(PlotRectanglesProperty, value); }
        }

        public static readonly DependencyProperty PlotRectanglesProperty =
            DependencyProperty.Register("PlotRectangles", typeof(IReadOnlyList<PlotRectangle>), typeof(WbImage),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,
                OnPlotRectanglesChanged));

        private static void OnPlotRectanglesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var graphicsInfo = (IReadOnlyList<PlotRectangle>)e.NewValue;
            if (graphicsInfo == null)
            {
                return;
            }

            var gridImage = d as WbImage;
            if ((gridImage == null)) return;
            gridImage.DoPlotRectangles();
        }

        void DoPlotRectangles()
        {
            if (_writeableBmp == null)
            {
                _writeableBmp = BitmapFactory.New(ImageWidth, ImageHeight);
                Source = _writeableBmp;
            }
            using (_writeableBmp.GetBitmapContext())
            {
                _writeableBmp.Clear();

                foreach (var plotRectangle in PlotRectangles)
                {
                    _writeableBmp.FillRectangle(
                        plotRectangle.X,
                        plotRectangle.Y,
                        plotRectangle.X +  plotRectangle.Width,
                        plotRectangle.Y +  plotRectangle.Height,
                        plotRectangle.Color
                        );
                }

            } // Invalidates on exit of using block

        }

        [Category("Custom Properties")]
        public int ImageWidth
        {
            get { return (int)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(int), typeof(WbImage));


        [Category("Custom Properties")]
        public int ImageHeight
        {
            get { return (int)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(int), typeof(WbImage));


    }
}
