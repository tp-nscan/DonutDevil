using System;
using System.Windows;
using System.Windows.Controls;
using DonutDevilControls.ViewModel.Legend;

namespace DonutDevilControls.View.Legend
{
    public class HistogramSelector : DataTemplateSelector
    {
        public DataTemplate RingTemplate { get; set; }

        public DataTemplate TorusTemplate { get; set; }

        public DataTemplate UnitTemplate { get; set; }

        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var legendVm = item as IHistogramVm;

            if (legendVm != null)
            {
                switch (legendVm.DisplaySpaceType)
                {
                    case LegendType.Torus:
                        return TorusTemplate;
                    case LegendType.Ring:
                        return RingTemplate;
                    case LegendType.Interval:
                        return UnitTemplate;
                    default:
                        throw new Exception(String.Format("LegendVmType {0} not handled in SelectTemplate", legendVm.DisplaySpaceType));
                }
            }

            return DefaultTemplate;
        }

    }
}
