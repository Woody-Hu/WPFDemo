using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPFDemo
{
    public static class HomePageUtility
    {
        private static Func<Page> _homePageFunc;

        public static Func<Page> HomePageFunc
        {
            get => _homePageFunc;
            set
            {
                if (_homePageFunc != null)
                {
                    return;
                }
                else
                {
                    _homePageFunc = value;
                }
            }
        }
    }
}
