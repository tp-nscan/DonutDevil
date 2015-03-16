using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DonutDevilControls.ViewModel.Common;
using DonutDevilControls.ViewModel.NgIndexer;
using DonutDevilControls.ViewModel.Params;
using MathLib.Intervals;
using NodeLib;
using NodeLib.Updaters;
using WpfUtils;
using WpfUtils.Utils;

namespace DonutDevilMain.ViewModel
{
    public class NetworkVm : NotifyPropertyChanged
    {
        public NetworkVm(INetwork network)
        {
            Network = network;
            _paramSetEditorVm = new ParamSetEditorVm();
            _paramSetEditorVm.ParamVms.AddMany(network.Parameters.Values.Select(v => v.ToParamEditorVm()));
            _displayFrequencySliderVm = new SliderVm(RealInterval.Make(1, 49), 2, "0") { Title = "Display Frequency", Value = 10 };
            _ngIndexerSetVm = new NgIndexerSetVm(network.NodeGroupIndexers.ToNgIndexerVms());
        }

        private readonly ParamSetEditorVm _paramSetEditorVm;
        public ParamSetEditorVm ParamSetEditorVm
        {
            get { return _paramSetEditorVm; }
        }

        INetwork Network { get; set; }

        #region local vars

        private readonly IColorSequence _nodeGroupColorSequence;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private readonly IColorSequence _histogramColorSequence;

        private INgUpdater _ngUpdater;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private bool _isRunning;

        public bool IsDirty
        {
            get
            {
                return false;
                //return _stepSizeSliderVm.IsDirty || _alphaSliderVm.IsDirty || _betaSliderVm.IsDirty;
            }

        }

        public void Clean()
        {
            //_stepSizeSliderVm.IsDirty = false;
            //_alphaSliderVm.IsDirty = false;
            //_betaSliderVm.IsDirty = false;
        }

        #endregion





        #region UpdateNetworkCommand

        RelayCommand _updateNetworkCommand;
        public ICommand UpdateNetworkCommand
        {
            get
            {
                return _updateNetworkCommand ?? (_updateNetworkCommand = new RelayCommand(
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
                    if (IsDirty)
                    {
                        Network.UpdateParams(ParamSetEditorVm.EditedParameters.ToDictionary(p => p.Name));
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
                                () =>
                                {
                                    //ResetNgUpdaters();
                                    //UpdateBindingProperties();
                                    //MakeHistogram();
                                    //DrawMainNetwork(_nodeGroup);
                                },
                                DispatcherPriority.Background
                            );
                    }
                }
            },
                cancellationToken: _cancellationTokenSource.Token
            );
        }

        void UpdateUi()
        {
            CommandManager.InvalidateRequerySuggested();
            OnPropertyChanged("Generation");
            OnPropertyChanged("ElapsedTime");
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


        #region ResetCommand

        RelayCommand _resetCommand;
        public ICommand ResetCommand
        {
            get
            {
                return _resetCommand ?? (_resetCommand = new RelayCommand(
                    param => DoReset(),
                    param => CanDoReset()
                    ));
            }
        }

        private void DoReset()
        {
            InitializeRun();
        }

        bool CanDoReset()
        {
            return !_isRunning;
        }

        #endregion // ResetCommand

        void InitializeRun()
        {
            //_nodeGroup = NodeGroup.RandomNodeGroup(SquareSize * SquareSize, DateTime.Now.Millisecond);

            //MakeHistogram();

            //ResetNgUpdaters();

            //DrawMainNetwork(_nodeGroup);
        }


        public int Generation
        {
            get
            {
                return 0; // _nodeGroup.Generation; 
            }
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

        private readonly NgIndexerSetVm _ngIndexerSetVm;
        public NgIndexerSetVm NgIndexerSetVm
        {
            get { return _ngIndexerSetVm; }
        }
    }
}
