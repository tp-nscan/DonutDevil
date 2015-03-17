using System;
using System.Windows;
using System.Windows.Controls;
using DonutDevilControls.ViewModel.Legend;

namespace DonutDevilControls.View.Legend
{
    public class LegendSelector : DataTemplateSelector
    {
        public DataTemplate SeqTemplate { get; set; }

        public DataTemplate ImageTemplate { get; set; }

        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var legendVm = item as ILegendVm;

            if (legendVm != null)
            {
                switch (legendVm.LegendVmType)
                {
                    case LegendVmType.Image:
                        return ImageTemplate;
                    case LegendVmType.Seq:
                        return SeqTemplate;
                    default:
                        throw new Exception(String.Format("LegendVmType {0} not handled in SelectTemplate", legendVm.LegendVmType));
                }
            }

            return DefaultTemplate;
        }

    }
}
