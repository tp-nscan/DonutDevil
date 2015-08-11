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
using La.ViewModel.Pram;
using LibNode;
using MathLib.Intervals;
using WpfUtils;

namespace La.ViewModel
{
    public class ZeusVm : NotifyPropertyChanged, IMainContentVm
    {
        const int TargetLength = 350;

        ZeusSnaps _zeusSnaps = new ZeusSnaps();
        private AthenaStageRes _athenaStageRes;

        public ZeusVm(Zeus zeus)
        {
            Zeus = zeus;
            ZeusParamsVm = new ZeusParamsVm();
            AthenaTr = ZeusBuilders.AthenaToTr(ZeusBuilders.CreateRandomAthena(
                    seed: ZeusParamsVm.PSeedVm.CurVal,
                    ngSize: Zeus.GroupCount,
                    pSig: ZeusParamsVm.PSigVm.CurVal,
                    sSig: ZeusParamsVm.SSigVm.CurVal
                ));

            ZeusSnapVm = new ZeusSnapVm(
                    athenaTr: AthenaTr,
                    caption: "Start"
                );

            IndexSelectorVm = new IndexSelectorVm(Enumerable.Range(0, zeus.EnsembleCount));
            DisplayFrequencySliderVm = new SliderVm(RealInterval.Make(1, 49), 2, "0")
                { Title = "Display Frequency", Value = 10 };
        }

        private ZeusSnapVm _zeusSnapVm;
        public ZeusSnapVm ZeusSnapVm
        {
            get { return _zeusSnapVm; }
            set
            {
                _zeusSnapVm = value;
                OnPropertyChanged("ZeusSnapVm");
            }
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

        public Zeus Zeus { get; private set; }

        public AthenaTr AthenaTr { get; private set; }

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

        private readonly Subject<IMainContentVm> _mainWindowTypeChanged 
            = new Subject<IMainContentVm>();
        public IObservable<IMainContentVm> OnMainWindowTypeChanged => 
            _mainWindowTypeChanged;

        #endregion

        public SliderVm DisplayFrequencySliderVm { get; }

        #region local vars

        private CancellationTokenSource _cancellationTokenSource 
            = new CancellationTokenSource();

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
                var randy = new System.Random(123);
                while ( ! _cancellationTokenSource.IsCancellationRequested)
                {
                    AthenaTr = ZeusF.RepAthenaTr(
                        zeus: Zeus,
                        memIndex: IndexSelectorVm.IndexVm.Index,
                        pNoiseL: (float) ZeusParamsVm.pNoiseLVm.CurVal,
                        sNoiseL: (float) ZeusParamsVm.sNoiseLVm.CurVal,
                        seed: randy.Next(),
                        cPp: (float) ZeusParamsVm.CPpVm.CurVal,
                        cSs: (float) ZeusParamsVm.CSsVm.CurVal,
                        cRp: (float) ZeusParamsVm.CRpVm.CurVal,
                        cPs: (float) ZeusParamsVm.CPsVm.CurVal,
                        athena: AthenaTr.Athena,
                        reps: (int) DisplayFrequencySliderVm.Value
                        ).AthenaTr;

                    Application.Current.Dispatcher.Invoke
                    (
                        UpdateUi,
                        DispatcherPriority.ApplicationIdle
                    );
                }

            },
                cancellationToken: _cancellationTokenSource.Token
            );

            IsRunning = false;
            UpdateUi();
        }

        bool CanUpdate()
        {
            return (!_isRunning); //&& (Wng != null);
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
           // var newWng = Wng.Learn((float)ZeusParamsVm.LearnRateVm.CurVal);
            //Waffle = ZeusBuilder.UpdateFromWng(Waffle, newWng);
            //Wng = ZeusBuilder.ResetC(zeus: Waffle, wng: Wng);
            UpdateUi();
        }

        bool CanLearn()
        {
            return (!_isRunning); // && (Wng != null);
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
            AthenaTr = ZeusBuilders.AthenaToTr(
                ZeusBuilders.CreateRandomAthena(
                    seed: ZeusParamsVm.SSeedVm.CurVal,
                    ngSize: Zeus.GroupCount,
                    pSig: ZeusParamsVm.PSigVm.CurVal,
                    sSig: ZeusParamsVm.SSigVm.CurVal
                ));
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
            return (!_isRunning); /// && (Wng != null);
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
            return (!_isRunning); // && (Wng != null);
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
            return (!_isRunning); // && (Wng != null);
        }

        #endregion // ResetSCommand

        #region  StopCommand

        RelayCommand _stopCommand;
        public ICommand StopCommand
        {
            get
            {
                return _stopCommand ?? (
                    _stopCommand = new RelayCommand(
                        param => Dostop(),
                        param => Canstop()
                    ));
            }
        }

        private void Dostop()
        {
            _cancellationTokenSource.Cancel();
        }

        bool Canstop()
        {
            return (_isRunning); // && (Wng != null);
        }

        #endregion // StopCommand

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
            ZeusSnapVm = new ZeusSnapVm(
                athenaTr: AthenaTr,
                caption: String.Empty
            );
            CommandManager.InvalidateRequerySuggested();
            Generation = AthenaTr.Athena.Iteration.ToString();
        }


        private readonly Stopwatch _stopwatch = new Stopwatch();
        private string _generation;
        public string ElapsedTime =>
            $"{_stopwatch.Elapsed.Hours.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Minutes.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Seconds.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Milliseconds.ToString("000")}";

    }
}