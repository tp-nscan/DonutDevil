using System;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
                MinValueX = 0.0,
                MaxValueX =  1.0,
                MinValueY = 0.0,
                MaxValueY =  1.0
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
                            param => SetImageFileWithDialog(),
                            param => CanLoadImage()
                        );

                return _loadImageFileCommand;
            }
        }

        private void SetImageFileWithDialog()
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
            if (od.ShowDialog() != CommonFileDialogResult.Ok)
            {
                LoadImageFile(od.FileName);
            }
        }


        public async Task LoadImageFile(string imageFileName)
        {
            await Task.Run(() =>
            {
                try
                {
                    var image1 = (Bitmap)Image.FromFile(imageFileName, true);

                    _imageColors = new System.Windows.Media.Color[image1.Height, image1.Width];

                    unsafe
                    {
                        var bmd = image1.LockBits(new Rectangle(0, 0, image1.Width, image1.Height), ImageLockMode.ReadOnly, image1.PixelFormat);
                        for (var row = 0; row < bmd.Height; row++)
                        {
                            var offset = (row * bmd.Stride);

                            for (var col = 0; col < bmd.Width; col++)
                            {
                                var currentPixel = (255 << 24);
                                currentPixel += (((byte*)bmd.Scan0)[offset + col * 3 + 0] << 16);
                                currentPixel += (((byte*)bmd.Scan0)[offset + col * 3 + 1] << 8);
                                currentPixel += (((byte*)bmd.Scan0)[offset + col * 3 + 2] << 0);

                                _imageColors[row, col] = currentPixel.IntToColor();
                            }
                        }
                    }

                    Application.Current.Dispatcher.Invoke
                        (
                            () =>
                            {

                                ImageFileName = imageFileName;
                                ColorLegendVm2D.GraphicsInfos =
                                    Enumerable.Range(0, ImageColors.GetLength(0) * ImageColors.GetLength(1))
                                        .Select(
                                            c => new GraphicsInfo
                                                (
                                                x: c % ImageWidth,
                                                y: c / ImageWidth,
                                                color: _imageColors[(c) / ImageHeight, (c) % ImageWidth]
                                                )).ToList();

                                _colorsChanged.OnNext(ImageColors);
                            },
                            DispatcherPriority.Background
                        );

                }
                catch (Exception)
                {
                }
            });
        }

        private readonly Subject<System.Windows.Media.Color[,]> _colorsChanged 
            = new Subject<System.Windows.Media.Color[,]>();
        public IObservable<System.Windows.Media.Color[,]> OnColorsChanged
        {
            get { return _colorsChanged; }
        }

        public int ImageHeight
        {
            get { return _imageColors.GetLength(1); }
        }

        public int ImageWidth
        {
            get { return _imageColors.GetLength(1); }
        }

        private System.Windows.Media.Color[,] _imageColors = new System.Windows.Media.Color[0, 0];
        public System.Windows.Media.Color[,] ImageColors
        {
            get { return _imageColors; }
            private set
            {
                _imageColors = value;
            }
        }

        public System.Windows.Media.Color ColorForUnitTorus(float xVal, float yVal)
        {
            return _imageColors[(int)(xVal*ImageHeight), (int)(yVal*ImageWidth)];
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
            private set
            {
                _imageFileName = value;
                OnPropertyChanged("ImageFileName");
            }
        }
    }
}
