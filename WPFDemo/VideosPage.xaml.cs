using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace WPFDemo
{
    /// <summary>
    /// Interaction logic for VideosPage.xaml
    /// </summary>
    public partial class VideosPage : Page, IBarPage
    {
        private readonly object _timerLock = new object();
        private readonly MajorVideoContext _majorVideoContext;
        private readonly AppConfig _appConfig;
        private int _currentIndex = -1;
        private bool _played = false;
        private DispatcherTimer _timer ;
        private int _progressState = 0;
        private Button _videoControlButton;
        private Image _playButtonImage;
        private Image _pauseButtonImage;
        private double _lastProgress = 0.0d;
        private readonly Window _window;
        private IList<ImageButton> _imageButtons;

        public VideosPage(MajorVideoContext majorVideoContext, AppConfig appConfig, Window window)
        {
            _appConfig = appConfig;
            _majorVideoContext = majorVideoContext;
            _window = window;
            var listViewItemCommand = new ListViewItemCommand(this);
            InitializeComponent();
            VideoPlayer.UnloadedBehavior = MediaState.Close;
            VideoPlayer.MediaEnded += VideoPlayer_MediaEnded;
            VideoPlayer.MediaOpened += VideoPlayer_MediaOpened;

            foreach (var keyValuePair in _majorVideoContext.ResourceInfos)
            {
                var viewBox = new Viewbox();
                var button = ButtonUtility.CreateButton(keyValuePair.Value.ImagePath, keyValuePair.Key, keyValuePair.Value.MoveEnterImagePath);
                button.Tag = keyValuePair.Value.Path;
                button.Click += OpenFolderButton_Click;
                viewBox.Child = button;
                this.ToolPanel.Children.Add(viewBox);
            }

            PreparePlayControlButton(appConfig);
            PrepareNoneControlVideoButtons();
            PrepareListView(listViewItemCommand);

            if (!string.IsNullOrWhiteSpace(appConfig.VideoPageTitle) && !string.IsNullOrWhiteSpace(majorVideoContext.MajorName))
            {
                var title = $"{majorVideoContext.MajorName}.{appConfig.VideoPageTitle}";
                Title = title;
            }

            var videoBottomGridBackgroundImagePath = _appConfig.GetVideoBottomGridBackgroundImagePath();
            if (!string.IsNullOrWhiteSpace(videoBottomGridBackgroundImagePath))
            {
                var image = new Image
                {
                    Source = new BitmapImage(new Uri(videoBottomGridBackgroundImagePath))
                };

                BottomGrid.Background = new ImageBrush(image.Source);
            }

            this.Unloaded += VideosPage_Unloaded;
            BarPageUtility.PrepareBarPage(this, _appConfig);
        }

        internal void VideoPlayerControlMethod()
        {
            if (_currentIndex == -1)
            {
                return;
            }

            if (!_played)
            {
                StartTimer();
                VideoPlayer.Play();
            }
            else
            {
                StopTimer();
                VideoPlayer.Pause();
            }

            _played = !_played;
            if (_videoControlButton is VideoImageButton videoImageButton)
            {
                videoImageButton.ChangePlayed(_played);
            }
            else if (_videoControlButton is ImageButton)
            {
                _videoControlButton.Content = _played ? _pauseButtonImage : _playButtonImage;
            }
            else
            {
                _videoControlButton.Content = _played ? "Pause" : "Play";
            }

            _videoControlButton.ToolTip = _played ? _appConfig.VideoPauseToolTip : _appConfig.VideoPlayToolTip;
        }

        internal void SelectNextVideo()
        {
            if (_currentIndex == -1)
            {
                return;
            }

            var index = _currentIndex + 1;
            if (index == VideoFilesListView.Items.Count)
            {
                index = 0;
            }

            ChangeCurrentIndex(index);
        }

        internal void SelectLastVideo()
        {
            if (_currentIndex == -1)
            {
                return;
            }

            var index = _currentIndex - 1;
            if (index == -1)
            {
                index = VideoFilesListView.Items.Count - 1;
            }

            ChangeCurrentIndex(index);
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

        private void PrepareListView(ListViewItemCommand listViewItemCommand)
        {
            var fileIndex = 0;
            foreach (var oneFileName in _majorVideoContext.VideoFileNames)
            {
                var listViewItem = new ListViewItem
                {
                    Content = System.IO.Path.GetFileNameWithoutExtension(oneFileName),
                    Tag = fileIndex
                };
                ControlDoubleClick.SetCommand(listViewItem, listViewItemCommand);
                this.VideoFilesListView.Items.Add(listViewItem);
                fileIndex++;
            }

            if (_majorVideoContext.VideoFileNames.Count > 0)
            {
                this.VideoFilesListView.SelectedIndex = 0;
                ChangeCurrentIndex(0);
            }

            var videoFilesListViewBackgroundImagePath = _appConfig.GetVideoFilesListViewBackgroundImagePath();
            if (!string.IsNullOrWhiteSpace(videoFilesListViewBackgroundImagePath))
            {
                var image = new Image
                {
                    Source = new BitmapImage(new Uri(videoFilesListViewBackgroundImagePath))
                };

                VideoFilesListView.Background = new ImageBrush(image.Source);
            }
        }

        private void PreparePlayControlButton(AppConfig appConfig)
        {
            if (CanCreateVideoImageButton())
            {
                var list = new List<Image>();
                var pathList = new List<string>()
                {
                    _majorVideoContext.VideoStartButtonImagePath, _majorVideoContext.VideoStartButtonMouseEnterImagePath,
                    _majorVideoContext.VideoPauseButtonImagePath, _majorVideoContext.VideoPauseButtonMouseEnterImagePath
                };
                foreach (var onePath in pathList)
                {
                    list.Add(new Image
                    {
                        Source = new BitmapImage(new Uri(onePath))
                    });
                }

                _videoControlButton = new VideoImageButton(list.ToArray());
                _videoControlButton.Content = list[0];
            }
            else if (CanCreateImageButton())
            {
                _playButtonImage = new Image
                {
                    Source = new BitmapImage(new Uri(_majorVideoContext.VideoStartButtonImagePath))
                };
                _pauseButtonImage = new Image
                {
                    Source = new BitmapImage(new Uri(_majorVideoContext.VideoPauseButtonImagePath))
                };

                _videoControlButton = ButtonUtility.CreateButton(_playButtonImage, "Play", null);
            }
            else
            {
                _videoControlButton = ButtonUtility.CreateButton((Image)null, "Play", null);
            }

            _videoControlButton.ToolTip = appConfig.VideoPlayToolTip;
            _videoControlButton.Click += VideoPlayerControl_Click;
            var buttonViewBox = new Viewbox { Child = _videoControlButton };
            PlayButtonGrid.Children.Add(buttonViewBox);
        }

        private void VideosPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _lastProgress = VideoSlider.Value;
            if (_played)
            {
                StopTimer();
            }
        }

        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            VideoSlider.Maximum = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            if (_lastProgress >= 0.0)
            {
                VideoPlayer.Position = TimeSpan.FromSeconds(_lastProgress);
                VideoSlider.Value = _lastProgress;
                _lastProgress = 0.0;
            }
            else
            {
                VideoSlider.Value = 0;
            }

            ReCreateTimer();
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            StopTimer();
            var files = _majorVideoContext.VideoFileNames;
            if (_currentIndex >= files.Count - 1)
            {
                _currentIndex = -1;
            }
            _currentIndex++;
            ChangeCurrentIndex(_currentIndex);
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button)) return;
            var folderPath = button.Tag.ToString();
            FolderUtility.ExploreFolder(folderPath);
        }

        private void ChangeCurrentIndex(int index)
        {
            var nowCurrentIndex = _currentIndex;
            var nowPaused = !_played;
            VideoPlayer.Stop();
            _currentIndex = index;
            var files = _majorVideoContext.VideoFileNames;
            //same index release
            if (nowCurrentIndex == _currentIndex)
            {
                VideoPlayer.Source = null;
            }

            VideoPlayer.Source = new Uri(files[_currentIndex]);
            this.VideoFilesListView.SelectedIndex = _currentIndex;

            if (!nowPaused)
            {
                VideoPlayer.Play();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!_played)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref _progressState, 0, 0) == 0)
            {
                VideoSlider.Value = VideoPlayer.Position.TotalSeconds;
            }
        }

        private void VideoSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            //handle when video have not played
            if (!_played)
            {
                VideoPlayer.Play();
                VideoPlayer.Pause();
            }

            VideoPlayer.Position = TimeSpan.FromSeconds(VideoSlider.Value);
            Interlocked.CompareExchange(ref _progressState, 0, 1);
        }

        private void VideoSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            Interlocked.CompareExchange(ref _progressState, 1, 0);
        }

        private void AudioSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            VideoPlayer.Volume = AudioSlider.Value;
        }

        private void VideoPlayerControl_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayerControlMethod();
        }

        private void StartTimer()
        {
            lock (_timerLock)
            {
                _timer?.Start();
            }
        }

        private void StopTimer()
        {
            lock (_timerLock)
            {
                _timer.Stop();
            }
        }

        private void ReCreateTimer()
        {
            lock (_timerLock)
            {
                _timer?.Stop();
                _timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                _timer.Tick += Timer_Tick;
                _timer.Start();
            }
        }

        private bool CanCreateImageButton()
        {
            return !string.IsNullOrWhiteSpace(_majorVideoContext.VideoStartButtonImagePath) && !string.IsNullOrWhiteSpace(_majorVideoContext.VideoPauseButtonImagePath);
        }

        private bool CanCreateVideoImageButton()
        {
            return !string.IsNullOrWhiteSpace(_majorVideoContext.VideoStartButtonImagePath) && !string.IsNullOrWhiteSpace(_majorVideoContext.VideoPauseButtonImagePath) && !string.IsNullOrWhiteSpace(_majorVideoContext.VideoPauseButtonMouseEnterImagePath) && !string.IsNullOrWhiteSpace(_majorVideoContext.VideoStartButtonMouseEnterImagePath);
        }

        private void PrepareNoneControlVideoButtons()
        {
            var expandFoldeVideoListViewBox = new Viewbox();
            ExpandFoldVideoListButtonGrid.Children.Add(expandFoldeVideoListViewBox);
            var selectLastVideoViewBox = new Viewbox();
            SelectLastVideoButtonGrid.Children.Add(selectLastVideoViewBox);
            var selectNextVideoViewBox = new Viewbox();
            SelectNextVideoButtonGrid.Children.Add(selectNextVideoViewBox);
            var selectLastVideoButton = CreateButton(_appConfig.GetSelectLastVideoImagePath(), _appConfig.SelectLastVideoToolTip);
            selectLastVideoButton.Click += delegate (object sender, RoutedEventArgs args)
            {
                SelectLastVideo();
            };

            var selectNextVideoButton = CreateButton(_appConfig.GetSelectNextVideoImagePath(), _appConfig.SelectNextVideoToolTip);
            selectNextVideoButton.Click += delegate (object sender, RoutedEventArgs args)
            {
                SelectNextVideo();
            };

            var expandVideoListButton = CreateButton(_appConfig.GetExpandVideoListImagePath(), _appConfig.ExpandVideoListToolTip);
            var foldVideoListButton = CreateButton(_appConfig.GetFoldVideoListImagePath(), _appConfig.FoldVideoListToolTip);
            expandVideoListButton.Click += delegate (object sender, RoutedEventArgs args)
            {
                MainGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
                expandFoldeVideoListViewBox.Child = foldVideoListButton;
            };

            foldVideoListButton.Click += delegate (object sender, RoutedEventArgs args)
            {
                MainGrid.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Star);
                expandFoldeVideoListViewBox.Child = expandVideoListButton;
            };

            expandFoldeVideoListViewBox.Child = foldVideoListButton;
            selectLastVideoViewBox.Child = selectLastVideoButton;
            selectNextVideoViewBox.Child = selectNextVideoButton;
        }

        private Button CreateButton(string imagePath, string content)
        {
            var imageMovePath = AppConfig.GetMouseEnterImagePath(imagePath);
            var createdButton = ButtonUtility.CreateButton(imagePath, content, imageMovePath);
            return createdButton;
        }

        private class ListViewItemCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;

            private readonly VideosPage _videosPage;

            internal ListViewItemCommand(VideosPage videosPage)
            {
                _videosPage = videosPage;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                if (parameter == null || !int.TryParse(parameter.ToString(), out var index))
                {
                    return;
                }

                _videosPage.ChangeCurrentIndex(index);
            }
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
