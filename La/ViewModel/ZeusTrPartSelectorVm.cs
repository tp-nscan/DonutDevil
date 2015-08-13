using System;
using LibNode;
using WpfUtils.ViewModels;

namespace La.ViewModel
{
    public class ZeusTrPartSelectorVm : TextSelectorVm
    {
        public ZeusTrPartSelectorVm() : base
            (
                Enum.GetNames(typeof(ZeusTrParts))
            )
        {
        } 

    }

    public enum ZeusTrParts
    {
        A,
        B,
        S,
        R,
        AA,
        AB,
        BA,
        BB,
        dAA,
        dAB,
        dBA,
        dBB
    }
}