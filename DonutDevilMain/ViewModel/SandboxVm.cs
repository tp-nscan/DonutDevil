using System;
using System.Reactive.Subjects;
using System.Windows.Input;
using DonutDevilControls.ViewModel.Common;
using DonutDevilControls.ViewModel.Design.Legend;
using DonutDevilControls.ViewModel.Legend;
using MathLib.Intervals;
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
            _mainGridVm = new WbUniformGridVm(GridStride, GridStride);
            LinearHistogramVm = new DesignLinearHistogramVm();
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

        private double _f;
        public double F
        {
            get { return _f; }
            set
            {
                _f = value;
                OnPropertyChanged("F");
            }
        }


        private double _x;
        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                OnPropertyChanged("X");
            }
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


        private readonly WbUniformGridVm _mainGridVm;
        public WbUniformGridVm MainGridVm
        {
            get { return _mainGridVm; }
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

        private LinearHistogramVm _linearHistogramVm;
        public LinearHistogramVm LinearHistogramVm
        {
            get { return _linearHistogramVm; }
            set
            {
                _linearHistogramVm = value;
                OnPropertyChanged("LinearHistogramVm");
            }
        }
    }
}
