using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using WpfUtils.Utils;

namespace DonutDevilMain.ViewModel
{
    public class RingValuedNodeGroupVm : NotifyPropertyChanged
    {
        private const int SquareSize = 100;
        private const int Colorsteps = 512;
        private const int HistogramBins = 100;


        public RingValuedNodeGroupVm()
        {
            _rangeSliderVm = new SliderVm(RealInterval.Make(0, 0.5), 0.02, "0.00") {Title = "Range"};
            _stepSizeSliderVm = new SliderVm(RealInterval.Make(0, 0.5), 0.002, "0.0000") { Title = "Step" };
            InitializeRun();
        }


        #region local vars

        private IColorSequence _nodeGroupColorSequence;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private IColorSequence _histogramColorSequence;

        private INodeGroup _nodeGroup;

        private INodeGroupUpdater _nodeGroupUpdater;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool _isRunning;

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
                try
                {
                    _stopwatch.Start();
                    for (var i = 0; _isRunning; i++)
                    {
                        _nodeGroup = _nodeGroupUpdater.Update(_nodeGroup);
                        _generation++;

                        if (_cancellationTokenSource.IsCancellationRequested)
                        {
                            _isRunning = false;
                            _stopwatch.Stop();
                        }

                        if (i % 10 == 0)
                        {
                            Application.Current.Dispatcher.Invoke
                                (
                                    () =>
                                    {
                                        ResetNodeGroupUpdaters();
                                        UpdateBindingProperties();
                                        MakeHistogram();
                                        DrawMainGrid(_nodeGroup);
                                        CommandManager.InvalidateRequerySuggested();
                                    },
                                    DispatcherPriority.Background
                                );
                        }

                    }
                }

                catch (Exception)
                {
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

        void MakeHistogram()
        {
            var histogram =
                _nodeGroup.Values.ToHistogram
                (
                    bins: RealInterval.UnitRange.SplitToEvenIntervals(HistogramBins).ToList(),
                    valuatorFunc: n => n
                );

            var colorDexer = (Colorsteps - 1) / histogram.Max(t => Math.Sqrt(t.Item2.Item));

            HistogramVm = new ColorLegendVm
            {
                Title = "Node frequencies",
                MinValue = -0.0,
                MaxValue = 1.0,
                GraphicsInfos = histogram.Select(
                    (bin, index) => new GraphicsInfo
                        (
                                x: index,
                                y: 0,
                                color: _histogramColorSequence.Colors[(int)(colorDexer * Math.Sqrt(bin.Item2.Item))]
                        )).ToList()
            };

        }

        void UpdateBindingProperties()
        {
            OnPropertyChanged("Generation");
            OnPropertyChanged("ElapsedTime");
        }

        void DrawMainGrid(INodeGroup nodeGroup)
        {
            GridGraphicsInfo =
                nodeGroup.Nodes().Select(n =>
                    new GraphicsInfo(
                        x: n.GroupIndex / SquareSize,
                        y: n.GroupIndex % SquareSize,
                        color: _nodeGroupColorSequence.ToUnitColor(n.Value)))
                    .ToList();
        }

        void InitializeRun()
        {
            _nodeGroupColorSequence = ColorSequence.Quadrupolar(Colors.Red, Colors.Orange, Colors.Green, Colors.Blue,
                Colorsteps);

            _histogramColorSequence = Colors.White.ToUniformColorSequence(Colorsteps);

            _nodeGroup = NodeGroup.RandomNodeGroup(SquareSize * SquareSize, DateTime.Now.Millisecond);

            MakeHistogram();

            SetupColorLegend();

            ResetNodeGroupUpdaters();

            DrawMainGrid(_nodeGroup);
        }


        void SetupColorLegend()
        {
            var x = 0;

            ColorLegendVm = new ColorLegendVm
            {
                Title = "Node values",
                MinValue = -0.0,
                MaxValue = 1.0,
                GraphicsInfos = _nodeGroupColorSequence
                                    .Colors
                                    .Select(
                                    c => new GraphicsInfo
                                        (
                                            x: x++,
                                            y: 0,
                                            color: c
                                    )).ToList()
            };
        }


        void ResetNodeGroupUpdaters()
        {
            _nodeGroupUpdater = NodeGroupUpdaterRing.ForSquareTorus
            (
                gain: 0.095f,
                step: (float)StepSizeSliderVm.Value,
                squareSize: SquareSize,
                range: (float)RangeSliderVm.Value,
                use8Way: Use8Way
            );
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

        private ColorLegendVm _colorLegendVm;
        public ColorLegendVm ColorLegendVm
        {
            get { return _colorLegendVm; }
            set
            {
                _colorLegendVm = value;
                OnPropertyChanged("ColorLegendVm");
            }
        }

        private ColorLegendVm _histogramVm;
        public ColorLegendVm HistogramVm
        {
            get { return _histogramVm; }
            set
            {
                _histogramVm = value;
                OnPropertyChanged("HistogramVm");
            }
        }

        private readonly SliderVm _rangeSliderVm;
        public SliderVm RangeSliderVm
        {
            get { return _rangeSliderVm; }
        }

        private readonly SliderVm _stepSizeSliderVm;
        public SliderVm StepSizeSliderVm
        {
            get { return _stepSizeSliderVm; }
        }

        private readonly SliderVm _horizontalBiasSliderVm;
        public SliderVm HorizontalBiasSliderVm
        {
            get { return _horizontalBiasSliderVm; }
        }

        private readonly SliderVm _verticalBiasSliderVm;
        public SliderVm VerticalBiasSliderVm
        {
            get { return _verticalBiasSliderVm; }
        }


        #endregion
    }
}
