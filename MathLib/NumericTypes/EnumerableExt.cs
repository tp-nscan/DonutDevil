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
    }
}
