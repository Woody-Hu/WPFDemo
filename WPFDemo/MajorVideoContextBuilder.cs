using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFDemo
{
    internal class MajorVideoContextBuilder:MajorContextBuilder
    {
        internal IList<string> VideoFileNames { get; set; }

        internal bool StartVideoWhenActive { get; set; }
    }
}
