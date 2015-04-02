using System;
using System.Windows;
using System.Windows.Controls;
using DonutDevilControls.ViewModel.Legend;

namespace DonutDevilControls.View.Legend
{
    public class LegendLoadSelector : DataTemplateSelector
    {
        public DataTemplate RingTemplate { get; set; }

        public DataTemplate TorusTemplate { get; set; }

        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var legendVm = item as ILegendVm;

            if (legendVm != null)
            {
                switch (legendVm.LegendType)
                {
                    case LegendType.Torus:
                        return TorusTemplate;
                    case LegendType.Ring:
                        return RingTemplate;
                    default:
                        throw new Exception(String.Format("LegendVmType {0} not handled in SelectTemplate", legendVm.LegendType));
                }
            }

            return DefaultTemplate;
        }

    }
}
