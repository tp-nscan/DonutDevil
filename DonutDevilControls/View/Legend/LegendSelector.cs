using System;
using System.Windows;
using System.Windows.Controls;
using DonutDevilControls.ViewModel.Legend;

namespace DonutDevilControls.View.Legend
{
    public class LegendSelector : DataTemplateSelector
    {
        public DataTemplate RingTemplate { get; set; }

        public DataTemplate TorusTemplate { get; set; }

        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var legendVm = item as ILegendVm;

            if (legendVm != null)
            {
                switch (legendVm.DisplaySpaceType)
                {
                    case DisplaySpaceType.Torus:
                        return TorusTemplate;
                    case DisplaySpaceType.Ring:
                        return RingTemplate;
                    default:
                        throw new Exception(String.Format("LegendVmType {0} not handled in SelectTemplate", legendVm.DisplaySpaceType));
                }
            }

            return DefaultTemplate;
        }

    }
}
