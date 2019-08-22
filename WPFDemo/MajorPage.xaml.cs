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
                    var majorVideoContext = GetMajorVideoContext();

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
                var textBlock = new TextBlock
                {
                    Text = _majorContext.AppTitle
                };
                TitleViewBox.Child = textBlock;
            }
            else
            {
                Image image = new Image
                {
                    Source = new BitmapImage(new Uri(_majorContext.AppTitleImagePath))
                };
                TitleViewBox.Child = image;
            }

            if (!string.IsNullOrWhiteSpace(_majorContext.MajorName))
            {
                var textBlock = new TextBlock
                {
                    Text = _majorContext.MajorName
                };
                PageTitleViewBox.Child = textBlock;
            }
        }

        private MajorVideoContext GetMajorVideoContext()
        {
            var majorVideoContext = new MajorVideoContext
            {
                VideoFileNames = GetVideoFileNames(),
                ToolInfos = _majorContext.ToolInfos
            };
            var currentPath = Environment.CurrentDirectory;
            var pluginPath = System.IO.Path.Combine(currentPath, _appConfig.PluginFolderName);
            var playImagePath = System.IO.Path.Combine(pluginPath, AppConfig.VideoPlayImageName);
            var pauseImagePath = System.IO.Path.Combine(pluginPath, AppConfig.VideoPauseImageName);
            playImagePath = File.Exists(playImagePath) ? playImagePath : string.Empty;
            pauseImagePath = File.Exists(pauseImagePath) ? pauseImagePath : string.Empty;
            majorVideoContext.VideoStartButtonImagePath = playImagePath;
            majorVideoContext.VideoPauseButtonImagePath = pauseImagePath;
            return majorVideoContext;
        }

        private IList<string> GetVideoFileNames()
        {
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
            var listVideoFiles = new List<string>();
            var set = new HashSet<string>();
            foreach (var oneFormat in _appConfig.VideoFormates)
            {
                set.Add(oneFormat.ToLower());
            }

            foreach (var oneFile in files)
            {
                var extension = System.IO.Path.GetExtension(oneFile).ToLower();
                if (set.Contains(extension))
                {
                    listVideoFiles.Add(oneFile);
                }
            }

            return listVideoFiles;
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
                button.Tag = oneInfoPair.Value.Path;
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

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is FunctionInfo info) FolderUtility.OpenFile(info.ProgramName, info.GetFileFullPath());
        }

        private void VideoButton_Click(object sender, RoutedEventArgs e)
        {
            var majorVideoContext = (sender as Button)?.Tag as MajorVideoContext;
            var navigationService = this.NavigationService;
            navigationService?.Navigate(new VideosPage(majorVideoContext, _appConfig));
        }

        private void ToolButton_Click(object sender, RoutedEventArgs e)
        {
            var path = (sender as Button)?.Tag;
            if (path != null) FolderUtility.ExploreFolder(path.ToString());
        }
    }
}
