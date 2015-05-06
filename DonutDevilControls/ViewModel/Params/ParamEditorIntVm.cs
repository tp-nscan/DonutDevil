using System;
using MathLib.Intervals;
using MathLib.NumericTypes;
using NodeLib.Params;
using WpfUtils;

namespace DonutDevilControls.ViewModel.Params
{
    public class ParamEditorIntVm: NotifyPropertyChanged, IParamEditorVm
    {
        public ParamEditorIntVm(ParamInt paramInt)
        {
            _numberFormat = "0";
            _paramInt = paramInt;
            _value = (int) _paramInt.Value;
            _interval = RealInterval.Make(_paramInt.Min, _paramInt.Max - _paramInt.Min);
            _tickFrequency = 1.0;
        }

        private readonly ParamInt _paramInt;

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
            get { return _paramInt.Name; }
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

        private int _value;
        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _isDirty = Math.Abs((int)_paramInt.Value - _value) > Mf.Epsilon;
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
            get { return ParamType.Int; }
        }

        public IParameter EditedValue
        {
            get { return new ParamInt(_paramInt.Min, _paramInt.Max, Value, _paramInt.Name, _paramInt.CanChangeAtRunTime); }
        }
    }
}
