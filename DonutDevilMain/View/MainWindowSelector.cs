﻿using System;
using System.Windows;
using System.Windows.Controls;
using DonutDevilMain.ViewModel;

namespace DonutDevilMain.View
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
                        throw new Exception($"MainWindowType {legendVm.MainWindowType} not handled in SelectTemplate");
                }
            }

            return DefaultTemplate;
        }
    }
}
