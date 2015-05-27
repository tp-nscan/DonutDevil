using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DonutDevilControls.ViewModel.Common;
using La.Model;
using MathLib.Intervals;
using WpfUtils;

namespace La.ViewModel
{
    public class NetworkVm : NotifyPropertyChanged, IMainWindowVm
    {
        public NetworkVm(INetwork network)
        {
            Network = network;
            DisplayFrequencySliderVm = new SliderVm(RealInterval.Make(1, 49), 2, "0") { Title = "Display Frequency", Value = 10 };
        }

        #region IMainWindowVm

        public MainWindowType MainWindowType => MainWindowType.Network;

        private readonly Subject<IMainWindowVm> _mainWindowTypehanged = new Subject<IMainWindowVm>();
        public IObservable<IMainWindowVm> OnMainWindowTypeChanged => _mainWindowTypehanged;

        #endregion

        #region NetworkVm base impl

        INetwork Network { get; }

        public int Generation => Network?.Generation ?? 0;

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



        #endregion

    }
}
