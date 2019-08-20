using System;
using System.Collections.Generic;
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

        public MajorPage()
        {
            InitializeComponent();
            //refresh title
            //refresh background
            //add buttons by _resourcesPaths
        }

        private IList<string> GetVideoFileNames()
        {
            throw new NotImplementedException();
        }

        private void NavigateToVideoPage()
        {
            var navigationService = this.NavigationService;
            if (navigationService == null)
            {
                throw new ArgumentException();
            }
            else
            {
                var videoFileNames = GetVideoFileNames();
                var videosPage = new VideosPage();
                navigationService.Navigate(videosPage);
            }
        }
    }
}
