﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DonutDevilControls.ViewModel.Common;
using MathLib.Intervals;
using NodeLib;
using WpfUtils;
using WpfUtils.BitmapGraphics;

namespace DonutDevilMain.ViewModel
{
    public class TorusValuedNodeGroupVm : NotifyPropertyChanged
    {
        private const int SquareSize = 256;


        public TorusValuedNodeGroupVm()
        {
            _imageLegendVm = new ImageLegendVm();
            _imageLegendVm.OnColorsChanged.Subscribe(OnPixelsChanged);

            _alphaSliderVm = new SliderVm(RealInterval.Make(0, 0.999), 0.02, "0.00") { Title = "Alpha", Value = 0.1};
            _betaSliderVm = new SliderVm(RealInterval.Make(0, 0.999), 0.02, "0.00") { Title = "Beta", Value = 0.0 };
            _stepSizeSliderVm = new SliderVm(RealInterval.Make(0, 0.5), 0.002, "0.0000") { Title = "Step", Value = 0.1 };

            InitializeRun();
        }

        #region local vars

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private INodeGroup _nodeGroup;
        private INodeGroupUpdater _nodeGroupUpdater;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool _isRunning;
        private bool _isDirty;

        public bool IsDirty
        {
            get
            {
                return  _stepSizeSliderVm.IsDirty || 
                        _alphaSliderVm.IsDirty || 
                        _betaSliderVm.IsDirty ||
                        _imageLegendVm.IsDirty ||
                        _isDirty;
            }

        }

        public void Clean()
        {
            _stepSizeSliderVm.IsDirty = false;
            _alphaSliderVm.IsDirty = false;
            _betaSliderVm.IsDirty = false;
            _imageLegendVm.IsDirty = false;
            _isDirty = false;
        }


        #endregion


        #region UpdateGridCommand

        RelayCommand _updateGridCommand;
        public ICommand UpdateGridCommand
        {
            get
            {
                return _updateGridCommand ?? (_updateGridCommand = new RelayCommand(
                    param => DoUpdateGrid(),
                    param => CanUpdateGrid()
                    ));
            }
        }

        private async void DoUpdateGrid()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _isRunning = true;

            await Task.Run(() =>
            {
                _stopwatch.Start();
                for (var i = 0; _isRunning; i++)
                {
                    var newNodeGroup = _nodeGroupUpdater.Update(_nodeGroup);
                    Activity = _nodeGroup.Activity(newNodeGroup);
                    _nodeGroup = newNodeGroup;
                    Generation++;

                    if (_cancellationTokenSource.IsCancellationRequested)
                    {
                        _isRunning = false;
                        _stopwatch.Stop();
                    }

                    if (i % 5 == 0)
                    {
                        Application.Current.Dispatcher.Invoke
                            (
                                () =>
                                {
                                    ResetNodeGroupUpdaters();
                                    UpdateBindingProperties();
                                    DrawMainGrid();
                                    CommandManager.InvalidateRequerySuggested();
                                },
                                DispatcherPriority.Background
                            );
                    }

                }
            },
                cancellationToken: _cancellationTokenSource.Token
            );
        }

        bool CanUpdateGrid()
        {
            return !_isRunning;
        }

        #endregion // UpdateGridCommand



        #region StopUpdateGridCommand

        RelayCommand _stopUpdateGridCommand;
        public ICommand StopUpdateGridCommand
        {
            get
            {
                if (_stopUpdateGridCommand == null)
                    _stopUpdateGridCommand = new RelayCommand(
                        param => DoCancelUpdateGrid(),
                        param => CanCancelUpdateGrid()
                        );

                return _stopUpdateGridCommand;
            }
        }

        private void DoCancelUpdateGrid()
        {
            _cancellationTokenSource.Cancel();
        }

        bool CanCancelUpdateGrid()
        {
            return _isRunning;
        }

        #endregion // StopUpdateGridCommand


        #region ResetCommand

        RelayCommand _resetCommand;
        public ICommand ResetCommand
        {
            get
            {
                if (_resetCommand == null)
                    _resetCommand = new RelayCommand(
                        param => DoReset(),
                        param => CanDoReset()
                        );

                return _resetCommand;
            }
        }

        private void DoReset()
        {
            InitializeRun();
        }

        bool CanDoReset()
        {
            return !_isRunning;
        }

        #endregion // ResetCommand




        #region local helpers

        void UpdateBindingProperties()
        {
            OnPropertyChanged("Generation");
            OnPropertyChanged("ElapsedTime");
        }

        void DrawMainGrid()
        {
            if (_nodeGroup == null)
            {
                return;
            }

            var ggi = new List<GraphicsInfo>();

            const int offset = SquareSize*SquareSize;

            for (float i = 0; i < SquareSize; i++)
            {
                for (float j = 0; j < SquareSize; j++)
                {
                    ggi.Add(new GraphicsInfo(
                            x:(int) i,
                            y:(int) j,
                            color: ImageLegendVm.ColorForUnitTorus(
                                xVal: _nodeGroup.Values[(int) (j*SquareSize + i)],
                                yVal: _nodeGroup.Values[offset + (int)(j * SquareSize + i)]
                              )
                        ));
                }
            }

            GridGraphicsInfo = ggi;

        }

        async void InitializeRun()
        {
           await ImageLegendVm.LoadImageFile(@"C:\Users\tpnsc_000\Documents\GitHub\DonutDevil\DonutDevilControls\Images\earth.bmp");

           _nodeGroup = NodeGroup.RandomNodeGroup(SquareSize * SquareSize * 2, (int)DateTime.Now.Ticks);

            DrawMainGrid();

            ResetNodeGroupUpdaters();
        }

        void ResetNodeGroupUpdaters()
        {
            if (!IsDirty)
                return;

            _nodeGroupUpdater = NodeGroupUpdaterTorus.ForSquareTorus
            (
                gain: 0.095f,
                step: (float)StepSizeSliderVm.Value,
                squareSize: SquareSize,
                alpha: (float)AlphaSliderVm.Value,
                beta: (float)BetaSliderVm.Value,
                use8Way: Use8Way
            );

            Clean();
        }

        #endregion

        #region Binding variables

        private bool _use8Way;
        public bool Use8Way
        {
            get { return _use8Way; }
            set
            {
                _use8Way = value;
                _isDirty = true;
                OnPropertyChanged("Use8Way");
            }
        }


        private int _generation;
        public int Generation
        {
            get { return _generation; }
            set
            {
                _generation = value;
                OnPropertyChanged("Generation");
            }
        }

        private double _activity;
        public double Activity
        {
            get { return _activity; }
            set
            {
                _activity = value;
                OnPropertyChanged("Activity");
            }
        }

        public TimeSpan ElapsedTime
        {
            get { return _stopwatch.Elapsed; }
        }

        private IReadOnlyList<GraphicsInfo> _gridGraphicsInfo = new List<GraphicsInfo>();
        public IReadOnlyList<GraphicsInfo> GridGraphicsInfo
        {
            get { return _gridGraphicsInfo; }
            set
            {
                _gridGraphicsInfo = value;
                OnPropertyChanged("GridGraphicsInfo");
            }
        }

        private readonly ImageLegendVm _imageLegendVm;
        public ImageLegendVm ImageLegendVm
        {
            get { return _imageLegendVm; }
        }

        void OnPixelsChanged(Color[,] pixels)
        {
            DrawMainGrid();
        }

        private readonly SliderVm _alphaSliderVm;
        public SliderVm AlphaSliderVm
        {
            get { return _alphaSliderVm; }
        }

        private readonly SliderVm _stepSizeSliderVm;
        public SliderVm StepSizeSliderVm
        {
            get { return _stepSizeSliderVm; }
        }

        private readonly SliderVm _betaSliderVm;
        public SliderVm BetaSliderVm
        {
            get { return _betaSliderVm; }
        }

        #endregion

    }
}
