using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DonutDevilControls.ViewModel.Common;
using LibNode;
using MathLib.Intervals;
using MathLib.NumericTypes;
using WpfUtils;
using WpfUtils.ViewModels.Graphics;

namespace La.ViewModel
{
    public class WhatVm : NotifyPropertyChanged, IMainContentVm
    {
        public WhatVm(Wng wng)
        {
            Wng = wng;
            DisplayFrequencySliderVm = new SliderVm(RealInterval.Make(1, 49), 2, "0") { Title = "Display Frequency", Value = 10 };
            RadiusSliderVm = new SliderVm(RealInterval.Make(1, 40), 1, "0") { Title = "Radius", Value = 10 };
            FrequencySliderVm = new SliderVm(RealInterval.Make(0.0, 3), 0.015, "0.000") { Title = "Frequency * Pi / Radius", Value = 0.5 };
            DecaySliderVm = new SliderVm(RealInterval.Make(0, 1), 0.005, "0.000") { Title = "Decay", Value = 0.5 };

            RadiusSliderVm.OnSliderVmChanged.Subscribe(v => UpdateUi());
            FrequencySliderVm.OnSliderVmChanged.Subscribe(v => UpdateUi());
            DecaySliderVm.OnSliderVmChanged.Subscribe(v => UpdateUi());

            _mainGridVm = new WbUniformGridVm2(1024, 1024);
        }

        public Wng Wng { get; private set; }


        public string Generation => Wng?.Iteration.ToString() ?? "-";

        #region Navigation

        private IEntityRepo _entityRepo;
        public IEntityRepo EntityRepo
        {
            get { return _entityRepo; }
        }

        public MainContentType MainContentType => MainContentType.What;


        private readonly Subject<IMainContentVm> _mainWindowTypehanged = new Subject<IMainContentVm>();
        public IObservable<IMainContentVm> OnMainWindowTypeChanged => _mainWindowTypehanged;

        #endregion

        public int GridStride => (int)(RadiusSliderVm.Value * 2 + 1);

        public int Radius => (int)RadiusSliderVm.Value;

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

        }

        private WbUniformGridVm2 _mainGridVm;
        public WbUniformGridVm2 MainGridVm
        {
            get { return _mainGridVm; }
            set
            {
                _mainGridVm = value;
                OnPropertyChanged("MainGridVm");
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
