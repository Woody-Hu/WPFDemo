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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFDemo
{
    /// <summary>
    /// Interaction logic for MajorPage.xaml
    /// </summary>
    public partial class MajorPage : Page
    {
        private readonly MajorContext _majorContext;
        private readonly AppConfig _appConfig;
        private readonly int _countPerRow = 4;

        public MajorPage(MajorContext majorContext, AppConfig appConfig)
        {
            _majorContext = majorContext;
            _appConfig = appConfig;
            InitializeComponent();
            var functionInfos = _majorContext.FunctionInfos;
            var columnCount = functionInfos.Count;
            CoreGrid.Rows = 1;
            CoreGrid.Columns = columnCount;
            var index = 0;
            foreach (var oneFunctionInfo in functionInfos)
            {
                var viewBox = new Viewbox();
                var button = ButtonUtility.CreateButton(oneFunctionInfo.Value.ImagePath, oneFunctionInfo.Key);
                if (oneFunctionInfo.Value.Kind == FunctionKind.Video)
                {
                    MajorVideoContext majorVideoContext = GetMajorVideoContext();

                    button.Tag = majorVideoContext;
                    button.Click += VideoButton_Click;
                }
                else if (oneFunctionInfo.Value.Kind == FunctionKind.OpenFile)
                {
                    button.Tag = oneFunctionInfo.Value;
                    button.Click += OpenFileButton_Click; ;
                }

                viewBox.Child = button;
                CoreGrid.Children.Add(viewBox);
                viewBox.SetValue(Grid.RowProperty, 0);
                viewBox.SetValue(Grid.ColumnProperty, index);
                index++;
            }

            PrepareGrid(this.ToolsGrid, this._majorContext.ToolInfos);
            GridUtility.SetBackGround(_majorContext.TopBackgroundImagePath, TopGrid);
            GridUtility.SetBackGround(_majorContext.BottomBackgroundImagePath, BottomGrid);

            if (string.IsNullOrWhiteSpace(_majorContext.AppTitleImagePath))
            {
                var textBlock = new TextBlock();
                textBlock.Text = _majorContext.AppTitle;
                TitleViewBox.Child = textBlock;
            }
            else
            {
                Image image = new Image();
                image.Source = new BitmapImage(new Uri(_majorContext.AppTitleImagePath));
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = image.Source;
                TitleViewBox.Child = image;
            }

            if (!string.IsNullOrWhiteSpace(_majorContext.MajorName))
            {
                var textBlock = new TextBlock();
                textBlock.Text = _majorContext.MajorName;
                PageTitleViewBox.Child = textBlock;
            }
        }

        private MajorVideoContext GetMajorVideoContext()
        {
            var majorVideoContext = new MajorVideoContext();
            majorVideoContext.VideoFileNames = GetVideoFileNames(_majorContext.MajorName);
            majorVideoContext.ToolInfos = _majorContext.ToolInfos;
            var currentPath = System.Environment.CurrentDirectory;
            var pluginPath = System.IO.Path.Combine(currentPath, _appConfig.PluginFolderName);
            var palyImagePath = System.IO.Path.Combine(pluginPath, AppConfig.VideoPlayImageName);
            var pauseImagePath = System.IO.Path.Combine(pluginPath, AppConfig.VideoPauseImageName);
            palyImagePath = System.IO.File.Exists(palyImagePath) ? palyImagePath : string.Empty;
            pauseImagePath = System.IO.File.Exists(pauseImagePath) ? pauseImagePath : string.Empty;
            majorVideoContext.VideoStartButtonImagePath = palyImagePath;
            majorVideoContext.VideoPauseButtonImagePath = pauseImagePath;
            return majorVideoContext;
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var info = (sender as Button).Tag as FunctionInfo;
            FolderUtility.OpenFile(info.ProgramName, info.FilePath);
        }

        private void VideoButton_Click(object sender, RoutedEventArgs e)
        {
            var majorVideoContext = (sender as Button).Tag as MajorVideoContext;
            this.NavigationService.Navigate(new VideosPage(majorVideoContext, _appConfig));
        }

        private IList<string> GetVideoFileNames(string majorName)
        {
            var currentPath = System.Environment.CurrentDirectory;
            var videoFolderPath = _majorContext.FunctionInfos.FirstOrDefault(k=>k.Value.Kind == FunctionKind.Video).Value?.VideosPath;
            if (videoFolderPath == null)
            {
                return new List<string>();
            }

            if (Directory.Exists(videoFolderPath))
            {
                Directory.CreateDirectory(videoFolderPath);
            }

            var files = Directory.GetFiles(videoFolderPath);
            var listVoideoFiles = new List<string>();
            var set = new HashSet<string>();
            foreach (var oneFormate in _appConfig.VideoFormates)
            {
                set.Add(oneFormate.ToLower());
            }

            foreach (var oneFile in files)
            {
                var extension = System.IO.Path.GetExtension(oneFile).ToLower();
                if (set.Contains(extension))
                {
                    listVoideoFiles.Add(oneFile);
                }
            }

            return listVoideoFiles;
        }

        private void PrepareGrid(System.Windows.Controls.Primitives.UniformGrid majorsGrid, IDictionary<string, FolderInfo> infos)
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
                button.Click += ToolButton_Click;
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

        private void ToolButton_Click(object sender, RoutedEventArgs e)
        {
            var path = (sender as Button).Tag;
            FolderUtility.ExploreFolder(path.ToString());
        }
    }
}
