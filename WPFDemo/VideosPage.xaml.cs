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
    public partial class VideosPage : Page
    {
        private readonly object _timerLock = new object();
        private readonly MajorVideoContext _majorVideoContext;
        private readonly AppConfig _appConfig;
        private int _currentIndex = -1;
        private bool _played = false;
        private DispatcherTimer _timer ;
        private int _progressState = 0;
        private readonly Button _videoControlButton;
        private readonly Image _playButtonImage;
        private readonly Image _pauseButtonImage;
        private double _lastProgress = 0.0d;

        public VideosPage(MajorVideoContext majorVideoContext, AppConfig appConfig)
        {
            _appConfig = appConfig;
            _majorVideoContext = majorVideoContext;
            var listViewItemCommand = new ListViewItemCommand(this);
            InitializeComponent();
            VideoPlayer.UnloadedBehavior = MediaState.Close;
            foreach (var keyValuePair in _majorVideoContext.ResourceInfos)
            {
                var viewBox = new Viewbox();
                var button = ButtonUtility.CreateButton(keyValuePair.Value.ImagePath, keyValuePair.Key);
                button.Tag = keyValuePair.Value.Path;
                button.Click += OpenFolderButton_Click;
                viewBox.Child = button;
                this.ToolPanel.Children.Add(viewBox);
            }

            if (!string.IsNullOrWhiteSpace(_majorVideoContext.VideoStartButtonImagePath) && !string.IsNullOrWhiteSpace(_majorVideoContext.VideoPauseButtonImagePath))
            {
                _playButtonImage = new Image
                {
                    Source = new BitmapImage(new Uri(_majorVideoContext.VideoStartButtonImagePath))
                };
                _pauseButtonImage = new Image
                {
                    Source = new BitmapImage(new Uri(_majorVideoContext.VideoPauseButtonImagePath))
                };
                _videoControlButton = ButtonUtility.CreateButton(_playButtonImage, "Play");
            }
            else
            {
                _videoControlButton = ButtonUtility.CreateButton((Image)null, "Play", false);
            }

            _videoControlButton.ToolTip = appConfig.VideoPlayToolTip;
            _videoControlButton.Click += VideoPlayerControl_Click;
            var buttonViewBox = new Viewbox {Child = _videoControlButton};
            ButtonGrid.Children.Add(buttonViewBox);

            var fileIndex = 0;
            foreach (var oneFileName in _majorVideoContext.VideoFileNames)
            {;
                var listViewItem = new ListViewItem
                {
                    Content = System.IO.Path.GetFileNameWithoutExtension(oneFileName), Tag = fileIndex
                };
                ControlDoubleClick.SetCommand(listViewItem, listViewItemCommand);
                this.VideoFilesListView.Items.Add(listViewItem);
                fileIndex++;
            }

            VideoPlayer.MediaEnded += VideoPlayer_MediaEnded;
            VideoPlayer.MediaOpened += VideoPlayer_MediaOpened;
            if (_majorVideoContext.VideoFileNames.Count > 0)
            {
                this.VideoFilesListView.SelectedIndex = 0;
                ChangeCurrentIndex(0);
            }

            if (!string.IsNullOrWhiteSpace(appConfig.VideoPageTitle) && !string.IsNullOrWhiteSpace(majorVideoContext.MajorName))
            {
                var title = $"{majorVideoContext.MajorName}.{appConfig.VideoPageTitle}";
                Title = title;
            }

            this.Unloaded += VideosPage_Unloaded;
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
            if (_videoControlButton is ImageButton)
            {
                _videoControlButton.Content = _played ? _pauseButtonImage : _playButtonImage;
            }
            else
            {
                _videoControlButton.Content = _played ? "Pause" : "Play";
            }

            _videoControlButton.ToolTip = _played ? _appConfig.VideoPauseToolTip : _appConfig.VideoPlayToolTip;
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
            var nowPaused = !_played;
            VideoPlayer.Stop();
            _currentIndex = index;
            var files = _majorVideoContext.VideoFileNames;
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
    }
}
