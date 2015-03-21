using System;
using System.Collections.Generic;

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

    public static class D2ValExt
    {
        public static IEnumerable<D2Val<T>> ToSquareD2Array<T>(this int arraySize,
            Func<int, int, T> cellMakerFunc)
        {
            for (var i = 0; i < arraySize; i++)
            {
                for (var j = 0; j < arraySize; j++)
                {
                    yield return new D2Val<T>(i,j, cellMakerFunc(i,j));
                }
            }

        }

    }
}
