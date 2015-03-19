using System;
using System.Reactive.Subjects;
using MathLib.Intervals;
using WpfUtils;

namespace DonutDevilControls.ViewModel.Common
{
    public class SliderVm : NotifyPropertyChanged
    {
        public SliderVm(IRealInterval interval, 
                        double tickFrequency, 
                        string numberFormat)
        {
            _tickFrequency = tickFrequency;
            _numberFormat = numberFormat;
            _interval = interval;
            Value = Interval.Mid();
        }

        private readonly string _numberFormat;
        private string NumberFormat
        {
            get { return _numberFormat; }
        }

        private readonly IRealInterval _interval;
        public IRealInterval Interval
        {
            get { return _interval; }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public string LegendMaximum
        {
            get { return Interval.Max.ToString(NumberFormat); }
        }

        public string LegendMinimum
        {
            get { return Interval.Min.ToString(NumberFormat); }
        }

        public string LegendValue
        {
            get { return Value.ToString(NumberFormat); }
        }

        private double _value;
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _sliderVmChanged.OnNext(this);
                IsDirty = true;
                OnPropertyChanged("Value");
                OnPropertyChanged("LegendValue");
            }
        }

        private readonly double _tickFrequency;
        public double TickFrequency
        {
            get { return _tickFrequency; }
        }

        public bool IsDirty { get; set; }

        private readonly Subject<SliderVm> _sliderVmChanged
            = new Subject<SliderVm>();
        public IObservable<SliderVm> OnSliderVmChanged
        {
            get { return _sliderVmChanged; }
        }
    }
}
