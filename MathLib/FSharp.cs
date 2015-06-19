using System.Collections.Generic;
using Microsoft.FSharp.Collections;

namespace MathLib
{
    public static class FSharpInteropExtensions
    {
        public static FSharpList<TItemType> ToFSharplist<TItemType>(this IEnumerable<TItemType> myList)
        {
            return SeqModule.ToList(myList);
        }

        public static IEnumerable<TItemType> ToEnumerable<TItemType>(this FSharpList<TItemType> fList)
        {
            return ListModule.ToSeq(fList);
        }
    }
}