using System;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using System.Windows.Media;
using DonutDevilControls.ViewModel.Common;
using DonutDevilControls.ViewModel.Design.Legend;
using DonutDevilControls.ViewModel.Legend;
using MathLib.Intervals;
using MathLib.NumericTypes;
using WpfUtils;
using WpfUtils.ViewModels.Graphics;

namespace DonutDevilMain.ViewModel
{
    public class SandboxVm : NotifyPropertyChanged, IMainWindowVm
    {
        public SandboxVm()
        {
            _radiusSliderVm = new SliderVm(RealInterval.Make(1, 40), 1, "0") { Title = "Radius", Value = 10 };
            _frequencySliderVm = new SliderVm(RealInterval.Make(0.0, 3), 0.015, "0.000") { Title = "Frequency * Pi / Radius", Value = 0.5 };
            _decaySliderVm = new SliderVm(RealInterval.Make(0, 1), 0.005, "0.000") { Title = "Decay", Value = 0.5 };

            _radiusSliderVm.OnSliderVmChanged.Subscribe(v => DrawMainNetwork());
            _frequencySliderVm.OnSliderVmChanged.Subscribe(v => DrawMainNetwork());
            _decaySliderVm.OnSliderVmChanged.Subscribe(v => DrawMainNetwork());

            _mainGridVm = new WbUniformGridVm(1024, 1024);
            _histogramVm = new DesignLinearHistogramVm();
            _legendVm = new OneDlegendVm();
            _legendVm.OnLegendVmChanged.Subscribe(LegendChangedHandler);
        }

        #region Navigation

        public int GridStride
        {
            get { return (int) (_radiusSliderVm.Value * 2 + 1); }
        }

        public MainWindowType MainWindowType
        {
            get { return MainWindowType.Sandbox; }
        }

        private readonly Subject<IMainWindowVm> _mainWindowTypehanged
            = new Subject<IMainWindowVm>();
        public IObservable<IMainWindowVm> OnMainWindowTypeChanged
        {
            get { return _mainWindowTypehanged; }
        }

        #endregion // Navigation

        public int Radius
        {
            get { return (int) RadiusSliderVm.Value; }
        }

        public double Decay
        {
            get { return _decaySliderVm.Value; }
        }

        public double Frequency
        {
            get { return _frequencySliderVm.Value; }
        }

        private void DrawMainNetwork()
        {
            var n = NeighborhoodExt.CircularGlauber(Radius, Frequency *  Math.PI/ Radius, Decay);
            var valueList = n.ToReadingOrder.Select(v=> v) .ToList();

            var cellColors = valueList.Select(
                (v,i) => new D2Val<Color>
                            (
                                x: i % (GridStride),
                                y: i / (GridStride),
                                value: LegendVm.ColorFor1D((float)(v / 2.0 + 0.5))
                            )
                ).ToList();

            MainGridVm.AddValues(cellColors);

            Total = valueList.Sum(v => v);

            AbsTotal = valueList.Sum(v => Math.Abs(v));

            _histogramVm.MakeHistogram(valueList.Select(cc => (float)(cc / 2.0 + 0.5)));
        }


        private void DrawMainNetwork0()
        {
            var randy = new Random();
            var cellColors =
            Enumerable.Range(0, GridStride * GridStride).Select(
                i => new D2Val<Color>
                            (
                                x: i % (GridStride),
                                y: i / (GridStride),
                                value: LegendVm.ColorFor1D((float)randy.NextDouble())
                            )
                   ).ToList();

            MainGridVm.AddValues(cellColors);
        }

        private readonly SliderVm _radiusSliderVm;
        public SliderVm RadiusSliderVm
        {
            get { return _radiusSliderVm; }
        }

        private readonly SliderVm _frequencySliderVm;
        public SliderVm FrequencySliderVm
        {
            get { return _frequencySliderVm; }
        }

        private readonly SliderVm _decaySliderVm;
        public SliderVm DecaySliderVm
        {
            get { return _decaySliderVm; }
        }

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


        private WbUniformGridVm _mainGridVm;
        public WbUniformGridVm MainGridVm
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

        void LegendChangedHandler(ILegendVm legendVm)
        {
            switch (_histogramVm.DisplaySpaceType)
            {
                case LegendType.Ring:
                    _histogramVm.DrawLegend(legendVm.ColorFor1D);
                    break;
                case LegendType.Torus:
                    _histogramVm.DrawLegend(legendVm.ColorFor2D);
                    break;
                case LegendType.Interval:
                    _histogramVm.DrawLegend(legendVm.ColorForInterval);
                    break;
                default:
                    throw new Exception("Unhandled LegendType");
            }

            DrawMainNetwork();
        }

        private LinearHistogramVm _histogramVm;
        public LinearHistogramVm HistogramVm
        {
            get { return _histogramVm; }
            set
            {
                _histogramVm = value;
                OnPropertyChanged("HistogramVm");
            }
        }

        private double _total;
        public double Total
        {
            get { return _total; }
            set
            {
                _total = value;
                OnPropertyChanged("Total");
            }
        }

        private double _absTotal;
        public double AbsTotal
        {
            get { return _absTotal; }
            set
            {
                _absTotal = value;
                OnPropertyChanged("AbsTotal");
            }
        }
    }
}
