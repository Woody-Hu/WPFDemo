using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFDemo
{
    public class FunctionInfo
    {
        public FunctionKind Kind { get; set; }

        public string FunctionName { get; set; }

        public string VideosPath { get; set; }

        public string ProgramName { get; set; }

        public string FilePath { get; set; }

        public string ImagePath { get; set; }

        public string MouseEnterImagePath { get; set; }

        public string GetFileFullPath()
        {
            var currentPath = Environment.CurrentDirectory;
            var combinedPath = Path.Combine(currentPath, FilePath);
            if (File.Exists(combinedPath))
            {
                return combinedPath;
            }
            else
            {
                return FilePath;
            }
        }
    }
}
