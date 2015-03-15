using System;
using System.Windows;
using System.Windows.Controls;
using DonutDevilControls.ViewModel.Params;
using NodeLib.Params;

namespace DonutDevilControls.View.Params
{
    public class ParamEditorSelector : DataTemplateSelector
    {
        public DataTemplate BoolTemplate { get; set; }

        public DataTemplate FloatTemplate { get; set; }

        public DataTemplate EnumTemplate { get; set; }

        public DataTemplate IntTemplate { get; set; }

        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var paramVm = item as IParamEditorVm;

            if (paramVm != null)
            {
                switch (paramVm.ParamType)
                {
                    case ParamType.Bool:
                        return BoolTemplate;
                    case ParamType.Enum:
                        return EnumTemplate;
                    case ParamType.Float:
                        return FloatTemplate;
                    case ParamType.Int:
                        return IntTemplate;
                    default:
                        throw new Exception(String.Format("ParamsType {0} not handled in SelectTemplate", paramVm.ParamType));
                }
            }

            return DefaultTemplate;
        }

    }
}
