using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows.Input;
using DonutDevilControls.ViewModel.Common;
using La.ViewModel.Design;
using LibNode;
using MathLib.Intervals;
using WpfUtils;

namespace La.ViewModel
{
    public class NetworkVm : NotifyPropertyChanged, IMainContentVm
    {
        public NetworkVm(ISym network)
        {
            Network = network;
            DisplayFrequencySliderVm = new SliderVm(RealInterval.Make(1, 49), 2, "0") { Title = "Display Frequency", Value = 10 };
            ZeusSnapVm  = new DesignZeusSnapVm();
        }

        public ZeusSnapVm ZeusSnapVm
        {
            get { return _zeusSnapVm; }
            set
            {
                _zeusSnapVm = value;
                OnPropertyChanged("ZeusSnapVm");
            }
        }

        #region IMainWindowVm

        public IEntityRepo EntityRepo
        {
            get { return _entityRepo; }
        }

        public MainContentType MainContentType => MainContentType.Network;
        private readonly Subject<IMainContentVm> _mainWindowTypehanged = new Subject<IMainContentVm>();
        public IObservable<IMainContentVm> OnMainWindowTypeChanged => _mainWindowTypehanged;

        #endregion

        #region NetworkVm base impl

        ISym Network { get; }

        public int Generation => Network?.Iteration ?? 0;

        public double DisplayFrequency => DisplayFrequencySliderVm.Value;

        public SliderVm DisplayFrequencySliderVm { get; }

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private bool _isRunning;

        private readonly Stopwatch _stopwatch = new Stopwatch();
        public string ElapsedTime =>
            $"{_stopwatch.Elapsed.Hours.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Minutes.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Seconds.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Milliseconds.ToString("000")}";

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
        const int Seed = 1238;
        const int DGpSz = 256;
        const int DMemSz = 16;
        const int GlauberRadius = 5;

        private async void DoUpdateNetwork()
        {

            ZeusSnapVm = new ZeusSnapVm(
                ZeusBuilders.DesignAthenaTr(seed: DateTime.Now.Millisecond,
                    ngSize: DGpSz, memSize: DMemSz,
                    ppSig: 0.3f,
                    glauberRadius: GlauberRadius).Value,
                "yo",
                100
                );
        }


        //_cancellationTokenSource = new CancellationTokenSource();
        //_isRunning = true;

        //await Task.Run(() =>
        //    {
        //        _stopwatch.Start();

        //        for (var i = 0; _isRunning; i++)
        //        {

        //            //if (ParamSetEditorVm.IsDirty)
        //            //{
        //            //    Network = Network.UpdateParams(ParamSetEditorVm.LatestParameters.ToDictionary(p => p.Name));
        //            //    ParamSetEditorVm.IsDirty = false;
        //            //}

        //            // Network = Network.UpdateNodeGroup();

        //            if (_cancellationTokenSource.IsCancellationRequested)
        //            {
        //                _isRunning = false;
        //                _stopwatch.Stop();
        //                CommandManager.InvalidateRequerySuggested();
        //            }

        //            if (i % (int)DisplayFrequencySliderVm.Value == 0)
        //            {
        //                Application.Current.Dispatcher.Invoke
        //                    (
        //                        UpdateUi,
        //                        DispatcherPriority.Background
        //                    );
        //            }
        //        }
        //    },
        //        cancellationToken: _cancellationTokenSource.Token
        //    );
        //}

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
        private IEntityRepo _entityRepo;
        private ZeusSnapVm _zeusSnapVm;

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
            this.OnPropertyChanged("ElapsedTime");
        }



        #endregion

    }
}
