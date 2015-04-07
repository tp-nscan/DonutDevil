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
using NodeLib.Indexers;
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
            _layerCorrelationVm = new LayerCorrelationVm(
                    name: "Memory Correlations",
                    master: Network.NodeGroupIndexers[0],
                    slaves: Network.NodeGroupIndexers.Skip(1)
                );

            _mainGridVm = new WbUniformGridVm(1024, 1024);

            _paramSetEditorVm = new ParamSetEditorVm();
            _paramSetEditorVm.ParamVms.AddMany
                (
                    network.Parameters.Values
                                      .Where(v=>v.CanChangeAtRunTime)
                                      .Select(v => v.ToParamEditorVm())
                );

            _displayFrequencySliderVm = new SliderVm(RealInterval.Make(1, 49), 2, "0") { Title = "Display Frequency", Value = 10 };

            _legendVm = new LinearLegendVm();
            _legendVm.OnLegendVmChanged.Subscribe(LegendChangedHandler);

            _ngIndexerSetVm = new NgIndexerSetVm(network.NodeGroupIndexers.ToNgIndexerVms());
            _ngIndexerSetVm.OnNgDisplayStateChanged.Subscribe(UpdateUi);
            _ngIndexerSetVm.Is1D = true;

            UpdateUi(NgIndexerSetVm.NgDisplayIndexing);
        }

        private readonly ParamSetEditorVm _paramSetEditorVm;
        public ParamSetEditorVm ParamSetEditorVm
        {
            get { return _paramSetEditorVm; }
        }

        INetwork Network { get; set; }

        private readonly LayerCorrelationVm _layerCorrelationVm;
        public LayerCorrelationVm LayerCorrelationVm
        {
            get { return _layerCorrelationVm; }
        }

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
            if (NgIndexerSetVm.Is1D)
            {
                var cellColors = NgIndexerSetVm.Indexer1D()
                    .IndexingFunc(Network.NodeGroup)
                    .Select(
                        d2F =>
                            new D2Val<Color>
                                (
                                x: d2F.X,
                                y: d2F.Y,
                                value: LegendVm.ColorFor1D(NgIndexerSetVm.Indexer1D().ValuesToUnitRange(d2F.Value))
                    )).ToList();

                MainGridVm.AddValues(cellColors);
            }

            else
            {
                var cellColors = NgIndexerSetVm.Indexer2Dx().IndexingFunc(Network.NodeGroup)
                    .Zip
                    (
                        NgIndexerSetVm.Indexer2Dy().IndexingFunc(Network.NodeGroup),
                            (x, y) => new D2Val<Color>
                            (
                                x: x.X,
                                y: x.Y,
                                value: LegendVm.ColorFor2D
                                            (
                                                xVal: NgIndexerSetVm.Indexer2Dx().ValuesToUnitRange(x.Value),
                                                yVal: NgIndexerSetVm.Indexer2Dy().ValuesToUnitRange(y.Value)
                                            )
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
            switch (ngDisplayIndexing.LegendMode)
            {
                case LegendMode.OneLayer:
                switch (NgIndexerSetVm.Indexer1D().NgIndexerType)
                    {
                        case NodeLib.Indexers.NgIndexerType.LinearArray2D:
                            LegendVm = _linearLegendVm;
                            HistogramVm = _linearHistogramVm;
                            break;
                        case NodeLib.Indexers.NgIndexerType.RingArray2D:
                            LegendVm = _ringLegendVm;
                            HistogramVm = _ringHistogramVm;

                            break;
                        default:
                            throw new Exception("Incorrect NgIndexerType");
                    }

                    HistogramVm.MinValue = NgIndexerSetVm.Indexer1D().UnitRangeToValues(0.0f);
                    HistogramVm.MaxValue = NgIndexerSetVm.Indexer1D().UnitRangeToValues(1.0f);
                    HistogramVm.Title = NgIndexerSetVm.Indexer1D().Name;
                    HistogramVm.MakeHistogram
                    (
                        NgIndexerSetVm.Indexer1D().IndexingFunc(Network.NodeGroup)
                                .Select(d2V => NgIndexerSetVm.Indexer1D().ValuesToUnitRange(d2V.Value))
                    );
                    HistogramVm.DrawLegend(f=>LegendVm.ColorFor1D(f));

                    break;
                case LegendMode.TwoLayers:
                    HistogramVm = _torusHistogramVm;
                    HistogramVm.TitleX = NgIndexerSetVm.Indexer2Dx().Name;
                    HistogramVm.TitleY = NgIndexerSetVm.Indexer2Dy().Name;

                    HistogramVm.MinValueX = NgIndexerSetVm.Indexer2Dx().UnitRangeToValues(0.0f);
                    HistogramVm.MaxValueX = NgIndexerSetVm.Indexer2Dx().UnitRangeToValues(1.0f);
                    HistogramVm.MinValueY = NgIndexerSetVm.Indexer2Dy().UnitRangeToValues(0.0f);
                    HistogramVm.MaxValueY = NgIndexerSetVm.Indexer2Dy().UnitRangeToValues(1.0f);


                    HistogramVm.MakeHistogram
                            (
                                 xVals: NgIndexerSetVm.Indexer2Dx().IndexingFunc(Network.NodeGroup)
                                          .Select(d2V => NgIndexerSetVm.Indexer2Dx().ValuesToUnitRange(d2V.Value)),
                                 yVals: NgIndexerSetVm.Indexer2Dy().IndexingFunc(Network.NodeGroup)
                                          .Select(d2V => NgIndexerSetVm.Indexer2Dy().ValuesToUnitRange(d2V.Value))
                            );


                    if (_torusLegendVm == null)
                    {
                        LegendVm = new TwoDLegendVm();
                        _torusLegendVm = LegendVm;
                        ((TwoDLegendVm) LegendVm).IsStandard = true;
                    }
                    else
                    {
                        LegendVm = _torusLegendVm;
                    }
                    break;
                default:
                    throw new Exception("LegendMode not handled in NgDisplayStateChangedHandler");
            }

            DrawMainNetwork();
            LayerCorrelationVm.UpdateCorrelations(NgIndexer.AbsCorrelationZFunc(Network.NodeGroup));
            OnPropertyChanged("Generation");
            OnPropertyChanged("ElapsedTime");
            CommandManager.InvalidateRequerySuggested();
        }

        readonly ILegendVm _linearLegendVm = new LinearLegendVm();
        readonly ILegendVm _ringLegendVm = new RingLegendVm();
        ILegendVm _torusLegendVm;

        readonly IHistogramVm _ringHistogramVm = new RingHistogramVm("", 0.0, 1.0);
        readonly IHistogramVm _linearHistogramVm = new LinearHistogramVm("", -1.0, 1.0);
        readonly IHistogramVm _torusHistogramVm = new TwoDhistogramVm("", 512);

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
                case LegendType.Interval:
                    _histogramVm.DrawLegend(legendVm.ColorFor1D);
                    break;
                case LegendType.Ring:
                    _histogramVm.DrawLegend(legendVm.ColorFor1D);
                    break;
                case LegendType.Torus:
                    _histogramVm.DrawLegend(legendVm.ColorFor2D);
                    break;
                default:
                    throw new Exception("Unhandled LegendType");
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
