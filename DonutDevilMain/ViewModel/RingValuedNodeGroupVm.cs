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
using DonutDevilControls.ViewModel.Legend;
using MathLib.Intervals;
using MathLib.NumericTypes;
using NodeLib;
using NodeLib.Updaters;
using WpfUtils;
using WpfUtils.ViewModels.Graphics;

namespace DonutDevilMain.ViewModel
{
    public class RingValuedNodeGroupVm : NotifyPropertyChanged
    {
        private const int SquareSize = 256;
        public RingValuedNodeGroupVm()
        {
            _legendVm = new RingLegendVm();
            _legendVm.OnLegendVmChanged.Subscribe(RespondToLegendChange);

            _alphaSliderVm = new SliderVm(RealInterval.Make(0, 0.999), 0.02, "0.00") {Title = "Alpha"};
            _betaSliderVm = new SliderVm(RealInterval.Make(0, 0.999), 0.02, "0.00") { Title = "Beta" };
            _stepSizeSliderVm = new SliderVm(RealInterval.Make(0, 0.5), 0.002, "0.0000") { Title = "Step" };
            _displayFrequencySliderVm = new SliderVm(RealInterval.Make(1, 49), 2, "0") { Title = "Display Frequency", Value = 10 };

            _ringHistogramVm = new RingHistogramVm
                (
                  title: "Cell values"
                );

            _wbUniformGridVm = new WbUniformGridVm(SquareSize, SquareSize);

            InitializeRun();
        }

        void RespondToLegendChange(ILegendVm legendVm)
        {
            RingHistogramVm.DrawLegend(f=>LegendVm.ColorForRing(f));

            DrawMainNetwork(_nodeGroup);
        }


        #region local vars

        private readonly Stopwatch _stopwatch = new Stopwatch();

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


        #region UpdateNetworkCommand

        RelayCommand _updateNetworkCommand;
        public ICommand UpdateNetworkCommand
        {
            get
            {
                return _updateNetworkCommand ?? (_updateNetworkCommand = new RelayCommand(
                    param => DoUpdateNetwork(),
                    param => CanUpdateNetwork()
                    ));
            }
        }

        private async void DoUpdateNetwork()
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
                                    DrawMainNetwork(_nodeGroup);
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

        bool CanUpdateNetwork()
        {
            return !_isRunning;
        }

        #endregion // UpdateNetworkCommand


        #region StopUpdateNetworkCommand

        RelayCommand _stopUpdateNetworkCommand;
        public ICommand StopUpdateNetworkCommand
        {
            get
            {
                return _stopUpdateNetworkCommand ?? (
                    _stopUpdateNetworkCommand = new RelayCommand(
                        param => DoCancelUpdateNetwork(),
                        param => CanCancelUpdateNetwork()
                    ));
            }
        }

        private void DoCancelUpdateNetwork()
        {
            _cancellationTokenSource.Cancel();
        }

        bool CanCancelUpdateNetwork()
        {
            return _isRunning;
        }

        #endregion // StopUpdateNetworkCommand


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
            RingHistogramVm.MakeHistogram(_nodeGroup.Values);
        }

        void UpdateBindingProperties()
        {
            OnPropertyChanged("Generation");
            OnPropertyChanged("ElapsedTime");
        }

        void DrawMainNetwork(INodeGroup nodeGroup)
        {
            WbUniformGridVm.AddValues(nodeGroup.Values.Select((v,i)=> 
                new D2Val<Color>(i/SquareSize, i%SquareSize,  LegendVm.ColorForRing(v) )));
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

            RespondToLegendChange(LegendVm);
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

        private ILegendVm _legendVm;
        public ILegendVm LegendVm
        {
            get { return _legendVm; }
            set
            {
                _legendVm = value;
                OnPropertyChanged("LegendVm");
            }
        }

        #endregion

    }
}
