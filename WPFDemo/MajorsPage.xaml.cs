﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFDemo
{
    /// <summary>
    /// Interaction logic for MajorsPage.xaml
    /// </summary>
    public partial class MajorsPage : Page
    {
        private readonly MajorsContext _majorsContext;
        private readonly AppConfig _appConfig;
        private readonly int _countPerRow = 4;

        public MajorsPage(MajorsContext majorsContext, AppConfig appConfig)
        {
            InitializeComponent();
            _majorsContext = majorsContext;
            _appConfig = appConfig;

            var majorsGrid = this.MajorsGrid;
            PrepareGrid(majorsGrid, this._majorsContext.MajorInfos,true);
            PrepareGrid(this.ToolsGrid, this._majorsContext.ToolInfos,false);

            GridUtility.SetBackGround(_majorsContext.TopBackgroundImagePath, TopGrid);
            GridUtility.SetBackGround(_majorsContext.BottomBackgroundImagePath, BottomGrid);

            if (string.IsNullOrWhiteSpace(_majorsContext.AppTitleImagePath))
            {
                var textBlock = new TextBlock();
                textBlock.Text = _majorsContext.AppTitle;
                TitleViewBox.Child = textBlock;
            }
            else
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri(_majorsContext.AppTitleImagePath));
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = image.Source;
                TitleViewBox.Child = image;
            }

            if (!string.IsNullOrWhiteSpace(_majorsContext.AppTitle))
            {
                this.WindowTitle = _majorsContext.AppTitle;
            }
        }

        private void PrepareGrid(System.Windows.Controls.Primitives.UniformGrid majorsGrid, IDictionary<string, FolderInfo> infos, bool isMajorRequest)
        {
            var rowCount = infos.Count / _countPerRow;
            if (infos.Count % _countPerRow != 0)
            {
                rowCount++;
            }
            var columnCount = rowCount == 1 ? infos.Count % _countPerRow : _countPerRow;
            majorsGrid.Rows = rowCount;
            majorsGrid.Columns = columnCount;
            var useRowIndex = 0;
            var useColumnIndex = 0;
            foreach (var oneInfoPair in infos)
            {
                var viewBox = new Viewbox();
                var button = ButtonUtility.CreateButton(oneInfoPair.Value.ImagePath, oneInfoPair.Key);
                if (isMajorRequest)
                {
                    MajorContext majorContext = MajorsContext.PrepareMajorContext(_majorsContext, _appConfig, oneInfoPair);

                    button.Tag = majorContext;
                    button.Click += MajorButton_Click;
                }
                else
                {
                    button.Tag = oneInfoPair.Value.Path;
                    button.Click += ToolButton_Click;
                }

                viewBox.Child = button;
                majorsGrid.Children.Add(viewBox);
                viewBox.SetValue(Grid.RowProperty, useRowIndex);
                viewBox.SetValue(Grid.ColumnProperty, useColumnIndex);

                useColumnIndex++;
                if (useColumnIndex >= columnCount)
                {
                    useRowIndex++;
                    useColumnIndex = 0;
                }
            }
        }

        private void MajorButton_Click(object sender, RoutedEventArgs e)
        {
            var majorContext = (sender as Button).Tag as MajorContext;
            this.NavigationService.Navigate(new MajorPage(majorContext, _appConfig));
        }

        private void ToolButton_Click(object sender, RoutedEventArgs e)
        {
            var path = (sender as Button).Tag;
            FolderUtility.ExploreFolder(path.ToString());
        }
    }
}
