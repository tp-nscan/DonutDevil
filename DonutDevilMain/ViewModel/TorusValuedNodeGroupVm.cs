using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DonutDevilControls.ViewModel.Common;
using MathLib.Intervals;
using NodeLib;
using WpfUtils;
using WpfUtils.BitmapGraphics;

namespace DonutDevilMain.ViewModel
{
    public class TorusValuedNodeGroupVm : NotifyPropertyChanged
    {
        private const int SquareSize = 256;

        public TorusValuedNodeGroupVm()
        {
            _imageLegendVm = new ImageLegendVm();
            _imageLegendVm.OnPixelsChanged.Subscribe(OnPixelsChanged);
            _stepSizeSliderVm = new SliderVm(RealInterval.Make(0, 1.6), 0.05, "0.00") { Title = "Step" };
            //_nodeGroup = NodeGroup.RandomMnNodeGroup(SquareSize * SquareSize, 127734, DataModulus);
            InitializeRun();
        }
        #region local vars

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private INodeGroup _nodeGroup;
        private INodeGroupUpdater _nodeGroupUpdaterNbd;

        #endregion

        #region UpdateGridCommand

        RelayCommand _updateGridCommand;
        public ICommand UpdateGridCommand
        {
            get
            {
                if (_updateGridCommand == null)
                    _updateGridCommand = new RelayCommand(
                        param => DoUpdateGrid(),
                        param => CanUpdateGrid()
                        );

                return _updateGridCommand;
            }
        }

        private async void DoUpdateGrid()
        {
            await Task.Run(() =>
            {
                try
                {
                    _stopwatch.Start();

                    InitializeRun();

                    for (var i = 0; i < 1000000; i++)
                    {
                        if (i % 10 == 0)
                        {
                            var i1 = i;
                            Application.Current.Dispatcher.Invoke
                                (
                                    () =>
                                    {

                                        ResetNodeGroupUpdaters();
                                        UpdateBindingProperties(i1);
                                        DrawMainGrid(_nodeGroup);

                                    },
                                    DispatcherPriority.Background
                                );

                        }

                        //_nodeGroup = _nodeGroupUpdaterNbd
                        //                .Update(_nodeGroup, StepSizeSliderVm.Value)
                        //                .ToNodeGroup(SquareSize * SquareSize);

                    }

                }
                catch (Exception)
                {
                }
            });
        }

        bool CanUpdateGrid()
        {
            return true;
        }

        #endregion // UpdateGridCommand

        #region local helpers

        void UpdateBindingProperties(int generation)
        {

            Generation = generation;
            ElapsedTime = _stopwatch.Elapsed;
        }

        void DrawMainGrid(INodeGroup nodeGroup)
        {
            //GridGraphicsInfo =
            //    nodeGroup.Nodes.Select(n =>
            //        new GraphicsInfo(
            //            x: n.GroupIndex / SquareSize,
            //            y: n.GroupIndex % SquareSize,
            //            color: _nodeGroupColorSequence.ToUnitColor(n.Value.M10ToDouble())))
            //        .ToList();
        }

        void InitializeRun()
        {
            _imageLegendVm.ImageFileName = @"C:\Users\tpnsc_000\Documents\GitHub\DonutDevil\DonutDevilControls\Images\earth.bmp";
            //_nodeGroup = NodeGroup.RandomMnNodeGroup(SquareSize * SquareSize, DateTime.Now.Millisecond, DataModulus);

            ResetNodeGroupUpdaters();

            DrawMainGrid(_nodeGroup);
        }

        void ResetNodeGroupUpdaters()
        {
            //_nodeGroupUpdaterNbd = NodeGroupUpdaterRing.ForSquareTorus
            //(
            //    gain: 0.095,
            //    squareSize: SquareSize,
            //    range: RangeSliderVm.Value,
            //    use8Way: Use8Way
            //);
        }


        #endregion

        #region Binding variables

        private bool _use8Way;
        public bool Use8Way
        {
            get { return _use8Way; }
            set
            {
                _use8Way = value;
                OnPropertyChanged("Use8Way");
            }
        }


        private int _generation;
        public int Generation
        {
            get { return _generation; }
            set
            {
                _generation = value;
                OnPropertyChanged("Generation");
            }
        }


        private TimeSpan _elapsedTime;
        public TimeSpan ElapsedTime
        {
            get { return _elapsedTime; }
            set
            {
                _elapsedTime = value;
                OnPropertyChanged("ElapsedTime");
            }
        }

        private IReadOnlyList<GraphicsInfo> _gridGraphicsInfo = new List<GraphicsInfo>();
        public IReadOnlyList<GraphicsInfo> GridGraphicsInfo
        {
            get { return _gridGraphicsInfo; }
            set
            {
                _gridGraphicsInfo = value;
                OnPropertyChanged("GridGraphicsInfo");
            }
        }

        private readonly ImageLegendVm _imageLegendVm;
        public ImageLegendVm ImageLegendVm
        {
            get { return _imageLegendVm; }
        }

        void OnPixelsChanged(int[,] pixels)
        {
            DrawMainGrid(_nodeGroup);
        }

        private readonly SliderVm _stepSizeSliderVm;
        public SliderVm StepSizeSliderVm
        {
            get { return _stepSizeSliderVm; }
        }

        #endregion

    }
}
