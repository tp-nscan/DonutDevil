using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DonutDevilControls.ViewModel.Common;
using MathLib.Intervals;
using NodeLib;
using WpfUtils;
using WpfUtils.BitmapGraphics;
using WpfUtils.Utils;

namespace DonutDevilMain.ViewModel
{
    public class RingValuedNodeGroupVm : NotifyPropertyChanged
    {
        private const int SquareSize = 256;
        private const int Colorsteps = 128;
        public const int HistogramBins = 100;


        public RingValuedNodeGroupVm()
        {
            _rangeSliderVm = new SliderVm(RealInterval.Make(0, 0.5), 0.02, "0.00") {Title = "Range"};
            _stepSizeSliderVm = new SliderVm(RealInterval.Make(0, 1.6), 0.05, "0.00") { Title = "Step" };
            _nodeGroup = NodeGroup.RandomNodeGroup(SquareSize * SquareSize, 127734);
            InitializeRun();
        }


        #region local vars

        private IColorSequence _nodeGroupColorSequence;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private IColorSequence _histogramColorSequence;

        private INodeGroup _nodeGroup;

        private INodeGroupUpdater _nodeGroupUpdaterNbd;

            #endregion

        #region UpdateGridCommand

        RelayCommand _updateGridCommand;
        public ICommand UpdateGridCommand
        {
            get
            {
                return _updateGridCommand ?? (_updateGridCommand = new RelayCommand(
                    param => DoUpdateGrid(),
                    param => CanUpdateGrid()
                    ));
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
                                        MakeHistogram();
                                        DrawMainGrid(_nodeGroup);

                                    },
                                    DispatcherPriority.Background
                                );

                        }

                        _nodeGroup = _nodeGroupUpdaterNbd.Update(_nodeGroup);
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

        void MakeHistogram()
        {
            var histogram =
                _nodeGroup.Values.ToHistogram
                (
                    bins: RealInterval.ClosedZRange.SplitToEvenIntervals(HistogramBins).ToList(),
                    valuatorFunc: n => n
                );

            var colorDexer = (Colorsteps - 1) / histogram.Max(t => Math.Sqrt(t.Item2.Item));

            HistogramVm = new ColorLegendVm
            {
                Title = "Node frequencies",
                MinValue = -0.5,
                MaxValue = 0.5,
                GraphicsInfos = histogram.Select(
                    (bin, index) => new GraphicsInfo
                        (
                                x: index,
                                y: 0,
                                color: _histogramColorSequence.Colors[(int)(colorDexer * Math.Sqrt(bin.Item2.Item))]
                        )).ToList()
            };

        }

        void UpdateBindingProperties(int generation)
        {

            Generation = generation;
            ElapsedTime = _stopwatch.Elapsed;
        }

        void DrawMainGrid(INodeGroup nodeGroup)
        {
            GridGraphicsInfo =
                nodeGroup.Nodes().Select(n =>
                    new GraphicsInfo(
                        x: n.GroupIndex / SquareSize,
                        y: n.GroupIndex % SquareSize,
                        color: _nodeGroupColorSequence.ToUnitColor(n.Value)))
                    .ToList();
        }

        void InitializeRun()
        {
            _nodeGroupColorSequence = ColorSequence.Quadrupolar(Colors.Red, Colors.Orange, Colors.Green, Colors.Blue,
                Colorsteps);

            //_nodeGroupColorSequence = ColorSequence.Dipolar(Colors.Red, Colors.Blue,
            //    Colorsteps);

            _histogramColorSequence = Colors.White.ToUniformColorSequence(Colorsteps);

            _nodeGroup = NodeGroup.RandomNodeGroup(SquareSize * SquareSize, DateTime.Now.Millisecond);

            MakeHistogram();

            var x = 0;

            ColorLegendVm = new ColorLegendVm
            {
                Title = "Node values",
                MinValue = -1.0,
                MaxValue = 1.0,
                GraphicsInfos = _nodeGroupColorSequence
                                    .Colors
                                    .Select(
                                    c => new GraphicsInfo
                                        (
                                            x: x++,
                                            y: 0,
                                            color: c
                                    )).ToList()
            };


            ResetNodeGroupUpdaters();

            DrawMainGrid(_nodeGroup);

        }

        void ResetNodeGroupUpdaters()
        {
            _nodeGroupUpdaterNbd = NodeGroupUpdaterRing.ForSquareTorus
            (
                gain: 0.095,
                squareSize: SquareSize,
                range: RangeSliderVm.Value,
                use8Way: Use8Way
            );
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

        private ColorLegendVm _colorLegendVm;
        public ColorLegendVm ColorLegendVm
        {
            get { return _colorLegendVm; }
            set
            {
                _colorLegendVm = value;
                OnPropertyChanged("ColorLegendVm");
            }
        }

        private ColorLegendVm _histogramVm;
        public ColorLegendVm HistogramVm
        {
            get { return _histogramVm; }
            set
            {
                _histogramVm = value;
                OnPropertyChanged("HistogramVm");
            }
        }

        private readonly SliderVm _rangeSliderVm;
        public SliderVm RangeSliderVm
        {
            get { return _rangeSliderVm; }
        }

        private readonly SliderVm _stepSizeSliderVm;
        public SliderVm StepSizeSliderVm
        {
            get { return _stepSizeSliderVm; }
        }

        private readonly SliderVm _horizontalBiasSliderVm;
        public SliderVm HorizontalBiasSliderVm
        {
            get { return _horizontalBiasSliderVm; }
        }

        private readonly SliderVm _verticalBiasSliderVm;
        public SliderVm VerticalBiasSliderVm
        {
            get { return _verticalBiasSliderVm; }
        }


        #endregion
    }
}
