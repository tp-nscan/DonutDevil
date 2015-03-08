using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfUtils.ViewModels.Graphics
{
    public class ReadableImageVm : NotifyPropertyChanged
    {
        private int[,] _colorValues;
        private WriteableBitmap _writeableBitmap;

        public int[,] ColorValues
        {
            get { return _colorValues; }
            set
            {
                _colorValues = value;
                OnPropertyChanged("ColorValues");
                MakeWbm();
            }
        }

        public WriteableBitmap WriteableBitmap
        {
            get { return _writeableBitmap; }
            set
            {
                _writeableBitmap = value;
                OnPropertyChanged("WriteableBitmap");

            }
        }

        private WriteableBitmap _inputBitmap;
        public WriteableBitmap InputBitmap
        {
            get { return _inputBitmap; }
            set
            {
                _inputBitmap = value;
                OnPropertyChanged("ColorValues");

            }
        }

        void MakeWbm()
        {
            int shrink = 2;

            var height = ColorValues.GetLength(0);
            var width = ColorValues.GetLength(1);
            // Create data array to hold source pixel data
            var data = new byte[width * height];

            //for (var i = 0; i < height/shrink; i++)
            //{
            //    for (var j = 0; j < width/shrink; j++)
            //    {
            //        data[i * (width * 2) + j + 0] = (byte)(ColorValues[i * 2, j * 2] >> 24);
            //        data[i * (width * 2) + j + 1] = (byte)(ColorValues[i * 2, j * 2] >> 16);
            //        data[i * (width * 2) + j + 2] = (byte)(ColorValues[i * 2, j * 2] >> 8);
            //        data[i * (width * 2) + j + 3] = (byte)(ColorValues[i * 2, j * 2] >> 0);
            //    }
            //}

            for (var i = 0; i < height / shrink; i++)
            {
                for (var j = 0; j < width / shrink; j++)
                {
                    data[i * (width * 2) + j * 4 + 3] = (byte)(ColorValues[i * 2, j * 2] >> 24);
                    data[i * (width * 2) + j * 4 + 2] = (byte)(ColorValues[i * 2, j * 2] >> 16);
                    data[i * (width * 2) + j * 4 + 1] = (byte)(ColorValues[i * 2, j * 2] >> 8);
                    data[i * (width * 2) + j * 4 + 0] = (byte)(ColorValues[i * 2, j * 2] >> 0);
                }
            }

            // Create WriteableBitmap to copy the pixel data to.      
            var target = new WriteableBitmap(
              width/shrink,
              height/shrink,
              72.0, 72.0,
              PixelFormats.Bgr32, null);

            // Write the pixel data to the WriteableBitmap.
            target.WritePixels(
              new Int32Rect(0, 0, width / shrink, height / shrink),
              data, width*2, 0);

            WriteableBitmap = target;
        }
    }

    //BitmapSource source = sourceImage.Source as BitmapSource;

    //// Calculate stride of source
    //int stride = source.PixelWidth * (source.Format.BitsPerPixel / 8);

    //// Create data array to hold source pixel data
    //byte[] data = new byte[stride * source.PixelHeight];

    //// Copy source image pixels to the data array
    //source.CopyPixels(data, stride, 0);

    //// Create WriteableBitmap to copy the pixel data to.      
    //WriteableBitmap target = new WriteableBitmap(
    //  source.PixelWidth, 
    //  source.PixelHeight, 
    //  source.DpiX, source.DpiY, 
    //  source.Format, null);

    //// Write the pixel data to the WriteableBitmap.
    //target.WritePixels(
    //  new Int32Rect(0, 0, source.PixelWidth, source.PixelHeight), 
    //  data, stride, 0);

    //// Set the WriteableBitmap as the source for the <Image> element 
    //// in XAML so you can see the result of the copy
    //targetImage.Source = target;
}
