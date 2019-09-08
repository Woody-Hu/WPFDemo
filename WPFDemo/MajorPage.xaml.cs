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
    public partial class MajorPage : Page,IBarPage
    {
        private readonly MajorContext _majorContext;
        private readonly AppConfig _appConfig;
        private readonly Window _window;
        private IList<ImageButton> _imageButtons;

        public MajorPage(MajorContext majorContext, AppConfig appConfig, Window window)
        {
            _majorContext = majorContext;
            _appConfig = appConfig;
            _window = window;
            InitializeComponent();
            PrepareGrid();
            PrepareGrid(this.ToolsGrid, this._majorContext.ResourceInfos);
            var majorBackgroundImagePath = appConfig.GetMajorBackgroundImagePath(_majorContext.MajorName);
            if (!string.IsNullOrWhiteSpace(majorBackgroundImagePath))
            {
                GridUtility.SetBackGround(majorBackgroundImagePath, MainGrid);
            }
            else
            {
                GridUtility.SetBackGround(_majorContext.TopBackgroundImagePath, TopGrid);
                GridUtility.SetBackGround(_majorContext.BottomBackgroundImagePath, BottomGrid);
            }

            if (!string.IsNullOrWhiteSpace(_majorContext.MajorName) && appConfig.ShowMajorNameAtMajorPage)
            {
                Title = _majorContext.MajorName;
                var textBlock = new TextBlock
                {
                    Text = _majorContext.MajorName
                };
                PageTitleViewBox.Child = textBlock;
            }

            BarPageUtility.PrepareBarPage(this, _appConfig);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_imageButtons == null)
            {
                return;
            }

            foreach (var oneImageButton in _imageButtons)
            {
                var images = ButtonImages.GetButtonImages(oneImageButton);
                if (images == null)
                {
                    return;
                }

                if (oneImageButton.IsMouseOver)
                {
                    var image = images[1];
                    oneImageButton.Content = image;
                }
                else
                {
                    var image = images[0];
                    oneImageButton.Content = image;
                }
            }


            base.OnMouseMove(e);
        }

        private void PrepareGrid()
        {
            var functionInfos = _majorContext.FunctionInfos;
            int columnCount, useRowIndex, useColumnIndex;
            GridUtility.PrepareBuutonGrid(CoreGrid, functionInfos.Count, true, out columnCount, out useRowIndex, out useColumnIndex);

            foreach (var oneFunctionInfo in functionInfos)
            {
                var button = ButtonUtility.CreateButton(oneFunctionInfo.Value.ImagePath, oneFunctionInfo.Key,
                    oneFunctionInfo.Value.MouseEnterImagePath);
                if (oneFunctionInfo.Value.Kind == FunctionKind.Video)
                {
                    var majorVideoContext = GetMajorVideoContext();

                    button.Tag = majorVideoContext;
                    button.Click += NavigateVideoPageButton_Click;
                    if (!_appConfig.ShowVideoButtonToolTip)
                    {
                        button.ToolTip = null;
                    }
                }
                else if (oneFunctionInfo.Value.Kind == FunctionKind.OpenFile)
                {
                    button.Tag = oneFunctionInfo.Value;
                    button.Click += OpenFileButton_Click;
                    ;
                }

                GridUtility.SetButton(CoreGrid, button, true, columnCount, ref useRowIndex, ref useColumnIndex);
            }
        }

        private MajorVideoContext GetMajorVideoContext()
        {
            var majorVideoContext = new MajorVideoContext
            {
                VideoFileNames = GetVideoFileNames(),
                ResourceInfos = _majorContext.ResourceInfos
            };

            var playImagePath = _appConfig.GetVideoPlayImagePath();
            var pauseImagePath = _appConfig.GetVideoPauseImagePath();
            playImagePath = File.Exists(playImagePath) ? playImagePath : string.Empty;
            pauseImagePath = File.Exists(pauseImagePath) ? pauseImagePath : string.Empty;
            majorVideoContext.VideoStartButtonImagePath = playImagePath;
            majorVideoContext.VideoStartButtonMouseEnterImagePath = AppConfig.GetMouseEnterImagePath(playImagePath);
            majorVideoContext.VideoPauseButtonImagePath = pauseImagePath;
            majorVideoContext.VideoPauseButtonMouseEnterImagePath = AppConfig.GetMouseEnterImagePath(pauseImagePath);
            majorVideoContext.MajorName = _majorContext.MajorName;
            return majorVideoContext;
        }

        private IList<string> GetVideoFileNames()
        {
            var videoFolderPath = _majorContext.FunctionInfos.FirstOrDefault(k => k.Value.Kind == FunctionKind.Video).Value?.VideosPath;
            if (videoFolderPath == null)
            {
                return new List<string>();
            }

            if (!Directory.Exists(videoFolderPath))
            {
                return new List<string>();
            }

            var files = Directory.GetFiles(videoFolderPath);
            var listVideoFiles = new List<string>();
            var set = new HashSet<string>();
            foreach (var oneFormat in _appConfig.VideoFormats)
            {
                set.Add(oneFormat.ToLower());
            }

            foreach (var oneFile in files)
            {
                var extension = System.IO.Path.GetExtension(oneFile).ToLower().TrimStart('.');
                if (set.Contains(extension))
                {
                    listVideoFiles.Add(oneFile);
                }
            }

            return listVideoFiles;
        }

        private void PrepareGrid(Grid grid, IDictionary<string, FolderInfo> infos)
        {
            int columnCount, useRowIndex, useColumnIndex;
            GridUtility.PrepareBuutonGrid(grid, infos.Count, false, out columnCount, out useRowIndex, out useColumnIndex);

            foreach (var oneInfoPair in infos)
            {
                var button = ButtonUtility.CreateButton(oneInfoPair.Value.ImagePath, oneInfoPair.Key, oneInfoPair.Value.MoveEnterImagePath);
                button.Tag = oneInfoPair.Value.Path;
                button.Click += OpenFolderButton_Click;
                GridUtility.SetButton(grid, button, false, columnCount, ref useRowIndex, ref useColumnIndex);
            }
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is FunctionInfo info) FolderUtility.OpenFile(info.ProgramName, info.GetFileFullPath(), info.WorkDirection);
        }

        private void NavigateVideoPageButton_Click(object sender, RoutedEventArgs e)
        {
            var majorVideoContext = (sender as Button)?.Tag as MajorVideoContext;
            var navigationService = this.NavigationService;
            navigationService?.Navigate(new VideosPage(majorVideoContext, _appConfig,_window));
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var path = (sender as Button)?.Tag;
            if (path != null) FolderUtility.ExploreFolder(path.ToString());
        }

        private void BarGrid_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _window.DragMove();
            }
        }

        #region interface
        public Window GetWindow()
        {
            return _window;
        }

        public Viewbox GetHomepageViewBox()
        {
            return HomePageButtonViewBox;
        }

        public Viewbox GetMinimumViewBox()
        {
            return MinimizeButtonViewBox;
        }

        public Viewbox GetMaximumAndRestoreViewBox()
        {
            return MaximumRestoreButtonViewBox;
        }

        public Viewbox GetCloseViewBox()
        {
            return CloseButtonViewBox;
        }

        public Grid GetBarGrid()
        {
            return BarGrid;
        }

        public Viewbox GetTitleViewBox()
        {
            return TitleViewBox;
        }

        public NavigationService GetNavigationService()
        {
            return NavigationService;
        }

        public void SetBarButtons(IList<ImageButton> imageButtons)
        {
            if (imageButtons == null)
            {
                return;
            }

            if (_imageButtons == null)
            {
                _imageButtons = imageButtons;
            }
        }

        #endregion
    }
}
