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

        public DataTemplate WaffleTemplate { get; set; }

        public DataTemplate WhatTemplate { get; set; }

        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var legendVm = item as IMainContentVm;

            if (legendVm != null)
            {
                switch (legendVm.MainContentType)
                {
                    case MainContentType.Menu:
                        return MenuTemplate;
                    case MainContentType.Network:
                        return NetworkTemplate;
                    case MainContentType.Sandbox:
                        return SandboxTemplate;
                    case MainContentType.Waffle:
                        return WaffleTemplate;
                    case MainContentType.What:
                        return WhatTemplate;
                    default:
                        throw new Exception($"MainContentType {legendVm.MainContentType} not handled in SelectTemplate");
                }
            }

            return DefaultTemplate;
        }
    }
}
