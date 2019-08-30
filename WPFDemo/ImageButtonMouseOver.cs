using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFDemo
{
    public static class ImageButtonMouseOver
    {
        public static DependencyProperty ImageButtonMouseOverProperty =
            DependencyProperty.RegisterAttached("ImageButtonMouseOver", typeof(bool),
                typeof(ImageButtonMouseOver), new UIPropertyMetadata(false, OnImageButtonMouseOver));

        public static bool GetImageButtonMouseOver(DependencyObject obj)
        {
            return (bool)obj.GetValue(ImageButtonMouseOverProperty);
        }

        public static void SetImageButtonMouseOver(DependencyObject obj, bool value)
        {
            obj.SetValue(ImageButtonMouseOverProperty, value);
        }

        public static void OnImageButtonMouseOver(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var imageButton = d as ImageButton;
            if (imageButton == null)
            {
                return;
            }

            var images = ButtonImages.GetButtonImages(imageButton);
            if ((bool) e.NewValue)
            {
                var image = images[1];
                imageButton.Content = image;
            }
            else
            {
                var image = images[0];
                imageButton.Content = image;
            }
        }

    }
}
