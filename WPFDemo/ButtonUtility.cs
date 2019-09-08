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
        public static Button CreateButton(string imagePath, string content, string moveEnterImagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return CreateButton((Image)null, content, null);
            }
            else
            {
                var image = new Image {Source = new BitmapImage(new Uri(imagePath))};
                return CreateButton(image, content, moveEnterImagePath);
            }
        }

        public static Button CreateButton(Image image, string content, string moveEnterImagePath)
        {
            Button button;
            if (image == null)
            {
                button = new Button {Content = content};
            }
            else
            {
                button = new ImageButton { Content = image };
                ButtonImages.SetButtonImages(button, null);
                if (!string.IsNullOrWhiteSpace(moveEnterImagePath))
                {
                    var moveEnterImage = new Image { Source = new BitmapImage(new Uri(moveEnterImagePath)) };
                    var images = new Image[] { image, moveEnterImage };
                    ButtonImages.SetButtonImages(button, images);
                }
                else
                {
                    ButtonImages.SetButtonImages(button, null);
                }

            }

            button.HorizontalAlignment = HorizontalAlignment.Center;
            button.ToolTip = content;
            button.MouseEnter += Button_MouseEnter;
            button.MouseLeave += Button_MouseLeave;
            return button;
        }


        private static void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            var imageButton = sender as ImageButton;
            if (imageButton != null && ButtonImages.GetButtonImages(imageButton) == null)
            {
                ((Button)sender).Margin = new Thickness(0);
            }
        }

        private static void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            var imageButton = sender as ImageButton;
            if (imageButton != null && ButtonImages.GetButtonImages(imageButton) == null)
            {
                ((Button)sender).Margin = new Thickness(-5);
            }
        }
    }
}
