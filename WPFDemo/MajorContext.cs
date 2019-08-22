using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFDemo
{
    public class MajorContext
    {
        public string MajorName { get; set; }

        public IDictionary<string, FolderInfo> ToolInfos { get; set; } = new Dictionary<string, FolderInfo>();

        public IDictionary<string, FunctionInfo> FunctionInfos { get; set; } = new Dictionary<string, FunctionInfo>();

        public string AppTitle { get; set; }

        public string AppTitleImagePath { get; set; }

        public string TopBackgroundImagePath { get; set; }

        public string BottomBackgroundImagePath { get; set; }

    }
}
