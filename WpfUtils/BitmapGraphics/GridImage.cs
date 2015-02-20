using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfUtils.BitmapGraphics
{
    public class GridImage : System.Windows.Controls.Image
    {
        private WriteableBitmap _writeableBmp;

        [Category("Custom Properties")]
        public IReadOnlyList<GraphicsInfo> GraphicsInfos
        {
            get { return (IReadOnlyList<GraphicsInfo>)GetValue(GraphicsInfosProperty); }
            set { SetValue(GraphicsInfosProperty, value); }
        }

        public static readonly DependencyProperty GraphicsInfosProperty =
            DependencyProperty.Register("GraphicsInfos", typeof(IReadOnlyList<GraphicsInfo>), typeof(GridImage),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,
                OnGraphicsInfosChanged));

        private static void OnGraphicsInfosChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var graphicsInfo = (IReadOnlyList<GraphicsInfo>)e.NewValue;
            if ((graphicsInfo == null) || (graphicsInfo.Count == 0))
            {
                return;
            }
            var gridImage = d as GridImage;
            if (gridImage == null) return;

            gridImage.DrawGrid();

        }

        void DrawGrid()
        {
            if (_writeableBmp == null)
            {
                _writeableBmp = BitmapFactory.New((int)Width, (int)Height);
                Source = _writeableBmp;
            }

            var rectWidth = (Width / (GraphicsInfos.Max(g => g.X) +1));
            var rectHeight = (Height / (GraphicsInfos.Max(g => g.Y) +1));

            using (_writeableBmp.GetBitmapContext())
            {
                _writeableBmp.Clear();

                foreach (var pixelInfo in GraphicsInfos)
                {
                    _writeableBmp.FillRectangle(
                        (int) (pixelInfo.X * rectWidth),
                        (int) (pixelInfo.Y * rectWidth),
                        (int) ((pixelInfo.X + 1) * rectHeight),
                        (int) ((pixelInfo.Y + 1) * rectHeight), 
                        pixelInfo.Color);
                }

            } // Invalidates on exit of using block

        }
    }
}
