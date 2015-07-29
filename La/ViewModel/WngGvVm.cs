using System.Collections.Generic;
using LibNode;
using WpfUtils;

namespace La.ViewModel
{
    public class WngGvVm : NotifyPropertyChanged
    {
        public WngGvVm(Wng wng)
        {
            WngGvRecVms = new List<WngGvRecVm>();
            for (var i = 0; i < wng.GroupCount; i++)
            {
                WngGvRecVms.Add(new WngGvRecVm(
                        step: wng.Iteration,
                        index: i,
                        s: wng.mS[0,i],
                        a: wng.mA[0, i],
                        b: wng.mB[0, i],
                        v: wng.mV[0, i],
                        r: wng.mR[0, i],
                        aa1: wng.mAa[1, i],
                        ba1: wng.mBa[1, i],
                        ab1: wng.mAb[1, i],
                        bb1: wng.mBb[1, i]
                    ));
            }
        }

        public IList<WngGvRecVm> WngGvRecVms { get; }
    }

    public class WngGvRecVm : NotifyPropertyChanged
    {
        public WngGvRecVm(int step, int index, float s, 
            float a, float b, float v, float r, 
            float aa1, float ba1, float ab1, float bb1)
        {
            Step = step;
            Index = index;
            S = s;
            A = a;
            B = b;
            V = v;
            R = r;
            AA1 = aa1;
            BA1 = ba1;
            AB1 = ab1;
            BB1 = bb1;
        }

        public int Step { get; }
        public int Index { get; }
        public float S { get; }
        public float A { get; }
        public float B { get; }
        public float V { get; }
        public float R { get; }

        public float AA1 { get; }
        public float BA1 { get; }

        public float AB1 { get; }
        public float BB1 { get; }
    }
}
