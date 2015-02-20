using System;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WpfUtils;
using Microsoft.WindowsAPICodePack.Dialogs;
using WpfUtils.BitmapGraphics;
using WpfUtils.Utils;

namespace DonutDevilControls.ViewModel.Common
{
    public class ImageLegendVm : NotifyPropertyChanged
    {
        public ImageLegendVm()
        {
            _colorLegendVm2D = new ColorLegendVm2D
            {
                Title = "Node values",
                MinValueX = -0.5,
                MaxValueX =  0.5,
                MinValueY = -0.5,
                MaxValueY =  0.5
            };
        }

        private readonly ColorLegendVm2D _colorLegendVm2D;
        public ColorLegendVm2D ColorLegendVm2D
        {
            get { return _colorLegendVm2D; }
        }

        #region LoadImageFileCommand

        RelayCommand _loadImageFileCommand;
        public ICommand LoadImageFileCommand
        {
            get
            {
                if (_loadImageFileCommand == null)
                    _loadImageFileCommand = new RelayCommand(
                            param => DoLoadImageFile(),
                            param => CanLoadImage()
                        );

                return _loadImageFileCommand;
            }
        }

        private async void DoLoadImageFile()
        {

            //var d = new CommonSaveFileDialog()
            //{
            //    Title = "Save Screen",
            //    InitialDirectory = Directory.GetCurrentDirectory(),
            //    AddToMostRecentlyUsedList = false,
            //    DefaultDirectory = Directory.GetCurrentDirectory(),
            //    EnsurePathExists = true,
            //    EnsureReadOnly = false,
            //    EnsureValidNames = true,
            //    ShowPlacesList = true,
            //};

            var od = new CommonOpenFileDialog
            {
                Title = "Save Screen",
                InitialDirectory = Directory.GetCurrentDirectory(),
                AddToMostRecentlyUsedList = false,
                DefaultDirectory = Directory.GetCurrentDirectory(),
                EnsurePathExists = true,
                EnsureReadOnly = false,
                EnsureValidNames = true,
                ShowPlacesList = true
            };

            if (od.ShowDialog() == CommonFileDialogResult.Ok)
            {
                System.Diagnostics.Debug.WriteLine(od.FileName);
                ImageFileName = od.FileName;
                //C:\Users\tpnsc_000\Documents\GitHub\DonutDevil\DonutDevilControls\Images\earth.bmp
            }

            await Task.Run(() =>
            {
                try
                {

                    Application.Current.Dispatcher.Invoke
                        (
                            () =>
                            {


                            },
                            DispatcherPriority.Background
                        );

                }
                catch (Exception)
                {
                }
            });
        }

        private readonly Subject<int[,]> _pixelsChanged = new Subject<int[,]>();
        public IObservable<int[,]> OnPixelsChanged
        {
            get { return _pixelsChanged; }
        }

        private int[,] _pixels;
        public int[,] Pixels
        {
            get { return _pixels; }
            private set
            {
                _pixels = value;
                _pixelsChanged.OnNext(value);
            }
        }


        bool CanLoadImage()
        {
            return true;
        }

        #endregion // LoadImageFileCommand

        private string _imageFileName;
        public string ImageFileName
        {
            get { return _imageFileName; }
            set
            {
                _imageFileName = value;

                var image1 = (Bitmap)Image.FromFile(value, true);

                var pixels = new int[image1.Height, image1.Width]; 

                unsafe
                {
                    var bmd = image1.LockBits(new Rectangle(0, 0, image1.Width, image1.Height), ImageLockMode.ReadOnly, image1.PixelFormat);
                    for (var row = 0; row < bmd.Height; row++)
                    {
                        var offset = (row * bmd.Stride);

                        for (var col = 0; col < bmd.Width; col++)
                        {
                            var currentPixel = (  255                                    << 24);
                            currentPixel    += (((byte*)bmd.Scan0)[offset + col * 3 + 0] << 16);
                            currentPixel    += (((byte*)bmd.Scan0)[offset + col * 3 + 1] <<  8);
                            currentPixel    += (((byte*)bmd.Scan0)[offset + col * 3 + 2] <<  0);
                            pixels[row, col] = currentPixel;
                        }
                    }
                }

                ColorLegendVm2D.GraphicsInfos =
                    Enumerable.Range(0, image1.Height * image1.Width)
                        .Select(
                            c => new GraphicsInfo
                                (
                                x: c % image1.Width,
                                y: c / image1.Width,
                                color: pixels[(c) / image1.Width, (c) % image1.Width].IntToColor()
                                )).ToList();
            }
        }
    }
}
