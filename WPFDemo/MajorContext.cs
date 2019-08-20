using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFDemo
{
    internal class MajorContext
    {
        internal string MajorName { get; }

        internal IReadOnlyDictionary<string, string> ResourcePaths { get; }

        internal string VideoPath { get; }

        internal MajorContext(MajorContextBuilder builder)
        {
            MajorName = builder.MajorName;
            if (builder.ResourcePaths == null)
            {
                builder.ResourcePaths = new Dictionary<string, string>();
            }

            ResourcePaths = new ReadOnlyDictionary<string, string>(builder.ResourcePaths);
            VideoPath = builder.VideoPath;
        }
    }
}
