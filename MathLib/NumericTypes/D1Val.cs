namespace MathLib.NumericTypes
{
    public class D1Val<T>
    {
        public D1Val(int index, T value)
        {
            _index = index;
            _value = value;
        }

        private readonly T _value;
        public T Value
        {
            get { return _value; }
        }

        private readonly int _index;
        public int Index
        {
            get { return _index; }
        }

    }
}
