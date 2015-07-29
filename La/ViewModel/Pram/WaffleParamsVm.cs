using WpfUtils;

namespace La.ViewModel.Pram
{
    public class WaffleParamsVm : NotifyPropertyChanged
    {
        public WaffleParamsVm()
        {
            GlauberRadiusVm = new ParamIntVm(
                    minVal: 1,
                    maxVal: 7,
                    curVal: 5,
                    name: "S to S radius"
                );
            PSigVm = new ParamDoubleVm(
                    minVal: 0,
                    maxVal: 0.3,
                    curVal: 0.1,
                    increment: 0.005,
                    formatString: "0.000",
                    name: "P initial stDev"
                );
            SSigVm = new ParamDoubleVm(
                    minVal: 0,
                    maxVal: 0.3,
                    curVal: 0.1,
                    increment: 0.005,
                    formatString: "0.000",
                    name: "S initial stDev"
                );
            CPpVm = new ParamDoubleVm(
                    minVal: 0,
                    maxVal: 0.3,
                    curVal: 0.1,
                    increment: 0.005,
                    formatString: "0.000",
                    name: "P to P step"
                );
            CSsVm = new ParamDoubleVm(
                    minVal: 0,
                    maxVal: 0.3,
                    curVal: 0.1,
                    increment: 0.005,
                    formatString: "0.000",
                    name: "S to S step"
                );
            CRpVm = new ParamDoubleVm(
                    minVal: 0,
                    maxVal: 0.3,
                    curVal: 0.1,
                    increment: 0.005,
                    formatString: "0.000",
                    name: "R to P step"
                );
            CPsVm = new ParamDoubleVm(
                    minVal: 0,
                    maxVal: 0.3,
                    curVal: 0.1,
                    increment: 0.005,
                    formatString: "0.000",
                    name: "P to S step"
                );
            PNoiseLevelVm = new ParamDoubleVm(
                    minVal: 0,
                    maxVal: 0.3,
                    curVal: 0.1,
                    increment: 0.005,
                    formatString: "0.000",
                    name: "P noise level"
                );
            SNoiseLevelVm = new ParamDoubleVm(
                    minVal: 0,
                    maxVal: 0.3,
                    curVal: 0.1,
                    increment: 0.005,
                    formatString: "0.000",
                    name: "S noise level"
                );
            SeedVm = new ParamIntVm(
                    minVal: 1,
                    maxVal: 1000,
                    curVal: 0,
                    name: "Seed"
                );
            LearnRateVm = new ParamDoubleVm(
                    minVal: 0,
                    maxVal: 0.3,
                    curVal: 0.01,
                    increment: 0.005,
                    formatString: "0.000",
                    name: "Learn rate"
                );
            LearnFreqVm = new ParamIntVm(
                minVal: 1,
                maxVal: 1000,
                curVal: 20,
                name: "Learn frequency"
            );
        }

        public ParamIntVm GlauberRadiusVm { get; }
        public ParamDoubleVm PSigVm { get; }
        public ParamDoubleVm SSigVm { get; }
        public ParamDoubleVm CPpVm { get; }
        public ParamDoubleVm CSsVm { get; }
        public ParamDoubleVm CRpVm { get; }
        public ParamDoubleVm CPsVm { get; }
        public ParamDoubleVm PNoiseLevelVm { get; }
        public ParamDoubleVm SNoiseLevelVm { get; }
        public ParamIntVm SeedVm { get; }
        public ParamDoubleVm LearnRateVm { get; }
        public ParamIntVm LearnFreqVm { get; }

        public bool IsDirty => 
            CPpVm.IsDirty || CSsVm.IsDirty || 
            CRpVm.IsDirty || CPsVm.IsDirty;

        public void Clean()
        {
            CPpVm.Clean();
            CSsVm.Clean();
            CRpVm.Clean();
            CPsVm.Clean();
        }
    }

}
