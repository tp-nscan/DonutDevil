namespace MathLib.NumericTypes
{
    public class D2Val<T>
    {
        private readonly int _y;
        private readonly int _x;
        private readonly T _value;

        public D2Val(int x, int y, T value)
        {
            _x = x;
            _y = y;
            _value = value;
        }

        public T Value
        {
            get { return _value; }
        }

        public int X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }
    }
}
