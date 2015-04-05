using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace MathLib
{
    public static class ListArrayExt
    {

        public static IEnumerable<T> SymmetricArrayValues<T>(
            this IReadOnlyList<T> matrix, int index, int arraySize)
        {
            var offset = index.ToLowerTriangularArraySize();

            for (var col = 0; col < index; col++)
            {
                yield return matrix[offset + col];
            }

            offset += index;

            for (var col = index + 1; col < arraySize; col++)
            {
                yield return matrix[offset + index];
                offset += col;
            }
        }

        public static IEnumerable<float> SymmetricArrayValuesWithZeroDiagonal(
            this IReadOnlyList<float> matrix, int index, int arraySize, int matrixStart)
        {
            var offset = matrixStart + index.ToLowerTriangularArraySize();

            for (var col = 0; col < index; col++)
            {
                yield return matrix[offset + col];
            }
            yield return 0.0f;
            offset += index;

            for (var col = index + 1; col < arraySize; col++)
            {
                yield return matrix[offset + index];
                offset += col;
            }
        }

        public static int ToLowerTriangularArraySize(this int squareSize)
        {
            return ((squareSize - 1) * squareSize) / 2;
        }


        public static IReadOnlyList<T> ToLowerTriangular<T>(this Func<int, int, T> func, int stride)
        {
            var lstRet = new List<T>();

            for (var i = 0; i < stride; i++)
            {
                for (var j = 0; j < i; j++)
                {
                    lstRet.Add(func(i,j));
                }
            }

            return lstRet;
        }

        public static IReadOnlyList<int> AutoCorrelate(this IReadOnlyList<int> values)
        {
            var lstRet = new List<int>();

            for (var i = 0; i < values.Count; i++)
            {
                for (var j = 0; j < i; j++)
                {
                    lstRet.Add(values[i] *values[j]);
                }
            }

            return lstRet;
        }

        public static IReadOnlyList<int> SumLists(this IEnumerable<IReadOnlyList<int>> lists)
        {
            IList<int> listRet = new int[0];
            foreach (var list in lists)
            {
                if (listRet.Count == 0)
                {
                    listRet = new int[list.Count];
                }
                for (var i = 0; i < list.Count; i++)
                {
                    listRet[i] += list[i];
                }
                
            }

            return (IReadOnlyList<int>) listRet;
        }

        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> items, int count)
        {
            var lst = items.ToList();

            for (var i = 0; i < count; i++)
            {
                foreach (var item in lst)
                {
                    yield return item;
                }
            }
        }
    }
}
