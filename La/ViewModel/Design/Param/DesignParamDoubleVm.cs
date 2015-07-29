using La.ViewModel.Pram;

namespace La.ViewModel.Design.Param
{
    public class DesignParamDoubleVm : ParamDoubleVm
    {
        public DesignParamDoubleVm() 
            : base(1.0, 100.0, 42.42, "Design Double Param", 
                  increment: 0.01,
                  formatString: "0.00")
        {
        }
    }
}
