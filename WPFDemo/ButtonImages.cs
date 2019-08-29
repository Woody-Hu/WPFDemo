using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFDemo
{
    public class ButtonImages: DependencyObject
    {
        public static Image[] GetButtonImages(DependencyObject obj)
        {
            return (Image[])obj.GetValue(Property);
        }

        public static void SetButtonImages(DependencyObject obj, Image[] value)
        {
            obj.SetValue(Property, value);
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Property = DependencyProperty.RegisterAttached(nameof(ButtonImages), typeof(Image[]), typeof(ButtonImages));
    }
}
