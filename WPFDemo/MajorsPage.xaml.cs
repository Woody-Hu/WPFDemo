using Newtonsoft.Json;
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
    public partial class MajorsPage : Page, IBarPage
    {
        private readonly MajorsContext _majorsContext;
        private readonly Window _window;
        private readonly AppConfig _appConfig;
        private IList<ImageButton> _imageButtons;

        public MajorsPage(MajorsContext majorsContext, AppConfig appConfig, Window window)
        {
            InitializeComponent();
            _majorsContext = majorsContext;
            _appConfig = appConfig;
            _window = window;
            var majorsGrid = this.MajorsGrid;
            PrepareGrid(majorsGrid, this._majorsContext.MajorInfos,true);
            PrepareGrid(this.ToolsGrid, this._majorsContext.ResourceInfos,false);

            var backgroundImagePath = appConfig.GetAppBackgroundImagePath();
            if (!string.IsNullOrWhiteSpace(backgroundImagePath))
            {
                GridUtility.SetBackGround(backgroundImagePath, MainGrid);
            }
            else
            {
                GridUtility.SetBackGround(_majorsContext.TopBackgroundImagePath, TopGrid);
                GridUtility.SetBackGround(_majorsContext.BottomBackgroundImagePath, BottomGrid);
            }


            if (!string.IsNullOrWhiteSpace(_majorsContext.AppTitle))
            {
                this.WindowTitle = _majorsContext.AppTitle;
                this.Title = _majorsContext.AppTitle;
            }

            BarPageUtility.PrepareBarPage(this, _appConfig);

        }

        private void NavigateMajorPageButton_Click(object sender, RoutedEventArgs e)
        {
            var majorContext = (sender as Button)?.Tag as MajorContext;
            var navigationService = this.NavigationService;
            navigationService?.Navigate(new MajorPage(majorContext, _appConfig, _window));
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var path = (sender as Button)?.Tag;
            if (path != null) FolderUtility.ExploreFolder(path.ToString());
        }

        private void PrepareGrid(Grid grid, IDictionary<string, FolderInfo> infos, bool isMajorRequest)
        {
            int columnCount, useRowIndex, useColumnIndex;
            GridUtility.PrepareBuutonGrid(grid, infos.Count, isMajorRequest, out columnCount, out useRowIndex, out useColumnIndex);

            foreach (var oneInfoPair in infos)
            {
                var button = ButtonUtility.CreateButton(oneInfoPair.Value.ImagePath, oneInfoPair.Key, oneInfoPair.Value.MoveEnterImagePath);
                if (isMajorRequest)
                {
                    MajorContext majorContext = MajorsContext.PrepareMajorContext(_majorsContext, _appConfig, oneInfoPair);
                    button.Tag = majorContext;
                    button.Click += NavigateMajorPageButton_Click;
                }
                else
                {
                    button.Tag = oneInfoPair.Value.Path;
                    button.Click += OpenFolderButton_Click;
                }

                GridUtility.SetButton(grid, button, isMajorRequest, columnCount, ref useRowIndex, ref useColumnIndex);
            }
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _window.DragMove();
            }
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
