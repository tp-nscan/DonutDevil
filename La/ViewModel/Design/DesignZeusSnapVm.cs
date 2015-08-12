using System.Windows.Media;
using LibNode;
using MathLib;
using WpfUtils.Utils;

namespace La.ViewModel.Design
{
    public class DesignZeusSnapVm : ZeusSnapVm
    {
        public DesignZeusSnapVm() 
            : base(DesignAthenaTr, "hi there", 100)
        {
        }

        const int Seed = 1238;
        const int DGpSz = 256;
        const int DMemSz = 16;
        const int GlauberRadius = 5;

        public static AthenaTr DesignAthenaTr => ZeusBuilders.DesignAthenaTr(seed: Seed,
            ngSize: DGpSz, memSize: DMemSz,
            ppSig: 0.3f,
            glauberRadius: GlauberRadius).Value;
    }
}
