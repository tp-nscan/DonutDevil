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
using DonutDevilControls.ViewModel.NgIndexer;
using DonutDevilControls.ViewModel.Params;
using MathLib.Intervals;
using MathLib.NumericTypes;
using NodeLib;
using WpfUtils;
using WpfUtils.ViewModels.Graphics;

namespace DonutDevilMain.ViewModel
{
    public class NetworkVm : NotifyPropertyChanged, IMainWindowVm
    {
        #region Navigation

        public MainWindowType MainWindowType
        {
            get { return MainWindowType.Network; }
        }

        private readonly Subject<IMainWindowVm> _mainWindowTypehanged
            = new Subject<IMainWindowVm>();
        public IObservable<IMainWindowVm> OnMainWindowTypeChanged
        {
            get { return _mainWindowTypehanged; }
        }


        #endregion

        public NetworkVm(INetwork network)
        {
            Network = network;
            var arrayStride = (int)network.Parameters["ArrayStride"].Value;
            _mainGridVm = new WbUniformGridVm(arrayStride, arrayStride);

            _paramSetEditorVm = new ParamSetEditorVm();
            _paramSetEditorVm.ParamVms.AddMany(network.Parameters.Values.Select(v => v.ToParamEditorVm()));

            _displayFrequencySliderVm = new SliderVm(RealInterval.Make(1, 49), 2, "0") { Title = "Display Frequency", Value = 10 };

            _legendVm = new RingLegendVm();
            _legendVm.OnLegendVmChanged.Subscribe(LegendChangedHandler);

            _ngIndexerSetVm = new NgIndexerSetVm(network.NodeGroupIndexers.ToNgIndexerVms());
            _ngIndexerSetVm.OnNgDisplayStateChanged.Subscribe(UpdateUi);
            _ngIndexerSetVm.IsRing = true;

            UpdateUi(NgIndexerSetVm.NgDisplayIndexing);
        }

        private readonly ParamSetEditorVm _paramSetEditorVm;
        public ParamSetEditorVm ParamSetEditorVm
        {
            get { return _paramSetEditorVm; }
        }

        INetwork Network { get; set; }


        #region local vars

        private readonly Stopwatch _stopwatch = new Stopwatch();

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
                    if (ParamSetEditorVm.IsDirty)
                    {
                        Network = Network.UpdateParams(ParamSetEditorVm.EditedParameters.ToDictionary(p => p.Name));
                        ParamSetEditorVm.IsDirty = false;
                    }

                    Network = Network.UpdateNodeGroup();

                    if (_cancellationTokenSource.IsCancellationRequested)
                    {
                        _isRunning = false;
                        _stopwatch.Stop();
                    }

                    if (i % (int)DisplayFrequencySliderVm.Value == 0)
                    {
                        Application.Current.Dispatcher.Invoke
                            (
                                () => UpdateUi(NgIndexerSetVm.NgDisplayIndexing),
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


        private readonly WbUniformGridVm _mainGridVm;
        public WbUniformGridVm MainGridVm
        {
            get { return _mainGridVm; }
        }


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


        #region ResetNetworkCommand

        RelayCommand _resetNetworkCommand;
        public ICommand ResetNetworkCommand
        {
            get
            {
                return _resetNetworkCommand ?? (_resetNetworkCommand = new RelayCommand(
                    param => DoReset(),
                    param => CanDoReset()
                    ));
            }
        }

        private void DoReset()
        {
            _mainWindowTypehanged.OnNext(new MenuVm());
        }

        bool CanDoReset()
        {
            return !_isRunning;
        }

        #endregion // ResetCommand


        void DrawMainNetwork()
        {
            if (NgIndexerSetVm.IsRing)
            {
                var cellColors = NgIndexerSetVm.RingIndexer()
                    .IndexingFunc(Network.NodeGroup)
                    .Select(
                        d2F =>
                            new D2Val<Color>
                                (
                                x: d2F.X,
                                y: d2F.Y,
                                value: LegendVm.ColorForUnitRing(d2F.Value))
                    )
                    .ToList();

                MainGridVm.AddValues(cellColors);
            }

            else
            {
                var cellColors = NgIndexerSetVm.TorusXIndexer().IndexingFunc(Network.NodeGroup)
                    .Zip
                    (
                        NgIndexerSetVm.TorusYIndexer().IndexingFunc(Network.NodeGroup),
                            (x, y) => new D2Val<Color>
                            (
                                x: x.X,
                                y: x.Y,
                                value: LegendVm.ColorForUnitTorus(x.Value, y.Value)
                            )
                   ).ToList();

                MainGridVm.AddValues(cellColors);
            }
        }

        public int Generation
        {
            get { return Network.NodeGroup.Generation; }
        }

        public TimeSpan ElapsedTime
        {
            get { return _stopwatch.Elapsed; }
        }

        private readonly SliderVm _displayFrequencySliderVm;
        public SliderVm DisplayFrequencySliderVm
        {
            get { return _displayFrequencySliderVm; }
        }

        #region NgIndexerSet

        private readonly NgIndexerSetVm _ngIndexerSetVm;
        public NgIndexerSetVm NgIndexerSetVm
        {
            get { return _ngIndexerSetVm; }
        }

        void UpdateUi(INgDisplayIndexing ngDisplayIndexing)
        {
            switch (ngDisplayIndexing.NgisDisplayMode)
            {
                case NgDisplayMode.Ring:
                    LegendVm = _ringLegendVm;
                    HistogramVm = _ringHistogramVm;
                    HistogramVm.MakeHistogram(Network.NodeGroup.Values);
                    HistogramVm.DrawLegend(f=>LegendVm.ColorForUnitRing(f));
                    break;
                case NgDisplayMode.Torus:
                    HistogramVm = _torusHistogramVm;
                    HistogramVm.MakeHistogram
                        (
                             xVals: NgIndexerSetVm.TorusXIndexer().IndexingFunc(Network.NodeGroup)
                                      .Select(d2v=>d2v.Value),
                             yVals: NgIndexerSetVm.TorusYIndexer().IndexingFunc(Network.NodeGroup)
                                      .Select(d2v => d2v.Value)
                        );
                    if (_torusLegendVm == null)
                    {
                        LegendVm = new TorusLegendVm();
                        _torusLegendVm = LegendVm;
                        ((TorusLegendVm) LegendVm).IsStandard = true;
                    }
                    else
                    {
                        LegendVm = _torusLegendVm;
                    }
                    break;
                default:
                    throw new Exception("NgisDisplayMode not handled in NgDisplayStateChangedHandler");
            }

            DrawMainNetwork();
            OnPropertyChanged("Generation");
            OnPropertyChanged("ElapsedTime");
            CommandManager.InvalidateRequerySuggested();
        }


        readonly ILegendVm _ringLegendVm = new RingLegendVm();
        ILegendVm _torusLegendVm;

        readonly IHistogramVm _ringHistogramVm = new RingHistogramVm("Wubba r");
        readonly IHistogramVm _torusHistogramVm = new TorusHistogramVm("Wubba t", 128);

        #endregion

        #region Legend

        private IDisposable _onLegendVmChangedSubscr;

        private ILegendVm _legendVm;
        public ILegendVm LegendVm
        {
            get { return _legendVm; }
            set
            {
                _legendVm = value;

                if (_onLegendVmChangedSubscr != null)
                {
                    _onLegendVmChangedSubscr.Dispose();
                }

                _onLegendVmChangedSubscr = _legendVm.OnLegendVmChanged.Subscribe(LegendChangedHandler);
                OnPropertyChanged("LegendVm");
            }
        }


        void LegendChangedHandler(ILegendVm legendVm)
        {
            switch (_histogramVm.DisplaySpaceType)
            {
                case DisplaySpaceType.Ring:
                    _histogramVm.DrawLegend(legendVm.ColorForUnitRing);
                    break;
                case DisplaySpaceType.Torus:
                    _histogramVm.DrawLegend(legendVm.ColorForUnitTorus);
                    break;
                default:
                    throw new Exception("Unhandled DisplaySpaceType");
            }
            DrawMainNetwork();
        }

        #endregion

        #region Histogram

        private IHistogramVm _histogramVm;
        public IHistogramVm HistogramVm
        {
            get { return _histogramVm; }
            set
            {
                _histogramVm = value;
                OnPropertyChanged("HistogramVm");
            }
        }

        #endregion

    }
}
