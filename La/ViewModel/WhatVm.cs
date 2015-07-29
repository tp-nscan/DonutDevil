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
using DonutDevilControls.ViewModel.Legend;
using La.ViewModel.Pram;
using LibNode;
using MathLib.Intervals;
using WpfUtils;
using WpfUtils.ViewModels.Graphics;

namespace La.ViewModel
{
    public class WhatVm : NotifyPropertyChanged, IMainContentVm
    {
        public WhatVm(Waffle waffle)
        {
            Waffle = waffle;
            WaffleParamsVm = new WaffleParamsVm();
            WaffleHistoriesVm = new WaffleHistoriesVm(
                WngBuilder.InitHistories
                    (
                        arrayLength: Waffle.GroupCount,
                        ensembleSize: Waffle.EnsembleCount,
                        targetLength: 200
                    ),
                    "C"
                );

            IndexSelectorVm = new IndexSelectorVm(Enumerable.Range(0, waffle.EnsembleCount));
            DisplayFrequencySliderVm = new SliderVm(RealInterval.Make(1, 49), 2, "0")
            { Title = "Display Frequency", Value = 10 };

            _legendVm = new LinearLegendVm();
            _legendVm.OnLegendVmChanged.Subscribe(lvm => UpdateUi());
        }

        public IndexSelectorVm IndexSelectorVm { get; private set; }

        public WaffleParamsVm WaffleParamsVm { get; private set; }

        private IDisposable _whvmSub;
        private WaffleHistoriesVm _waffleHistoriesVm;
        public WaffleHistoriesVm WaffleHistoriesVm
        {
            get { return _waffleHistoriesVm; }
            set
            {
                _whvmSub?.Dispose();
                _waffleHistoriesVm = value;
                _whvmSub = _waffleHistoriesVm.OnArrayHistVmChanged.Subscribe(lvm => UpdateUi());
                OnPropertyChanged("WaffleHistoriesVm");
            }
        }

        public Waffle Waffle { get; private set; }

        public Wng Wng { get; private set; }

        public string Generation => Wng?.Iteration.ToString() ?? "-";

        #region Navigation

        public IEntityRepo EntityRepo { get; }

        public MainContentType MainContentType => MainContentType.Waffle;

        private readonly Subject<IMainContentVm> _mainWindowTypeChanged = new Subject<IMainContentVm>();
        public IObservable<IMainContentVm> OnMainWindowTypeChanged => _mainWindowTypeChanged;

        #endregion

        public SliderVm DisplayFrequencySliderVm { get; }

        #region local vars

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                OnPropertyChanged("IsRunning");
            }
        }

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
            IsRunning = true;

            await Task.Run(() =>
            {
                _stopwatch.Start();

                Wng = WaffleBuilder.CreateWng(
                     glauberRadius: WaffleParamsVm.GlauberRadiusVm.CurVal,
                     pSig: WaffleParamsVm.PSigVm.CurVal,
                     sSig: WaffleParamsVm.SSigVm.CurVal,
                     cPp: (float)WaffleParamsVm.CPpVm.CurVal,
                     pNoiseLevel: (float)WaffleParamsVm.PNoiseLevelVm.CurVal,
                     sNoiseLevel: (float)WaffleParamsVm.SNoiseLevelVm.CurVal,
                     cSs: (float)WaffleParamsVm.CSsVm.CurVal,
                     cRp: (float)WaffleParamsVm.CRpVm.CurVal,
                     cPs: (float)WaffleParamsVm.CPsVm.CurVal,
                     rIndex: IndexSelectorVm.IndexVm.Index,
                     seed: WaffleParamsVm.SeedVm.CurVal,
                     waffle: Waffle
                  ).Value;

                for (var i = 1; _isRunning; i++)
                {
                    if (WaffleParamsVm.IsDirty)
                    {
                        Wng = Wng.NewPrams(
                                cPp: (float)WaffleParamsVm.CPpVm.CurVal,
                                cSs: (float)WaffleParamsVm.CSsVm.CurVal,
                                cRp: (float)WaffleParamsVm.CRpVm.CurVal,
                                cPs: (float)WaffleParamsVm.CPsVm.CurVal
                            );
                        WaffleParamsVm.Clean();
                    }

                    var learnMod = i % WaffleParamsVm.LearnFreqVm.CurVal;
                    var learnTime = (learnMod == 0);

                    // ************************
                    if ((WaffleParamsVm.LearnFreqVm.CurVal > 0) &&
                         learnTime
                       )
                    {
                        Wng = Wng.Learn((float)WaffleParamsVm.LearnRateVm.CurVal);
                    }

                    Wng = Wng.Update();
                    // ************************

                    if (_cancellationTokenSource.IsCancellationRequested)
                    {
                        IsRunning = false;
                        _stopwatch.Stop();
                        Application.Current.Dispatcher.Invoke
                        (
                            CommandManager.InvalidateRequerySuggested
                        );
                    }

                    if (i % (int)DisplayFrequencySliderVm.Value == 0)
                    {
                        Application.Current.Dispatcher.Invoke
                            (
                                UpdateUi,
                                DispatcherPriority.ApplicationIdle
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

        #region LearnCommand

        RelayCommand _learnCommand;
        public ICommand LearnCommand
        {
            get
            {
                return _learnCommand ?? (
                        _learnCommand = new RelayCommand(
                            param => DoLearn(),
                            param => CanLearn()
                    ));
            }
        }

        private async void DoLearn()
        {
            var newWng = Wng.Learn((float)WaffleParamsVm.LearnRateVm.CurVal);
            Waffle = WaffleBuilder.UpdateFromWng(Waffle, newWng);
        }

        bool CanLearn()
        {
            return !_isRunning;
        }

        #endregion // LearnCommand

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

        #region GoToMenuCommand

        RelayCommand _goToMenuCommand;
        public ICommand GoToMenuCommand
        {
            get
            {
                return _goToMenuCommand ?? (
                    _goToMenuCommand = new RelayCommand(
                            param => DoGoToMenu(),
                            param => CanGoToMenu()
                        ));
            }
        }

        private void DoGoToMenu()
        {
            _mainWindowTypeChanged.OnNext(new MenuVm());
        }

        bool CanGoToMenu()
        {
            return true;
        }

        #endregion // GoToMenuCommand

        void UpdateUi()
        {
            WaffleHistoriesVm = WaffleHistoriesVm.Update(Wng, Waffle);

            MainGridVm = new WbRollingGridVm(
                imageWidth: 1000,
                imageHeight: 1000,
                cellDimX: WaffleHistoriesVm.ArrayHistVm.ArrayLength,
                cellDimY: 200
            );

            var cellColors = WaffleHistoriesVm.ArrayHistVm.GetD2Vals.Select(
                (v, i) => new D2Val<Color>
                            (
                                x: v.X,
                                y: v.Y,
                                val: LegendVm.ColorFor1D((float)(v.Val / 2.0 + 0.5))
                            )
                ).ToList();

            WngGvVm = new WngGvVm(Wng);
            MainGridVm.AddValues(cellColors);
            OnPropertyChanged("Generation");
        }

        private WbRollingGridVm _mainGridVm;
        public WbRollingGridVm MainGridVm
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

        public WngGvVm WngGvVm
        {
            get { return _wngGvVm; }
            set
            {
                _wngGvVm = value;
                OnPropertyChanged("WngGvVm");
            }
        }

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private WngGvVm _wngGvVm;

        public string ElapsedTime =>
            $"{_stopwatch.Elapsed.Hours.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Minutes.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Seconds.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Milliseconds.ToString("000")}";

    }
}