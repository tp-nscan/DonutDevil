using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NodeLib.Params;

namespace DonutDevilControls.ViewModel.Params
{
    public class ParamSetEditorVm
    {
        private ObservableCollection<IParamEditorVm> _paramVms = new ObservableCollection<IParamEditorVm>();
        public ObservableCollection<IParamEditorVm> ParamVms
        {
            get { return _paramVms; }
            set { _paramVms = value; }
        }

        public bool IsDirty
        {
            get { return ParamVms.Any(vm => vm.IsDirty); }
            set
            {
                foreach (var paramEditorVm in ParamVms)
                {
                    paramEditorVm.IsDirty = value;
                }
            }
        }

        public IEnumerable<IParameter> EditedParameters
        {
            get { return _paramVms.Select(vm => vm.EditedValue); }
        }
    }
}
