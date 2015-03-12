using System.Collections.ObjectModel;

namespace DonutDevilControls.ViewModel.Params
{
    public class ParamsVm
    {
        public ParamsVm()
        {
            
        }

        private ObservableCollection<IParamVm> _paramVms = new ObservableCollection<IParamVm>();
        public ObservableCollection<IParamVm> ParamVms
        {
            get { return _paramVms; }
            set { _paramVms = value; }
        }
    }
}
