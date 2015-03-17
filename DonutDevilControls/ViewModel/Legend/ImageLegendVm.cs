using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DonutDevilControls.ViewModel.Common;
using Microsoft.WindowsAPICodePack.Dialogs;
using WpfUtils;
using WpfUtils.Utils;
using WpfUtils.Views.Graphics;

namespace DonutDevilControls.ViewModel.Legend
{
    public class ImageLegendVm : NotifyPropertyChanged, ILegendVm
    {
        public ImageLegendVm()
        {
            _plot2DVm = new Plot2DVm
            {
                Title = "Node values",
                MinValueX = 0.0,
                MaxValueX =  1.0,
                MinValueY = 0.0,
                MaxValueY =  1.0
            };
        }

        private readonly Plot2DVm _plot2DVm;
        public Plot2DVm Plot2DVm
        {
            get { return _plot2DVm; }
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
            if (od.ShowDialog() == CommonFileDialogResult.Ok)
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
                                Plot2DVm.GraphicsInfos =
                                    Enumerable.Range(0, ImageColors.GetLength(0) * ImageColors.GetLength(1))
                                        .Select(
                                            c => new PlotPoint
                                                (
                                                x: c % ImageWidth,
                                                y: c / ImageWidth,
                                                color: _imageColors[(c) / ImageHeight, (c) % ImageWidth]
                                                )).ToList();

                                IsDirty = true;
                                _colorsChanged.OnNext(true);
                            },
                            DispatcherPriority.Background
                        );

                }
                catch (Exception)
                {
                }
            });
        }

        private readonly Subject<bool> _colorsChanged
            = new Subject<bool>();
        public IObservable<bool> OnColorsChanged
        {
            get { return _colorsChanged; }
        }

        //private readonly Subject<System.Windows.Media.Color[,]> _colorsChanged 
        //    = new Subject<System.Windows.Media.Color[,]>();
        //public IObservable<System.Windows.Media.Color[,]> OnColorsChanged
        //{
        //    get { return _colorsChanged; }
        //}

        public int ImageHeight
        {
            get { return _imageColors.GetLength(0); }
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


        #region StandardCommand

        RelayCommand _standardCommand;
        public ICommand StandardCommand
        {
            get
            {
                if (_standardCommand == null)
                    _standardCommand = new RelayCommand(
                            param => DoStandard(),
                            param => CanStandard()
                        );

                return _standardCommand;
            }
        }

        private void DoStandard()
        {
            try
            {
                _imageColors = new System.Windows.Media.Color[1024, 1024];


                var n1ColorSequence = ColorSequence.TriPolar(1024);

                for (var row = 0; row < 1024; row++)
                {
                    for (var col = 0; col < 1024; col++)
                    {
                        _imageColors[row, col] = n1ColorSequence.Colors[row*1024 + col];
                    }
                }

                Application.Current.Dispatcher.Invoke
                    (
                        () =>
                        {
                            Plot2DVm.GraphicsInfos =
                                Enumerable.Range(0, 1024 * 1024)
                                    .Select(
                                        c => new PlotPoint
                                            (
                                            x: c % 1024,
                                            y: c / 1024,
                                            color: _imageColors[(c) / 1024, (c) % 1024]
                                            )).ToList();
                            IsDirty = true;
                            _colorsChanged.OnNext(true);
                        },
                        DispatcherPriority.Background
                    );
            }
            catch (Exception)
            {
            }
        }


        bool CanStandard()
        {
            return true;
        }

        #endregion // StandardCommand

        #region XonlyCommand

        RelayCommand _xonlyCommand;
        public ICommand XonlyCommand
        {
            get
            {
                if (_xonlyCommand == null)
                    _xonlyCommand = new RelayCommand(
                            param => DoXOnly(),
                            param => CanXOnly()
                        );

                return _xonlyCommand;
            }
        }

        private void DoXOnly()
        {
            try
            {
                _imageColors = new System.Windows.Media.Color[1024,1024];
                var _colorSequence = ColorSequence.Quadrupolar(Colors.Red, Colors.Orange, Colors.Green, Colors.Blue, 256);
                for (var row = 0; row < 1024; row++)
                {
                    for (var col = 0; col < 1024; col++)
                    {
                        _imageColors[row, col] = _colorSequence.Colors[col];
                    }
                }

                Application.Current.Dispatcher.Invoke
                    (
                        () =>
                        {
                            Plot2DVm.GraphicsInfos =
                                Enumerable.Range(0, 1024 * 1024)
                                    .Select(
                                        c => new PlotPoint
                                            (
                                            x: c % 1024,
                                            y: c / 1024,
                                            color: _imageColors[(c) / 1024, (c) % 1024]
                                            )).ToList();
                            IsDirty = true;
                            _colorsChanged.OnNext(true);
                        },
                        DispatcherPriority.Background
                    );

            }
            catch (Exception)
            {
            }
        }


        bool CanXOnly()
        {
            return true;
        }

        #endregion // XonlyCommand

        #region YonlyCommand

        RelayCommand _yonlyCommand;
        public ICommand YonlyCommand
        {
            get
            {
                if (_yonlyCommand == null)
                    _yonlyCommand = new RelayCommand(
                            param => DoYOnly(),
                            param => CanYOnly()
                        );

                return _yonlyCommand;
            }
        }

        private void DoYOnly()
        {
            try
            {
                _imageColors = new System.Windows.Media.Color[1024, 1024];
                var _colorSequence = ColorSequence.Quadrupolar(Colors.Red, Colors.Orange, Colors.Green, Colors.Blue, 256);
                for (var row = 0; row < 1024; row++)
                {
                    for (var col = 0; col < 1024; col++)
                    {
                        _imageColors[row, col] = _colorSequence.Colors[row];
                    }
                }

                Application.Current.Dispatcher.Invoke
                    (
                        () =>
                        {
                            Plot2DVm.GraphicsInfos =
                                Enumerable.Range(0, 1024 * 1024)
                                    .Select(
                                        c => new PlotPoint
                                            (
                                            x: c % 1024,
                                            y: c / 1024,
                                            color: _imageColors[(c) / 1024, (c) % 1024]
                                            )).ToList();
                            IsDirty = true;
                            _colorsChanged.OnNext(true);

                        },
                        DispatcherPriority.Background
                    );

            }
            catch (Exception)
            {
            }
        }


        bool CanYOnly()
        {
            return true;
        }

        #endregion // YonlyCommand



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


        public bool IsDirty { get; set; }

        public LegendVmType LegendVmType
        {
            get { return LegendVmType.Image; }
        }
    }
}
