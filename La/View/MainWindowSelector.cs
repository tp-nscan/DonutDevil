using System;
using System.Windows;
using System.Windows.Controls;
using La.ViewModel;

namespace La.View
{
    public class MainWindowSelector : DataTemplateSelector
    {
        public DataTemplate MenuTemplate { get; set; }

        public DataTemplate SandboxTemplate { get; set; }

        public DataTemplate NetworkTemplate { get; set; }

        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var legendVm = item as IMainWindowVm;

            if (legendVm != null)
            {
                switch (legendVm.MainWindowType)
                {
                    case MainWindowType.Menu:
                        return MenuTemplate;
                    case MainWindowType.Network:
                        return NetworkTemplate;
                    case MainWindowType.Sandbox:
                        return SandboxTemplate;
                    default:
                        throw new Exception(String.Format("MainWindowType {0} not handled in SelectTemplate", legendVm.MainWindowType));
                }
            }

            return DefaultTemplate;
        }
    }
}
