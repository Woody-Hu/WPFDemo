using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFDemo
{
    public class MajorVideoContext
    {
        public IList<string> VideoFileNames { get; set; } = new List<string>();

        public IDictionary<string, FolderInfo> ToolInfos { get; set; } = new Dictionary<string, FolderInfo>();

        public string VideoStartButtonImagePath { get; set; }

        public string VideoPauseButtonImagePath { get; set; }
    }
}
