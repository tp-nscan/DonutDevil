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
using LibNode;
using MathLib.NumericTypes;
using Microsoft.WindowsAPICodePack.Dialogs;
using WpfUtils;
using WpfUtils.Utils;
using Color = System.Windows.Media.Color;
//Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System.Windows.Media.Color;

namespace DonutDevilControls.ViewModel.Legend
{
    public class TwoDLegendVm : NotifyPropertyChanged, ILegendVm
    {
        public TwoDLegendVm()
        {
            _plot2DVm = new Plot2DVm(128, 128)
            {
                Title = "Node values",
                MinValueX = 0.0,
                MaxValueX =  1.0,
                MinValueY = 0.0,
                MaxValueY =  1.0
            };

            IsStandard = true;
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
                return _loadImageFileCommand ?? (_loadImageFileCommand = new RelayCommand(
                    param => SetImageFileWithDialog(),
                    param => CanLoadImage()
                    ));
            }
        }

        private async void SetImageFileWithDialog()
        {
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
                await LoadImageFile(od.FileName);
            }
        }


        public async Task LoadImageFile(string imageFileName)
        {
            await Task.Run(() =>
            {
                var image1 = (Bitmap)Image.FromFile(imageFileName, true);

                _imageColors = new Color[image1.Height, image1.Width];

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
                            Plot2DVm.WbUniformGridVm.AddValues(
                                Enumerable.Range(0, ImageColors.GetLength(0) * ImageColors.GetLength(1))
                                    .Select(
                                        i=> new D2Val<Color>(
                                            x: i % ImageWidth,
                                            y: i / ImageWidth,
                                            val: _imageColors[(i) / ImageHeight, (i) % ImageWidth]
                                            )
                                    
                                    )
                                );
                            IsDirty = true;
                            _legendVmChanged.OnNext(this);
                        },
                        DispatcherPriority.Background
                    );
            });
        }


        private readonly Subject<ILegendVm> _legendVmChanged
            = new Subject<ILegendVm>();
        public IObservable<ILegendVm> OnLegendVmChanged
        {
            get { return _legendVmChanged; }
        }

        public int ImageHeight
        {
            get { return _imageColors.GetLength(0); }
        }

        public int ImageWidth
        {
            get { return _imageColors.GetLength(1); }
        }

        private Color[,] _imageColors = new Color[0, 0];
        public Color[,] ImageColors
        {
            get { return _imageColors; }
        }

        public Color ColorForInterval(float val)
        {
            throw new NotImplementedException();
        }

        public Color ColorFor1D(float val)
        {
            throw new NotImplementedException();
        }

        public Color ColorFor2D(float xVal, float yVal)
        {
            return _imageColors[(int)(xVal * 0.9999 * ImageHeight), (int)(yVal * 0.9999 * ImageWidth)];
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
                return _standardCommand ?? (_standardCommand = new RelayCommand(
                    param => DoStandard(),
                    param => CanStandard()
                    ));
            }
        }

        private void DoStandard()
        {
            _imageColors = new Color[1024, 1024];


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
                    Plot2DVm.WbUniformGridVm.AddValues(
                        Enumerable.Range(0, ImageColors.GetLength(0) * ImageColors.GetLength(1))
                            .Select(
                                i => new D2Val<Color>(
                                    x: i % 1024,
                                    y: i / 1024,
                                    val: _imageColors[(i) / 1024, (i) % 1024]
                                    )

                            )
                        );
                    IsDirty = true;
                    _legendVmChanged.OnNext(this);
                },
                DispatcherPriority.Background
            );
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
                return _xonlyCommand ?? (_xonlyCommand = new RelayCommand(
                    param => DoXOnly(),
                    param => CanXOnly()
                    ));
            }
        }

        private void DoXOnly()
        {
            _imageColors = new Color[1024,1024];
            var colorSequence = ColorSequence.Quadrupolar(Colors.Red, Colors.Orange, Colors.Green, Colors.Blue, 256);
            for (var row = 0; row < 1024; row++)
            {
                for (var col = 0; col < 1024; col++)
                {
                    _imageColors[row, col] = colorSequence.Colors[col];
                }
            }

            Application.Current.Dispatcher.Invoke
                (
                    () =>
                    {
                        Plot2DVm.WbUniformGridVm.AddValues(
                            Enumerable.Range(0, ImageColors.GetLength(0) * ImageColors.GetLength(1))
                                .Select(
                                    i => new D2Val<Color>(
                                        x: i % 1024,
                                        y: i / 1024,
                                        val: _imageColors[(i) / 1024, (i) % 1024]
                                        )

                                )
                            );
                        IsDirty = true;
                        _legendVmChanged.OnNext(this);
                    },
                    DispatcherPriority.Background
                );
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
                return _yonlyCommand ?? (_yonlyCommand = new RelayCommand(
                    param => DoYOnly(),
                    param => CanYOnly()
                    ));
            }
        }

        private void DoYOnly()
        {

            _imageColors = new Color[1024, 1024];
            var colorSequence = ColorSequence.Quadrupolar(Colors.Red, Colors.Orange, Colors.Green, Colors.Blue, 256);
            for (var row = 0; row < 1024; row++)
            {
                for (var col = 0; col < 1024; col++)
                {
                    _imageColors[row, col] = colorSequence.Colors[row];
                }
            }

            Application.Current.Dispatcher.Invoke
            (
                () =>
                {
                    Plot2DVm.WbUniformGridVm.AddValues(
                        Enumerable.Range(0, ImageColors.GetLength(0) * ImageColors.GetLength(1))
                            .Select(
                                i => new D2Val<Color>(
                                        x: i % 1024,
                                        y: i / 1024,
                                        val: _imageColors[(i) / 1024, (i) % 1024]
                                    )
                            )
                        );
                    IsDirty = true;
                    _legendVmChanged.OnNext(this);
                },
                DispatcherPriority.Background
            );
        }


        bool CanYOnly()
        {
            return true;
        }

        #endregion // YonlyCommand


        private bool _isStandard;
        public bool IsStandard
        {
            get { return _isStandard; }
            set
            {
                if (value)
                {
                    _isXonly = false;
                    OnPropertyChanged("IsXonly");
                    _isYonly = false;
                    OnPropertyChanged("IsYonly");
                    DoStandard();
                }

                _isStandard = value;
                OnPropertyChanged("IsStandard");
            }
        }

        private bool _isXonly;
        public bool IsXonly
        {
            get { return _isXonly; }
            set
            {
                if (value)
                {
                    _isStandard = false;
                    OnPropertyChanged("IsStandard");
                    _isYonly = false;
                    OnPropertyChanged("IsYonly");
                    DoXOnly();
                }

                _isXonly = value;
                OnPropertyChanged("IsXonly");
            }
        }

        private bool _isYonly;
        public bool IsYonly
        {
            get { return _isYonly; }
            set
            {
                if (value)
                {
                    _isStandard = false;
                    OnPropertyChanged("IsStandard");
                    _isXonly = false;
                    OnPropertyChanged("IsXonly");
                    DoYOnly();
                }

                _isYonly = value;
                OnPropertyChanged("IsYonly");
            }
        }


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

        public LegendType LegendType
        {
            get { return LegendType.Torus; }
        }
    }
}
