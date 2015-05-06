using System.Collections.Generic;
using System.Linq;
using NodeLib.Params;

namespace DonutDevilControls.ViewModel.Params
{
    public class ParamSetEditorVm
    {
        public ParamSetEditorVm(IReadOnlyList<IParameter> parameters, bool showAll)
        {
            if (showAll)
            {
                _unEditedParameters = new List<IParameter>();
                _paramVms = parameters.Select(v => v.ToParamEditorVm()).ToList();
                return;
            }
            _unEditedParameters = parameters.Where(p => !p.CanChangeAtRunTime).ToList();
            _paramVms = parameters.Where(p => p.CanChangeAtRunTime)
                .Select(v => v.ToParamEditorVm()).ToList();
        }

        private readonly List<IParameter> _unEditedParameters;

        private readonly List<IParamEditorVm> _paramVms;
        public IList<IParamEditorVm> ParamVms
        {
            get { return _paramVms; }
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

        public IEnumerable<IParameter> LatestParameters
        {
            get { return _paramVms.Select(vm => vm.EditedValue).Concat(_unEditedParameters); }
        }

    }
}
