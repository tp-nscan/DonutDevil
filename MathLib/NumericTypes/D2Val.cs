namespace MathLib.NumericTypes
{
    public class D2Val<T>
    {

        public D2Val(int x, int y, T value)
        {
            _x = x;
            _y = y;
            _value = value;
        }

        private readonly T _value;
        public T Value
        {
            get { return _value; }
        }

        private readonly int _x;
        public int X
        {
            get { return _x; }
        }

        private readonly int _y;
        public int Y
        {
            get { return _y; }
        }
    }
}
