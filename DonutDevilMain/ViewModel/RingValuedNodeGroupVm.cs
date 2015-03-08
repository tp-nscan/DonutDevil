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
using MathLib.NumericTypes;
using NodeLib;
using WpfUtils;
using WpfUtils.Utils;
using WpfUtils.ViewModels.Graphics;
using WpfUtils.Views.Graphics;

namespace DonutDevilMain.ViewModel
{
    public class RingValuedNodeGroupVm : NotifyPropertyChanged
    {
        private const int SquareSize = 256;
        private const int Colorsteps = 512;
        private const int HistogramBins = 100;


        public RingValuedNodeGroupVm()
        {
            _nodeGroupColorSequence = ColorSequence.Quadrupolar(Colors.Red, Colors.Orange, Colors.Green, Colors.Blue, Colorsteps);

            _histogramColorSequence = Colors.White.ToUniformColorSequence(Colorsteps);

            _alphaSliderVm = new SliderVm(RealInterval.Make(0, 0.999), 0.02, "0.00") {Title = "Alpha"};
            _betaSliderVm = new SliderVm(RealInterval.Make(0, 0.999), 0.02, "0.00") { Title = "Beta" };
            _stepSizeSliderVm = new SliderVm(RealInterval.Make(0, 0.5), 0.002, "0.0000") { Title = "Step" };
            _wbUniformGridVm = new WbUniformGridVm(SquareSize, SquareSize, f => _nodeGroupColorSequence.ToUnitColor(f));

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

        public bool IsDirty
        {
            get { return _stepSizeSliderVm.IsDirty || _alphaSliderVm.IsDirty || _betaSliderVm.IsDirty; }

        }

        public void Clean()
        {
            _stepSizeSliderVm.IsDirty = false;
            _alphaSliderVm.IsDirty = false;
            _betaSliderVm.IsDirty = false;
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

            HistogramVm = new Plot1DVm
            {
                Title = "Node frequencies",
                MinValue = -0.0,
                MaxValue = 1.0,
                GraphicsInfos = histogram.Select(
                    (bin, index) => new PlotPoint
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
            WbUniformGridVm.AddValues(nodeGroup.Values.Select((v,i)=> new D2Val<float>(i/SquareSize, i%SquareSize, v)));
        }

        private WbUniformGridVm _wbUniformGridVm;
        public WbUniformGridVm WbUniformGridVm
        {
            get { return _wbUniformGridVm; }
        }

        void InitializeRun()
        {
            _nodeGroup = NodeGroup.RandomNodeGroup(SquareSize * SquareSize, DateTime.Now.Millisecond);

            MakeHistogram();

            SetupColorLegend();

            ResetNodeGroupUpdaters();

            DrawMainGrid(_nodeGroup);
        }


        void SetupColorLegend()
        {
            var x = 0;

            ColorLegendVm = new Plot1DVm
            {
                Title = "Node values",
                MinValue = -0.0,
                MaxValue = 1.0,
                GraphicsInfos = _nodeGroupColorSequence
                                    .Colors
                                    .Select(
                                    c => new PlotPoint
                                        (
                                            x: x++,
                                            y: 0,
                                            color: c
                                    )).ToList()
            };
        }


        void ResetNodeGroupUpdaters()
        {
            if (!IsDirty)
                return;

            _nodeGroupUpdater = NodeGroupUpdaterRing.ForSquareTorus
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


        private Plot1DVm _colorLegendVm;
        public Plot1DVm ColorLegendVm
        {
            get { return _colorLegendVm; }
            set
            {
                _colorLegendVm = value;
                OnPropertyChanged("ColorLegendVm");
            }
        }

        private Plot1DVm _histogramVm;
        public Plot1DVm HistogramVm
        {
            get { return _histogramVm; }
            set
            {
                _histogramVm = value;
                OnPropertyChanged("HistogramVm");
            }
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
