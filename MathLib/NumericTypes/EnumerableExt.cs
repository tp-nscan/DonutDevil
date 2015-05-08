using System;
using System.Collections.Generic;
using System.Linq;

namespace MathLib.NumericTypes
{
    public static class EnumerableExt
    {
        public static bool AreEqual<T>(
            this IEnumerable<T> lhs, 
            IEnumerable<T> rhs,
            Func<T,T, bool> comp 
            )
        {
            var lstLeft = lhs.ToArray();
            var lstRight = rhs.ToArray();

            if (lstLeft.Length != lstRight.Length)
            {
                return false;
            }
            return
                Enumerable.Range(0, lstLeft.Length)
                    .All(i => comp(lstLeft[i], lstRight[i]));
        }

        public static int Max(this int[,] values)
        {
            var maxVal = 0;
            for (var i = 0; i < values.GetUpperBound(0); i++)
            {
                for (var j = 0; j < values.GetUpperBound(1); j++)
                {
                    if (values[i, j] > maxVal)
                    {
                        maxVal = values[i, j];
                    }
                }
            }
            return maxVal;
        }

        public static float Max(this float[,] values)
        {
            float maxVal = 0;
            for (var i = 0; i < values.GetUpperBound(0); i++)
            {
                for (var j = 0; j < values.GetUpperBound(1); j++)
                {
                    if (values[i, j] > maxVal)
                    {
                        maxVal = values[i, j];
                    }
                }
            }
            return maxVal;
        }

        public static IEnumerable<T> ToColumnMajorOrder<T>(this T[,] matrix)
        {
            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    yield return matrix[i, j];
                }
            }
        }
    }



}
