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
        private readonly int _countPerRow = 4;
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

            var backgroundImagePath = appConfig.GetAPPBackgroundImagePath();
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
