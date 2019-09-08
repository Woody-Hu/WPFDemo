using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFDemo
{
    public static class BarPageUtility
    {
        public static void PrepareBarPage(IBarPage barPage, AppConfig appConfig)
        {
            var imagePath = appConfig.GetCloseButtonImagePath();
            imagePath = imagePath.GetExistPath();
            var content = appConfig.CloseToolTip;
            var closeButton = CreateButton(imagePath, content);
            closeButton.Click += delegate(object sender, RoutedEventArgs args) { barPage.GetWindow().Close(); };
            barPage.GetCloseViewBox().Child = closeButton;

            imagePath = appConfig.GetHomeButtonImagePath();
            imagePath = imagePath.GetExistPath();
            content = appConfig.HomePageToolTip;
            var homePageButton = CreateButton(imagePath, content);
            homePageButton.Click += delegate(object sender, RoutedEventArgs args)
            {
                var homePageFunc = HomePageUtility.HomePageFunc;
                Page homePage = null;
                if (homePageFunc != null)
                {
                    homePage = homePageFunc();
                }

                if (homePage != null)
                {
                    barPage.GetNavigationService().Navigate(homePage);
                }
            };

            barPage.GetHomepageViewBox().Child = homePageButton;

            imagePath = appConfig.GetMinimizeButtonImagePath();
            imagePath = imagePath.GetExistPath();
            content = appConfig.MinimumToolTip;
            var minimizeButton = CreateButton(imagePath, content);
            minimizeButton.Click+= delegate(object sender, RoutedEventArgs args){ barPage.GetWindow().WindowState = WindowState.Minimized;  };
            barPage.GetMinimumViewBox().Child = minimizeButton;

            imagePath = appConfig.GetMaximumButtonImagePath();
            imagePath = imagePath.GetExistPath();
            content = appConfig.MaximumToolTip;
            var maximumButton = CreateButton(imagePath, content);

            imagePath = appConfig.GetRestoreButtonImagePath();
            imagePath = imagePath.GetExistPath();
            content = appConfig.RestoreToolTip;
            var restoreButton = CreateButton(imagePath, content);

            maximumButton.Click += delegate (object sender, RoutedEventArgs args) { barPage.GetWindow().WindowState = WindowState.Maximized;
                barPage.GetMaximumAndRestoreViewBox().Child = restoreButton;
            };
            restoreButton.Click += delegate(object sender, RoutedEventArgs args)
            {
                barPage.GetWindow().WindowState = WindowState.Normal;
                barPage.GetMaximumAndRestoreViewBox().Child = maximumButton;
            };

            if (barPage.GetWindow().WindowState == WindowState.Maximized)
            {
                barPage.GetMaximumAndRestoreViewBox().Child = restoreButton;
            }
            else
            {
                barPage.GetMaximumAndRestoreViewBox().Child = maximumButton;
            }


            if (string.IsNullOrWhiteSpace(appConfig.GetAppTitleImagePath()))
            {
                var textBlock = new TextBlock
                {
                    Text = appConfig.AppTitle
                };
                barPage.GetTitleViewBox().Child = textBlock;
            }
            else
            {
                Image image = new Image
                {
                    Source = new BitmapImage(new Uri(appConfig.GetAppTitleImagePath()))
                };
                barPage.GetTitleViewBox().Child = image;
            }

            var titleBackgroundImagePath = appConfig.GetTitleBarBackgroundImagePath();
            if (!string.IsNullOrWhiteSpace(titleBackgroundImagePath))
            {
                Image image = new Image
                {
                    Source = new BitmapImage(new Uri(titleBackgroundImagePath))
                };

                barPage.GetBarGrid().Background = new ImageBrush(image.Source);
            }

            var list = new List<ImageButton>();
            if (closeButton is ImageButton)
            {
                list.Add(closeButton as ImageButton);
            }

            if (homePageButton is ImageButton)
            {
                list.Add(homePageButton as ImageButton);
            }

            if (minimizeButton is ImageButton)
            {
                list.Add(minimizeButton as ImageButton);
            }

            if (maximumButton is ImageButton)
            {
                list.Add(maximumButton as ImageButton);
            }

            if (restoreButton is ImageButton)
            {
                list.Add(restoreButton as ImageButton);
            }

            if (list.Count == 0)
            {
                list = null;
            }

            barPage.SetBarButtons(list);
        }


        private static Button CreateButton(string imagePath, string content)
        {
            Button button;
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                button = ButtonUtility.CreateButton((Image)null, content, null);
                button.Width = 10;
                button.Height = 10;
                var viewBox = new Viewbox();
                var textBlock = new TextBlock {Text = content};
                viewBox.Child = textBlock;
                button.Content = viewBox;
            }
            else
            {
                var image = new Image {Source = new BitmapImage(new Uri(imagePath))};
                var imageEnterPath = AppConfig.GetMouseEnterImagePath(imagePath);
                button = ButtonUtility.CreateButton(image, content, imageEnterPath);
            }

            return button;
        }
    }
}
