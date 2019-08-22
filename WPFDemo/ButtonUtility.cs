using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFDemo
{
    public static class ButtonUtility
    {
        public static Button CreateButton(string imagePath, string content)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return CreateButton((Image)null, content);
            }
            else
            {
                var image = new Image {Source = new BitmapImage(new Uri(imagePath))};
                return CreateButton(image, content);
            }
        }

        public static Button CreateButton(Image image, string content, bool useClip = true)
        {
            Button button;
            if (image == null)
            {
                button = new Button {Content = content};
                if (useClip)
                {
                    button.Width = 120;
                    button.Height = 120;
                    button.Clip = new EllipseGeometry() { Center = new Point(60, 60), RadiusX = 50, RadiusY = 50 };
                }
            }
            else
            {
                button = new ImageButton {Content = image};
            }
            button.Margin = new Thickness(10);
            button.HorizontalAlignment = HorizontalAlignment.Center;
            button.ToolTip = content;
            button.MouseEnter += Button_MouseEnter;
            button.MouseLeave += Button_MouseLeave;

            return button;
        }

        private static void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Button) sender).Margin = new Thickness(10);
        }

        private static void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button) sender).Margin = new Thickness(0);
        }
    }
}
