using System;
using System.Collections.Generic;
using LibNode;

namespace MathLib.NumericTypes
{
    public static class D2ValExt
    { 
        public static IEnumerable<D2Val<T>> ToSquareDtArray<T>(this int arraySize,
            Func<int, int, T> cellMakerFunc)
        {
            for (var i = 0; i < arraySize; i++)
            {
                for (var j = 0; j < arraySize; j++)
                {
                    yield return new D2Val<T>(i, j, cellMakerFunc(i, j));
                }
            }
        }
    }
}
