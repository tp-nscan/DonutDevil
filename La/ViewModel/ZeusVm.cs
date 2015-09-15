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
using La.ViewModel.Design;
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

        public ZeusVm(Zeus zeusTr)
        {
            ZeusTr = ZeusBuilders.ZeusToTr(zeusTr);
            ZeusParamsVm = new ZeusParamsVm();
            AthenaTr = ZeusBuilders.AthenaToTr(
                ZeusBuilders.CreateRandomAthena(
                    seed: ZeusParamsVm.PSeedVm.CurVal,
                    ngSize: ZeusTr.Zeus.GroupCount,
                    pSig: ZeusParamsVm.PSigVm.CurVal,
                    sSig: ZeusParamsVm.SSigVm.CurVal
                ));

            PStrideVm = new ParamIntVm(
                minVal: 10,
                maxVal: 100,
                curVal: 50,
                name: "Stride"
            );

            PStrideVm.PropertyChanged += PStrideVm_PropertyChanged;
            ZeusSnapVm = new ZeusSnapVm(
                    athenaTr: AthenaTr,
                    caption: "Start",
                    stride: PStrideVm.CurVal
                );

            IndexSelectorVm = new IndexSelectorVm(Enumerable.Range(0, zeusTr.EnsembleCount));
            DisplayFrequencySliderVm = new SliderVm(RealInterval.Make(1, 49), 2, "0")
                { Title = "Display Frequency", Value = 10 };

            _zeusTrPartSelector = new ZeusTrPartSelectorVm();

            _zeusTrPartSelector.OnTextSelected.Subscribe(PasteTrToClipboard);
        }

        void PasteTrToClipboard(string trName)
        {
            ZeusTrParts outVal;
            if (ZeusTrParts.TryParse(trName, out outVal))
            {
                Clipboard.Clear();
                switch (outVal)
                {
                    case ZeusTrParts.A:
                        Clipboard.SetText(
                        MathNetUtils.MatrixDebug(
                            AthenaTr.Athena.mA));
                        break;
                    case ZeusTrParts.B:
                        Clipboard.SetText(
                        MathNetUtils.MatrixDebug(
                            AthenaTr.Athena.mB));
                        break;
                    case ZeusTrParts.S:
                        Clipboard.SetText(
                        MathNetUtils.MatrixDebug(
                            AthenaTr.Athena.mS));
                        break;
                    case ZeusTrParts.R:
                        Clipboard.SetText(
                        MathNetUtils.VectorDebug(
                            ZeusTr.curMem));
                        break;
                    case ZeusTrParts.AA:
                        Clipboard.SetText(
                        MathNetUtils.MatrixDebug(
                            ZeusTr.Zeus.mAa));
                        break;
                    case ZeusTrParts.AB:
                        Clipboard.SetText(
                        MathNetUtils.MatrixDebug(
                            ZeusTr.Zeus.mAb));
                        break;
                    case ZeusTrParts.BA:
                        Clipboard.SetText(
                        MathNetUtils.MatrixDebug(
                            ZeusTr.Zeus.mBa));
                        break;
                    case ZeusTrParts.BB:
                        Clipboard.SetText(
                        MathNetUtils.MatrixDebug(
                            ZeusTr.Zeus.mBb));
                        break;
                    case ZeusTrParts.dAA:
                        Clipboard.SetText(
                        MathNetUtils.MatrixDebug(
                            AthenaTr.dAdA));
                        break;
                    case ZeusTrParts.dAB:
                        Clipboard.SetText(
                        MathNetUtils.MatrixDebug(
                            AthenaTr.dAdB));
                        break;
                    case ZeusTrParts.dBA:
                        Clipboard.SetText(
                        MathNetUtils.MatrixDebug(
                            AthenaTr.dBdA));
                        break;
                    case ZeusTrParts.dBB:
                        Clipboard.SetText(
                        MathNetUtils.MatrixDebug(
                            AthenaTr.dBdB));
                        break;
                    case ZeusTrParts.sAA:
                        Clipboard.SetText(
                        MathNetUtils.MatrixDebug(
                            ZeusUtils.ScorAA(ZeusTr.scM)));
                        break;
                    case ZeusTrParts.sAB:
                        Clipboard.SetText(
                        MathNetUtils.MatrixDebug(
                            ZeusUtils.ScorAB(ZeusTr.scM)));
                        break;
                    case ZeusTrParts.sBA:
                        Clipboard.SetText(
                        MathNetUtils.MatrixDebug(
                             ZeusUtils.ScorBA(ZeusTr.scM)));
                        break;
                    case ZeusTrParts.sBB:
                        Clipboard.SetText(
                        MathNetUtils.MatrixDebug(
                            ZeusUtils.ScorBB(ZeusTr.scM)));
                        break;


                    default:
                        break;
                }

            }
        }

        public ZeusTrPartSelectorVm ZeusTrPartSelector
        {
            get { return _zeusTrPartSelector; }
        }

        private void PStrideVm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ZeusSnapVm = new ZeusSnapVm(
                    athenaTr: AthenaTr,
                    caption: "Start",
                    stride: PStrideVm.CurVal
                );
        }

        public ParamIntVm PStrideVm { get; }



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

        public ZeusTr ZeusTr { get; private set; }

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
                            zeus: ZeusTr.Zeus,
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
            //Wng = ZeusBuilder.ResetC(ZeusTr: Waffle, wng: Wng);

            var nuZ = ZeusF.NextZeusTr(
                zeus: ZeusTr.Zeus,
                memIndex: IndexSelectorVm.IndexVm.Index,
                learnRate: (float)ZeusParamsVm.LearnRateVm.CurVal,
                athena:AthenaTr.Athena
                );
            ZeusTr = nuZ;
            UpdateUi();
        }

        bool CanLearn()
        {
            return (! _isRunning) && (ZeusTr != null);
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
                    ngSize: ZeusTr.Zeus.GroupCount,
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
            AthenaTr = ZeusBuilders.AthenaToTr(
                ZeusBuilders.ResetAthenaP(
                    athena: AthenaTr.Athena,
                    pSig: ZeusParamsVm.PSigVm.CurVal,
                    seed: ZeusParamsVm.PSeedVm.CurVal
                    ));

            UpdateUi();
        }

        bool CanResetP()
        {
            return (AthenaTr != null);
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
            //     ZeusTr: Waffle,
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
            //     sseed: ZeusParamsVm.Stride.CurVal,
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
                caption: String.Empty,
                stride: PStrideVm.CurVal
            );
            CommandManager.InvalidateRequerySuggested();
            Generation = AthenaTr.Athena.Iteration.ToString();
        }


        private readonly Stopwatch _stopwatch = new Stopwatch();
        private string _generation;
        private readonly ZeusTrPartSelectorVm _zeusTrPartSelector;

        public string ElapsedTime =>
            $"{_stopwatch.Elapsed.Hours.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Minutes.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Seconds.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Milliseconds.ToString("000")}";

    }
}