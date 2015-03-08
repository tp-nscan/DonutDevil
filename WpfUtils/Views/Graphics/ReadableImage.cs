using System.Windows.Media.Imaging;
using WpfUtils.ViewModels.Graphics;

namespace WpfUtils.Views.Graphics
{
    public class ReadableImage : System.Windows.Controls.Image
    {
        public ReadableImage()
        {
            SourceUpdated += ReadableImage_SourceUpdated;
        }

        void ReadableImage_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            SetColorValues();
        }

        public override void EndInit()
        {
            base.EndInit();
            SetColorValues();
        }

        void SetColorValues()
        {
            var source = Source as BitmapSource;
            if (source == null)
            {
                return;
            }

            var colorArray = new int[source.PixelWidth * source.PixelHeight];

            source.CopyPixels(colorArray, source.PixelWidth * 4, 0);

            _colorArray2D = new int[source.PixelHeight, source.PixelWidth];

            for (var i = 0; i < source.PixelHeight; i++)
            {
                for (var j = 0; j < source.PixelWidth; j++)
                {
                    _colorArray2D[i, j] = colorArray[i * source.PixelWidth + j];
                }
            }

            var vm = DataContext as ReadableImageVm;
            if (vm == null)
            {
                return;
            }
            vm.ColorValues = _colorArray2D;
        }

        private int[,] _colorArray2D;
        public int[,] ColorArray2D
        {
            get { return _colorArray2D; }
        }
    }
}
