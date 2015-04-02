using System;
using System.Reactive.Subjects;
using System.Windows.Media;
using DonutDevilControls.ViewModel.Common;
using MathLib.Intervals;
using MathLib.NumericTypes;
using WpfUtils;
using WpfUtils.Utils;

namespace DonutDevilControls.ViewModel.Legend
{
    public class OneDlegendVm : NotifyPropertyChanged, ILegendVm
    {
        private const int Colorsteps = 512;

        public OneDlegendVm()
        {
            _offsetSliderVm = new SliderVm(RealInterval.Make(0, 1.0), 0.02, "0.00") { Title = "Offset" };
            _offsetSliderVm.OnSliderVmChanged.Subscribe(s => _legendVmChanged.OnNext(this));
            _ringColorSequence = ColorSequence.Quadrupolar(Colors.Red, Colors.Orange, Colors.Green, Colors.Blue, Colorsteps / 4);
            _unitZColorSequence = ColorSequence.Dipolar(Colors.Red, Colors.Blue, Colorsteps / 2);
            _histogramColorSequence = Colors.White.ToUniformColorSequence(Colorsteps);
            _colorSequenceRing = _ringColorSequence;
            _useScheme1 = true;
        }

        public LegendType LegendType
        {
            get { return LegendType.Ring; }
        }

        private IColorSequence _colorSequenceRing;
        public IColorSequence ColorSequenceRing
        {
            get { return _colorSequenceRing; }
        }

        public Color ColorForInterval(float val)
        {
            return ColorSequenceRing.ToUnitColor((val + OffsetSliderVm.Value).AsMf());
        }

        public Color ColorFor1D(float val)
        {
            return ColorSequenceRing.ToUnitColor((val + OffsetSliderVm.Value).AsMf());
        }

        private readonly IColorSequence _ringColorSequence;
        private readonly IColorSequence _unitZColorSequence;
        private readonly IColorSequence _histogramColorSequence;

        public Color ColorFor2D(float xVal, float yVal)
        {
            throw new NotImplementedException();
        }

        private readonly Subject<ILegendVm> _legendVmChanged
            = new Subject<ILegendVm>();
        public IObservable<ILegendVm> OnLegendVmChanged
        {
            get { return _legendVmChanged; }
        }

        private readonly SliderVm _offsetSliderVm;
        public SliderVm OffsetSliderVm
        {
            get { return _offsetSliderVm; }
        }

        private bool _useScheme1;
        public bool UseScheme1
        {
            get { return _useScheme1; }
            set
            {
                _useScheme1 = value;
                OnPropertyChanged("UseScheme1");
                if (value)
                {
                    _colorSequenceRing = _ringColorSequence;
                    _legendVmChanged.OnNext(this);
                }
            }
        }

        private bool _useScheme2;
        public bool UseScheme2
        {
            get { return _useScheme2; }
            set
            {
                _useScheme2 = value;
                OnPropertyChanged("UseScheme2");
                if (value)
                {
                    _colorSequenceRing = _histogramColorSequence;
                    _legendVmChanged.OnNext(this);
                }
            }
        }

        private bool _useScheme3;
        public bool UseScheme3
        {
            get { return _useScheme3; }
            set
            {
                _useScheme3 = value;
                OnPropertyChanged("UseScheme3");
                if (value)
                {
                    _colorSequenceRing = _unitZColorSequence;
                    _legendVmChanged.OnNext(this);
                }
            }
        }

    }
}
