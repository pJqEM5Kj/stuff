using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace WpfApplication1
{
    class AttachedProperties : DependencyObject
    {
        public static readonly DependencyProperty ExtDataProperty = DependencyProperty.RegisterAttached(
            "ExtData",
            typeof(object),
            typeof(AttachedProperties),
            new FrameworkPropertyMetadata(null)
        );

        public static void SetExtDatad(UIElement element, object value)
        {
            element.SetValue(ExtDataProperty, value);
        }

        public static object GetExtData(UIElement element)
        {
            return element.GetValue(ExtDataProperty);
        }
    }
}
