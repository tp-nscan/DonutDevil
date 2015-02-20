using System;
using System.Windows;

namespace WpfUtils
{
    public class StateHelper
    {
        public static readonly DependencyProperty StateProperty = DependencyProperty.RegisterAttached(
            "State",
            typeof(String),
            typeof(StateHelper),
            new PropertyMetadata(string.Empty, StateChanged));

        internal static void StateChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            var newState = args.NewValue as String;
            var frameworkElement = target as FrameworkElement;
            if (frameworkElement == null) return;

            if (String.IsNullOrEmpty(newState))
            {
                return;
            }
            VisualStateManager.GoToState(frameworkElement, newState, false);
        }

        public static void SetState(UIElement element, string value)
        {
            element.SetValue(StateProperty, value);
        }

        public static string GetOrientation(UIElement element)
        {
            return (string)element.GetValue(StateProperty);
        }
    }
}
