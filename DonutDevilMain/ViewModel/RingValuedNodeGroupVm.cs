using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DonutDevilControls.ViewModel.Common;
using MathLib;
using MathLib.Intervals;
using MathLib.NumericTypes;
using NodeLib;
using NodeLib.Updaters;
using WpfUtils;
using WpfUtils.Utils;
using WpfUtils.ViewModels.Graphics;

namespace DonutDevilMain.ViewModel
{
    public class RingValuedNodeGroupVm : NotifyPropertyChanged
    {
        private const int SquareSize = 256;
        private const int Colorsteps = 512;

        public RingValuedNodeGroupVm()
        {
            _nodeGroupColorSequence = ColorSequence.Quadrupolar(Colors.Red, Colors.Orange, Colors.Green, Colors.Blue, Colorsteps/4);

            _histogramColorSequence = Colors.White.ToUniformColorSequence(Colorsteps);

            _alphaSliderVm = new SliderVm(RealInterval.Make(0, 0.999), 0.02, "0.00") {Title = "Alpha"};
            _betaSliderVm = new SliderVm(RealInterval.Make(0, 0.999), 0.02, "0.00") { Title = "Beta" };
            _stepSizeSliderVm = new SliderVm(RealInterval.Make(0, 0.5), 0.002, "0.0000") { Title = "Step" };
            _displayFrequencySliderVm = new SliderVm(RealInterval.Make(1, 49), 2, "0") { Title = "Display Frequency", Value = 10 };

            _ringHistogramVm = new RingHistogramVm
                (
                  title: "Cell values",
                  legendColorMap: f => _nodeGroupColorSequence.ToUnitColor(f),
                  histogramColorMap: f => _histogramColorSequence.Colors[(int)f]
                );

            _ringHistogramVm.LegendVm.AddValues(
                    Enumerable.Range(0, Functions.TrigFuncSteps)
                              .Select(i => new D1Val<float>(i, (float)i / Functions.TrigFuncSteps))
                );

            _wbUniformGridVm = new WbUniformGridVm(SquareSize, SquareSize, f => _nodeGroupColorSequence.ToUnitColor(f));

            InitializeRun();
        }


        #region local vars

        private readonly IColorSequence _nodeGroupColorSequence;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private readonly IColorSequence _histogramColorSequence;

        private INodeGroup _nodeGroup;

        private INgUpdater _ngUpdater;

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
                _stopwatch.Start();
                for (var i = 0; _isRunning; i++)
                {
                    _nodeGroup = _ngUpdater.Update(_nodeGroup);

                    if (_cancellationTokenSource.IsCancellationRequested)
                    {
                        _isRunning = false;
                        _stopwatch.Stop();
                    }

                    if (i % (int)DisplayFrequencySliderVm.Value == 0)
                    {
                        Application.Current.Dispatcher.Invoke
                            (
                                () =>
                                {
                                    ResetNgUpdaters();
                                    UpdateBindingProperties();
                                    MakeHistogram();
                                    DrawMainGrid(_nodeGroup);
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
                return _stopUpdateGridCommand ?? (_stopUpdateGridCommand = new RelayCommand(
                    param => DoCancelUpdateGrid(),
                    param => CanCancelUpdateGrid()
                    ));
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
                return _resetCommand ?? (_resetCommand = new RelayCommand(
                    param => DoReset(),
                    param => CanDoReset()
                    ));
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
                    bins: RealInterval.UnitRange.SplitToEvenIntervals(Functions.TrigFuncSteps - 1).ToList(),
                    valuatorFunc: n => n
                );

            var colorDexer = (Colorsteps - 1) / histogram.Max(t => Math.Sqrt(t.Item2.Item));

            RingHistogramVm.HistogramVm.AddValues(
                    histogram.Select((bin, index) => new D1Val<float>(index, (float)(colorDexer * Math.Sqrt(bin.Item2.Item))))
                );
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

        private readonly WbUniformGridVm _wbUniformGridVm;
        public WbUniformGridVm WbUniformGridVm
        {
            get { return _wbUniformGridVm; }
        }

        void InitializeRun()
        {
            _nodeGroup = NodeGroup.RandomNodeGroup(SquareSize * SquareSize, DateTime.Now.Millisecond);

            MakeHistogram();

            ResetNgUpdaters();

            DrawMainGrid(_nodeGroup);
        }


        void ResetNgUpdaters()
        {
            if (!IsDirty)
                return;

            _ngUpdater = NgUpdaterRing.ForSquareTorus
            (
                gain: 0.095f,
                step: (float)StepSizeSliderVm.Value,
                squareSize: SquareSize,
                alpha: (float)AlphaSliderVm.Value,
                beta: (float)BetaSliderVm.Value
            );

            Clean();
        }


        #endregion


        #region Binding variables

        public int Generation
        {
            get { return _nodeGroup.Generation; }
        }

        public TimeSpan ElapsedTime
        {
            get { return _stopwatch.Elapsed; }
        }

        private readonly RingHistogramVm _ringHistogramVm;
        public RingHistogramVm RingHistogramVm
        {
            get { return _ringHistogramVm; }
        }

        private readonly SliderVm _displayFrequencySliderVm;
        public SliderVm DisplayFrequencySliderVm
        {
            get { return _displayFrequencySliderVm; }
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
