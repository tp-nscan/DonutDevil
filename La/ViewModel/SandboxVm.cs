using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DonutDevilControls.ViewModel.Common;
using DonutDevilControls.ViewModel.Design.Legend;
using DonutDevilControls.ViewModel.Legend;
using MathLib.Intervals;
using MathLib.NumericTypes;
using WpfUtils;
using WpfUtils.ViewModels.Graphics;

namespace La.ViewModel
{
    public class SandboxVm : NotifyPropertyChanged, IMainWindowVm
    {
        public SandboxVm()
        {
            DisplayFrequencySliderVm = new SliderVm(RealInterval.Make(1, 49), 2, "0") { Title = "Display Frequency", Value = 10 };
            RadiusSliderVm = new SliderVm(RealInterval.Make(1, 40), 1, "0") { Title = "Radius", Value = 10 };
            FrequencySliderVm = new SliderVm(RealInterval.Make(0.0, 3), 0.015, "0.000") { Title = "Frequency * Pi / Radius", Value = 0.5 };
            DecaySliderVm = new SliderVm(RealInterval.Make(0, 1), 0.005, "0.000") { Title = "Decay", Value = 0.5 };

            RadiusSliderVm.OnSliderVmChanged.Subscribe(v => UpdateUi());
            FrequencySliderVm.OnSliderVmChanged.Subscribe(v => UpdateUi());
            DecaySliderVm.OnSliderVmChanged.Subscribe(v => UpdateUi());

            _mainGridVm = new WbUniformGridVm(1024, 1024);
            _histogramVm = new DesignLinearHistogramVm();
            _legendVm = new LinearLegendVm();
            _legendVm.OnLegendVmChanged.Subscribe(LegendChangedHandler);

            //var p = LibNode.Parameters.TestBoolParam;
            //var up = p.Value;
            //var ib = up.IsBool;
        }

        #region Navigation

        public MainWindowType MainWindowType => MainWindowType.Sandbox;

        private readonly Subject<IMainWindowVm> _mainWindowTypehanged = new Subject<IMainWindowVm>();
        public IObservable<IMainWindowVm> OnMainWindowTypeChanged => _mainWindowTypehanged;

        #endregion

        public int GridStride => (int)(RadiusSliderVm.Value * 2 + 1);

        public int Radius => (int) RadiusSliderVm.Value;

        public double Decay => DecaySliderVm.Value;

        public double Frequency => FrequencySliderVm.Value;

        public SliderVm DisplayFrequencySliderVm { get; }

        public SliderVm RadiusSliderVm { get; }

        public SliderVm FrequencySliderVm { get; }

        public SliderVm DecaySliderVm { get; }

        #region local vars

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private bool _isRunning;

        #endregion

        #region UpdateNetworkCommand

        RelayCommand _updateNetworkCommand;
        public ICommand UpdateNetworkCommand
        {
            get
            {
                return _updateNetworkCommand ?? (
                    _updateNetworkCommand = new RelayCommand(
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
                    //if (ParamSetEditorVm.IsDirty)
                    //{
                    //    Network = Network.UpdateParams(ParamSetEditorVm.LatestParameters.ToDictionary(p => p.Name));
                    //    ParamSetEditorVm.IsDirty = false;
                    //}

                   // Network = Network.UpdateNodeGroup();

                    if (_cancellationTokenSource.IsCancellationRequested)
                    {
                        _isRunning = false;
                        _stopwatch.Stop();
                        CommandManager.InvalidateRequerySuggested();
                    }

                    if (i % (int)DisplayFrequencySliderVm.Value == 0)
                    {
                        Application.Current.Dispatcher.Invoke
                            (
                                UpdateUi,
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
                return _stopUpdateNetworkCommand ?? (_stopUpdateNetworkCommand = new RelayCommand(
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

        #region GoToMenuCommand

        RelayCommand _goToMenuCommand;
        public ICommand GoToMenuCommand
        {
            get
            {
                return _goToMenuCommand ?? (_goToMenuCommand = new RelayCommand(
                            param => DoGoToMenu(),
                            param => CanGoToMenu()
                        ));
                }
        }

        private void DoGoToMenu()
        {
            _mainWindowTypehanged.OnNext(new MenuVm());
        }

        bool CanGoToMenu()
        {
            return true;
        }

        #endregion // GoToMenuCommand

        void UpdateUi()
        {
            var n = NeighborhoodExt.CircularGlauber(Radius, Frequency * Math.PI / Radius, Decay);
            var valueList = n.ToReadingOrder.Select(v => v).ToList();

            var cellColors = valueList.Select(
                (v, i) => new D2Val<Color>
                            (
                                x: i % (GridStride),
                                y: i / (GridStride),
                                value: LegendVm.ColorFor1D((float)(v / 2.0 + 0.5))
                            )
                ).ToList();

            MainGridVm.AddValues(cellColors);
            Total = valueList.Sum(v => v);
            AbsTotal = valueList.Sum(v => Math.Abs(v));
            _histogramVm.MakeHistogram(valueList.Select(cc => (float)(cc / 2.0 + 0.5)));
        }

        private WbUniformGridVm _mainGridVm;
        public WbUniformGridVm MainGridVm
        {
            get { return _mainGridVm; }
            set
            {
                _mainGridVm = value;
                OnPropertyChanged("MainGridVm");
            }
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

        void LegendChangedHandler(ILegendVm legendVm)
        {
            switch (_histogramVm.DisplaySpaceType)
            {
                case LegendType.Ring:
                    _histogramVm.DrawLegend(legendVm.ColorFor1D);
                    break;
                case LegendType.Torus:
                    _histogramVm.DrawLegend(legendVm.ColorFor2D);
                    break;
                case LegendType.Interval:
                    _histogramVm.DrawLegend(legendVm.ColorForInterval);
                    break;
                default:
                    throw new Exception("Unhandled LegendType");
            }

            UpdateUi();
        }

        private LinearHistogramVm _histogramVm;
        public LinearHistogramVm HistogramVm
        {
            get { return _histogramVm; }
            set
            {
                _histogramVm = value;
                OnPropertyChanged("HistogramVm");
            }
        }

        private double _total;
        public double Total
        {
            get { return _total; }
            set
            {
                _total = value;
                OnPropertyChanged("Total");
            }
        }

        private double _absTotal;
        public double AbsTotal
        {
            get { return _absTotal; }
            set
            {
                _absTotal = value;
                OnPropertyChanged("AbsTotal");
            }
        }

        private readonly Stopwatch _stopwatch = new Stopwatch();
        public string ElapsedTime => 
            $"{_stopwatch.Elapsed.Hours.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Minutes.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Seconds.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Milliseconds.ToString("000")}";
    }
}
