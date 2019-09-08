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
    public static class GridUtility
    {
        private const int _countPerRow = 4;
        private const int _mainButtonSpan = 33;
        private const int _mainWhiteSpaceSpan = 13;
        private const int _subButtonSpan = 12;
        private const int _subWhiteSpaceSpan = 11;

        public static void SetBackGround(string path, Grid majorsGrid)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                var image = new Image {Source = new BitmapImage(new Uri(path))};
                var ib = new ImageBrush {ImageSource = image.Source};
                majorsGrid.Background = ib;
            }
        }

        public static void PrepareBuutonGrid(Grid grid, int count, bool isMainGrid, out int columnCount, out int useRowIndex, out int useColumnIndex)
        {
            var rowCount = count / _countPerRow;
            if (count % _countPerRow != 0)
            {
                rowCount++;
            }

            columnCount = 0;
            if (isMainGrid)
            {
                columnCount = _mainButtonSpan * _countPerRow + _mainWhiteSpaceSpan * (_countPerRow - 1);
            }
            else
            {
                columnCount = _subButtonSpan * _countPerRow + _subWhiteSpaceSpan * (_countPerRow - 1);
            }

            columnCount = 2 * columnCount;
            for (int i = 0; i < rowCount; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 0; i < columnCount; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }

            useRowIndex = 0;
            useColumnIndex = 0;
            if (rowCount == 1)
            {
                if (isMainGrid)
                {
                    useColumnIndex = (columnCount - count * _mainButtonSpan * 2 - (count - 1) * _mainWhiteSpaceSpan * 2) / 2;
                }
                else
                {
                    useColumnIndex = (columnCount - count * _subButtonSpan * 2 - (count - 1) * _subWhiteSpaceSpan * 2) / 2;
                }
            }
        }

        public static void SetButton(Grid grid, Button button, bool isMainGrid, int columnCount, ref int useRowIndex, ref int useColumnIndex)
        {
            var viewBox = new Viewbox();
            viewBox.Stretch = Stretch.Uniform;
            viewBox.Child = button;
            grid.Children.Add(viewBox);
            viewBox.SetValue(Grid.RowProperty, useRowIndex);
            viewBox.SetValue(Grid.ColumnProperty, useColumnIndex);
            var span = isMainGrid ? _mainButtonSpan: _subButtonSpan;
            var whiteSpaceSpan = isMainGrid ? _mainWhiteSpaceSpan : _subWhiteSpaceSpan;
            viewBox.SetValue(Grid.ColumnSpanProperty, 2 * span);

            if (isMainGrid)
            {
                button.Width = 215;
                button.Height = 193;
            }
            else
            {
                button.Width = 136;
                button.Height = 174;
            }
 
            useColumnIndex = useColumnIndex + 2*span + 2*whiteSpaceSpan;
            if (useColumnIndex >= columnCount)
            {
                useRowIndex++;
                useColumnIndex = 0;
            }
        }
    }
}
