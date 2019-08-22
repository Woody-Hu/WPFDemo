using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFDemo
{
    public static class StringUtility
    {
        public static string GetExistPath(this string path)
        {
            return System.IO.File.Exists(path) ? path : string.Empty;
        }
    }
}
