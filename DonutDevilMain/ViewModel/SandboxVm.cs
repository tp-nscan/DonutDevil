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
            _radiusSliderVm = new SliderVm(RealInterval.Make(1, 24), 1, "0") { Title = "Radius", Value = 10 };
            _frequencySliderVm = new SliderVm(RealInterval.Make(1, 24), 1, "0") { Title = "Frequency", Value = 10 };
            _decaySliderVm = new SliderVm(RealInterval.Make(1, 24), 1, "0") { Title = "Decay", Value = 10 };

            _radiusSliderVm.OnSliderVmChanged.Subscribe(v => DrawMainNetwork());
            _frequencySliderVm.OnSliderVmChanged.Subscribe(v => DrawMainNetwork());
            _decaySliderVm.OnSliderVmChanged.Subscribe(v => DrawMainNetwork());

            _mainGridVm = new WbUniformGridVm(GridStride, GridStride);
            _histogramVm = new DesignLinearHistogramVm();
            _legendVm = new RingLegendVm();
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
           MainGridVm = new WbUniformGridVm(GridStride, GridStride);
            var randy = new Random();
            var cellColors =
            Enumerable.Range(0, GridStride * GridStride).Select(
                i => new D2Val<Color>
                            (
                                x: i % (GridStride),
                                y: i / (GridStride),
                                value: LegendVm.ColorForRing((float) randy.NextDouble())
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
                case DisplaySpaceType.Ring:
                    _histogramVm.DrawLegend(legendVm.ColorForRing);
                    break;
                case DisplaySpaceType.Torus:
                    _histogramVm.DrawLegend(legendVm.ColorForTorus);
                    break;
                case DisplaySpaceType.Interval:
                    _histogramVm.DrawLegend(legendVm.ColorForInterval);
                    break;
                default:
                    throw new Exception("Unhandled DisplaySpaceType");
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
    }
}
