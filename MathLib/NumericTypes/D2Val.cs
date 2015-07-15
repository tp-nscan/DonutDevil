using System;
using System.Collections.Generic;
using LibNode;

namespace MathLib.NumericTypes
{
    public class D2Val<T>
    {
        public D2Val(int x, int y, T value)
        {
            X = x;
            Y = y;
            Value = value;
        }

        public T Value { get; }

        public int X { get; }

        public int Y { get; }
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

        public static IEnumerable<DTVal<T>> ToSquareDTArray<T>(this int arraySize,
            Func<int, int, T> cellMakerFunc)
        {
            for (var i = 0; i < arraySize; i++)
            {
                for (var j = 0; j < arraySize; j++)
                {
                    yield return new DTVal<T>(i, j, cellMakerFunc(i, j));
                }
            }
        }
    }
}
