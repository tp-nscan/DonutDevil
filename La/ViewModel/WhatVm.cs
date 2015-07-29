using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DonutDevilControls.ViewModel.Common;
using DonutDevilControls.ViewModel.Legend;
using LibNode;
using MathLib.Intervals;
using WpfUtils;
using WpfUtils.ViewModels.Graphics;

namespace La.ViewModel
{
    public class WhatVm : NotifyPropertyChanged, IMainContentVm
    {
        public WhatVm(Wng wng)
        {
            Wng = wng;

            WaffleHistories = WngBuilder.InitHistories(arrayLength: 5, targetLength: 200);

            DisplayFrequencySliderVm = new SliderVm(RealInterval.Make(1, 49), 2, "0") { Title = "Display Frequency", Value = 10 };
            RadiusSliderVm = new SliderVm(RealInterval.Make(1, 40), 1, "0") { Title = "Radius", Value = 10 };
            FrequencySliderVm = new SliderVm(RealInterval.Make(0.0, 3), 0.015, "0.000") { Title = "Frequency * Pi / Radius", Value = 0.5 };
            DecaySliderVm = new SliderVm(RealInterval.Make(0, 1), 0.005, "0.000") { Title = "Decay", Value = 0.5 };

            RadiusSliderVm.OnSliderVmChanged.Subscribe(v => UpdateUi());
            FrequencySliderVm.OnSliderVmChanged.Subscribe(v => UpdateUi());
            DecaySliderVm.OnSliderVmChanged.Subscribe(v => UpdateUi());

            _mainGridVm = new WbRollingGridVm(
                imageWidth:1000,
                imageHeight: 1000,
                cellDimX: Wng.GroupCount,
                cellDimY: Wng.GroupCount
                );

            _legendVm = new LinearLegendVm();
            _legendVm.OnLegendVmChanged.Subscribe(lvm => UpdateUi());

        }

        public WaffleHistories WaffleHistories { get; private set; }

        public Wng Wng { get; private set; }


        public string Generation => Wng.Iteration.ToString();

        #region Navigation

        public IEntityRepo EntityRepo { get; }

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
            _isRunning = true;

            await Task.Run(() =>
            {
                _stopwatch.Start();

                for (var i = 0; _isRunning; i++)
                {
                    //if (ParamSetEditorVm.IsDirty)
                    //{
                    //     = .UpdateParams(ParamSetEditorVm.LatestParameters.ToDictionary(p => p.Name));
                    //    ParamSetEditorVm.IsDirty = false;
                    //}

                    Wng = Wng.Update();

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

        bool CanUpdate()
        {
            return !_isRunning;
        }

        #endregion // UpdateCommand

        #region LearnCommand

        RelayCommand _learnCommand;
        public ICommand LearnCommand
        {
            get
            {
                return _learnCommand ?? (_learnCommand = new RelayCommand(
                    param => DoCancelUpdate(),
                    param => CanCancelUpdate()
                    ));
            }
        }

        private void DoCancelUpdate()
        {
            _cancellationTokenSource.Cancel();
        }

        bool CanCancelUpdate()
        {
            return _isRunning;
        }

        #endregion // LearnCommand

        #region ResetCommand

        RelayCommand _resetCommand;
        public ICommand ResetCommand
        {
            get
            {
                return _resetCommand ?? (_resetCommand = new RelayCommand(
                    param => DoReset(),
                    param => CanReset()
                    ));
            }
        }

        private void DoReset()
        {
            _cancellationTokenSource.Cancel();
        }

        bool CanReset()
        {
            return _isRunning;
        }

        #endregion // ResetCommand

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
            //WngHistories = WngBuilder.UpdateHistories(WngHistories, Wng);

            //var d2Vs = ArrayHistory.GetD2Vals(WngHistories.ahA);

            //var cellColors = d2Vs.Select(
            //        (v, i) => new D2Val<Color>
            //                    (
            //                        x: v.X,
            //                        y: v.Y,
            //                        val: LegendVm.ColorFor1D((float)(v.Val / 2.0 + 0.5))
            //                    )
            //        ).ToList();

            //MainGridVm.AddValues(cellColors);
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

        private readonly Stopwatch _stopwatch = new Stopwatch();

        public string ElapsedTime =>
            $"{_stopwatch.Elapsed.Hours.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Minutes.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Seconds.ToString("00")}:" +
            $"{_stopwatch.Elapsed.Milliseconds.ToString("000")}";

    }
}
