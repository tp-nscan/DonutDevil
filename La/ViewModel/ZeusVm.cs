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
    public class ZeusVm : NotifyPropertyChanged, IMainContentVm
    {
        public ZeusVm(Zeus zeus)
        {
            Zeus = zeus;
            ZeusParamsVm = new ZeusParamsVm();
            WaffleHistoriesVm = new WaffleHistoriesVm(
                WaffleHistBuilder.InitHistories
                    (
                        arrayLength: Zeus.GroupCount,
                        ensembleSize: Zeus.EnsembleCount,
                        targetLength: 500
                    ),
                    "C"
                );

            IndexSelectorVm = new IndexSelectorVm(Enumerable.Range(0, zeus.EnsembleCount));
            DisplayFrequencySliderVm = new SliderVm(RealInterval.Make(1, 49), 2, "0")
            { Title = "Display Frequency", Value = 10 };

            _legendVm = new LinearLegendVm();
            _legendVm.OnLegendVmChanged.Subscribe(lvm => UpdateUi());
        }

        private IndexSelectorVm _indexSelectorVm;
        public IndexSelectorVm IndexSelectorVm
        {
            get { return _indexSelectorVm; }
            set
            {
                _indexSelectorVm = value;
                _indexSelectorVm.OnSelectionChanged.Subscribe(
                        ivm => ResetRCommand.Execute(null)
                    );
            }
        }

        public ZeusParamsVm ZeusParamsVm { get; private set; }

        private IDisposable _whvmSub;
        private WaffleHistoriesVm _waffleHistoriesVm;
        public WaffleHistoriesVm WaffleHistoriesVm
        {
            get { return _waffleHistoriesVm; }
            set
            {
                _whvmSub?.Dispose();
                _waffleHistoriesVm = value;
                _whvmSub = _waffleHistoriesVm.OnArrayHistVmChanged
                            .Subscribe(lvm => UpdateUi());
                OnPropertyChanged("WaffleHistoriesVm");
            }
        }

        public Zeus Zeus { get; private set; }

        public Wng Wng { get; private set; }

        public string Generation
        {
            get { return _generation; }
            set
            {
                _generation = value;
                OnPropertyChanged("Generation");
            }
        }

        #region Navigation

        public IEntityRepo EntityRepo { get; }

        public MainContentType MainContentType => MainContentType.Zeus;

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

        #region UpdateCommand

        RelayCommand _updateCommand;
        public ICommand UpdateCommand
        {
            get
            {
                return _updateCommand ?? (
                    _updateCommand = new RelayCommand(
                        param => DoUpdate(),
                        param => CanUpdate()
                    ));
            }
        }

        private async void DoUpdate()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            IsRunning = true;
            await Task.Run(() =>
            {
                for (var i = 1; i < (int)DisplayFrequencySliderVm.Value + 1; i++)
                {
                    Wng = Wng.Update();
                    Application.Current.Dispatcher.Invoke
                    (
                        () => Generation = i.ToString(),
                        DispatcherPriority.ApplicationIdle
                    );
                }

            },
                cancellationToken: _cancellationTokenSource.Token
            );
            //WaffleHistoriesVm = WaffleHistoriesVm.Update(Wng, Zeus);
            UpdateUi();
            IsRunning = false;
        }

        bool CanUpdate()
        {
            return (!_isRunning) && (Wng != null);
        }

        #endregion // UpdateCommand

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

        private void DoLearn()
        {
            var newWng = Wng.Learn((float)ZeusParamsVm.LearnRateVm.CurVal);
            //Waffle = ZeusBuilder.UpdateFromWng(Waffle, newWng);
            //Wng = ZeusBuilder.ResetC(zeus: Waffle, wng: Wng);
            UpdateUi();
        }

        bool CanLearn()
        {
            return (!_isRunning) && (Wng != null);
        }

        #endregion // LearnCommand

        #region ResetAllCommand

        RelayCommand _resetAllCommand;
        public ICommand ResetAllCommand
        {
            get
            {
                return _resetAllCommand ?? (
                    _resetAllCommand = new RelayCommand(
                        param => DoResetAll(),
                        param => CanResetAll()
                    ));
            }
        }

        private void DoResetAll()
        {
            //Wng = ZeusBuilder.CreateWng(
            //     glauberRadius: ZeusParamsVm.GlauberRadiusVm.CurVal,
            //     pSig: ZeusParamsVm.PSigVm.CurVal,
            //     sSig: ZeusParamsVm.SSigVm.CurVal,
            //     cPp: (float)ZeusParamsVm.CPpVm.CurVal,
            //     pNoiseLevel: (float)ZeusParamsVm.PNoiseLevelVm.CurVal,
            //     sNoiseLevel: (float)ZeusParamsVm.SNoiseLevelVm.CurVal,
            //     cSs: (float)ZeusParamsVm.CSsVm.CurVal,
            //     cRp: (float)ZeusParamsVm.CRpVm.CurVal,
            //     cPs: (float)ZeusParamsVm.CPsVm.CurVal,
            //     rIndex: IndexSelectorVm.IndexVm.Index,
            //     sseed: ZeusParamsVm.SSeedVm.CurVal,
            //     pseed: ZeusParamsVm.PSeedVm.CurVal,
            //     zeus: Waffle
            //  ).Value;

            UpdateUi();
        }

        bool CanResetAll()
        {
            return !_isRunning;
        }

        #endregion // ResetAllCommand

        #region ResetPCommand

        RelayCommand _resetPCommand;
        public ICommand ResetPCommand
        {
            get
            {
                return _resetPCommand ?? (
                    _resetPCommand = new RelayCommand(
                        param => DoResetP(),
                        param => CanResetP()
                    ));
            }
        }

        private void DoResetP()
        {
            //Wng = ZeusBuilder.ResetP(
            //     pSig: ZeusParamsVm.PSigVm.CurVal,
            //     pseed: ZeusParamsVm.PSeedVm.CurVal,
            //     wng: Wng
            //  );

            UpdateUi();
        }

        bool CanResetP()
        {
            return (!_isRunning) && (Wng != null);
        }

        #endregion // ResetPCommand

        #region ResetRCommand

        RelayCommand _resetRCommand;
        public ICommand ResetRCommand
        {
            get
            {
                return _resetRCommand ?? (
                    _resetRCommand = new RelayCommand(
                        param => DoResetR(),
                        param => CanResetR()
                    ));
            }
        }

        private void DoResetR()
        {
            //Wng = ZeusBuilder.ResetR(
            //     zeus: Waffle,
            //     memDex: IndexSelectorVm.IndexVm.Index,
            //     wng: Wng
            //  );

            UpdateUi();
        }

        bool CanResetR()
        {
            return (!_isRunning) && (Wng != null);
        }

        #endregion // ResetRCommand

        #region ResetSCommand

        RelayCommand _resetSCommand;
        public ICommand ResetSCommand
        {
            get
            {
                return _resetSCommand ?? (
                    _resetSCommand = new RelayCommand(
                        param => DoResetS(),
                        param => CanResetS()
                    ));
            }
        }

        private void DoResetS()
        {
            //Wng = ZeusBuilder.ResetS(
            //     sSig: ZeusParamsVm.SSigVm.CurVal,
            //     sseed: ZeusParamsVm.SSeedVm.CurVal,
            //     wng: Wng
            //  );

            UpdateUi();
        }

        bool CanResetS()
        {
            return (!_isRunning) && (Wng != null);
        }

        #endregion // ResetSCommand

        #region  ChangeParamsCommand

        RelayCommand _changeParamsCommand;
        public ICommand ChangeParamsCommand
        {
            get
            {
                return _changeParamsCommand ?? (
                    _changeParamsCommand = new RelayCommand(
                        param => DochangeParams(),
                        param => CanchangeParams()
                    ));
            }
        }

        private void DochangeParams()
        {
            Wng = Wng.NewPrams(
                    cPp: (float)ZeusParamsVm.CPpVm.CurVal,
                    cSs: (float)ZeusParamsVm.CSsVm.CurVal,
                    cRp: (float)ZeusParamsVm.CRpVm.CurVal,
                    cPs: (float)ZeusParamsVm.CPsVm.CurVal
                );
        }

        bool CanchangeParams()
        {
            return (!_isRunning) && (Wng != null);
        }

        #endregion // ChangeParamsCommand

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
            MainGridVm = new WbRollingGridVm(
                imageWidth: 1000,
                imageHeight: 1000,
                cellDimX: Zeus.GroupCount,
                cellDimY: 500
            );

            var cellColors = WaffleHistoriesVm.ArrayHistVm.GetD2Vals.Select(
                (v, i) => new D2Val<Color>
                            (
                                x: v.X,
                                y: v.Y,
                                val: LegendVm.ColorFor1D((float)(v.Val / 2.0 + 0.5))
                            )
                ).ToList();

            if (Wng == null) return;
           // WngGvVm = new WngGvVm(Wng, Zeus, IndexSelectorVm.IndexVm.Index);
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
        private string _generation;

        public string ElapsedTime =>
            $"{_stopwatch.Elapsed.Hours.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Minutes.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Seconds.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Milliseconds.ToString("000")}";

    }
}