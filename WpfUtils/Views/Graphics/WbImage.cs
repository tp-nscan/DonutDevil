using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LibNode;

namespace WpfUtils.Views.Graphics
{
    public class WbImage : System.Windows.Controls.Image
    {
        private WriteableBitmap _writeableBmp;

        [Category("Custom Properties")]
        public IReadOnlyList<D2Val<Color>> PlotPoints
        {
            get { return (IReadOnlyList<D2Val<Color>>)GetValue(PlotPointsProperty); }
            set { SetValue(PlotPointsProperty, value); }
        }

        public static readonly DependencyProperty PlotPointsProperty =
            DependencyProperty.Register("PlotPoints", typeof(IReadOnlyList<D2Val<Color>>), typeof(WbImage),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,
                OnPlotPointsChanged));

        private static void OnPlotPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var graphicsInfo = (IReadOnlyList<D2Val<Color>>)e.NewValue;
            if ((graphicsInfo == null) || (graphicsInfo.Count == 0))
            {
                return;
            }

            var gridImage = d as WbImage;
            gridImage?.DoPlotPoints();
        }

        void DoPlotPoints()
        {
            if ((ImageWidth == 0) || (ImageHeight == 0))
            {
                return;
            }
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
                        pixelInfo.Val);
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
            gridImage?.DoPlotRectangles();
        }

        void DoPlotRectangles()
        {
            if ((ImageWidth == 0) || (ImageHeight == 0))
            {
                return;
            }

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
        public IReadOnlyList<PlotLine> PlotLines
        {
            get { return (IReadOnlyList<PlotLine>)GetValue(PlotLinesProperty); }
            set { SetValue(PlotLinesProperty, value); }
        }

        public static readonly DependencyProperty PlotLinesProperty =
            DependencyProperty.Register("PlotLines", typeof(IReadOnlyList<PlotLine>), typeof(WbImage),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,
                OnPlotLinesChanged));

        private static void OnPlotLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var graphicsInfo = (IReadOnlyList<PlotLine>)e.NewValue;
            if (graphicsInfo == null)
            {
                return;
            }

            var gridImage = d as WbImage;
            gridImage?.DoPlotLines();
        }

        void DoPlotLines()
        {
            if ((ImageWidth == 0) || (ImageHeight == 0))
            {
                return;
            }

            if (_writeableBmp == null)
            {
                _writeableBmp = BitmapFactory.New(ImageWidth, ImageHeight);
                Source = _writeableBmp;
            }
            using (_writeableBmp.GetBitmapContext())
            {
                _writeableBmp.Clear();

                foreach (var plotLine in PlotLines)
                {
                    _writeableBmp.DrawLineAa(
                            plotLine.X1,
                            plotLine.Y1,
                            plotLine.X2,
                            plotLine.Y2,
                            plotLine.Color
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
