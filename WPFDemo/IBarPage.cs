using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WPFDemo
{
    public interface IBarPage
    {
        Window GetWindow();

        Viewbox GetHomepageViewBox();

        Viewbox GetMinimumViewBox();

        Viewbox GetCloseViewBox();

        Grid GetBarGrid();

        Viewbox GetTitleViewBox();

        NavigationService GetNavigationService();

        void SetBarButtons(IList<ImageButton> imageButtons);
    }
}
