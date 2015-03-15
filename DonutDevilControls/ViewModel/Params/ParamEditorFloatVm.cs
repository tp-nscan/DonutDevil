using System;
using MathLib.Intervals;
using MathLib.NumericTypes;
using NodeLib.Params;
using WpfUtils;

namespace DonutDevilControls.ViewModel.Params
{
    public class ParamEditorFloatVm : NotifyPropertyChanged, IParamEditorVm
    {
        public ParamEditorFloatVm(ParamFloat paramFloat)
        {
            _paramFloat = paramFloat;
            _numberFormat = _paramFloat.Max.NumberFormat();
            Value = (float) _paramFloat.Value;
            _interval = RealInterval.Make(paramFloat.Min, paramFloat.Max - paramFloat.Min);
            _tickFrequency = (paramFloat.Max - paramFloat.Min) / 50.0;
        }

        private readonly ParamFloat _paramFloat;

        private readonly string _numberFormat;
        public string NumberFormat
        {
            get { return _numberFormat; }
        }

        private readonly IRealInterval _interval;
        public IRealInterval Interval
        {
            get { return _interval; }
        }

        public string Title
        {
            get { return _paramFloat.Name; }

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
                _isDirty = Math.Abs((float)_paramFloat.Value - _value) > Mf.Epsilon;
                OnPropertyChanged("Value");
                OnPropertyChanged("LegendValue");
            }
        }

        private readonly double _tickFrequency;
        public double TickFrequency
        {
            get { return _tickFrequency; }
        }

        private bool _isDirty;
        public bool IsDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; }
        }

        public ParamType ParamType
        {
            get { return ParamType.Float; }
        }

        public IParameter EditedValue
        {
            get { return new ParamFloat(_paramFloat.Min, _paramFloat.Max, (float) Value, _paramFloat.Name); }
        }
    }
}
