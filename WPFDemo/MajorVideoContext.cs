using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFDemo
{
    internal class MajorVideoContext:MajorContext
    {
        internal IReadOnlyList<string> VideoFileNames { get; }

        internal bool StartVideoWhenActive { get; }

        internal MajorVideoContext(MajorVideoContextBuilder builder) : base(builder)
        {
            if (builder.VideoFileNames == null)
            {
                builder.VideoFileNames = new List<string>();
            }

            VideoFileNames = new ReadOnlyCollection<string>(builder.VideoFileNames);
            StartVideoWhenActive = builder.StartVideoWhenActive;
        }
    }
}
