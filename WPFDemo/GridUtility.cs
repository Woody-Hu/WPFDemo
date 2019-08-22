using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPFDemo
{
    public static class GridUtility
    {
        public static void SetBackGround(string path, Grid majorsGrid)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri(path));
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = image.Source;
                majorsGrid.Background = ib;
            }
        }
    }
}
