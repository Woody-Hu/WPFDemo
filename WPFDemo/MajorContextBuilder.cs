using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFDemo
{
    internal class MajorContextBuilder
    {
        internal string MajorName { get; set; }

        internal IDictionary<string, string> ResourcePaths { get; set; }

        internal string VideoPath { get; set; }
    }
}
