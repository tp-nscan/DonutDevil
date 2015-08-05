using System.Windows.Media;
using LibNode;
using MathLib;
using WpfUtils.Utils;

namespace La.ViewModel.Design
{
    public class DesignZeusSnapVm : ZeusSnapVm
    {
        public DesignZeusSnapVm() 
            : base(DesignAthenaTr, "hi there")
        {
        }

        const int Seed = 123;
        const int DGpSz = 64;
        const int DMemSz = 16;
        const int GlauberRadius = 5;

        public static AthenaTr DesignAthenaTr => ZeusBuilder.DesignAthenaTr(seed: Seed,
            ngSize: DGpSz, memSize: DMemSz,
            ppSig: 0.3f,
            glauberRadius: GlauberRadius).Value;
    }
}
