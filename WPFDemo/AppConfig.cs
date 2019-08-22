using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFDemo
{
    public class AppConfig
    {
        public string AppTitle { get; set; }

        public IList<string> VideoFormates { get; set; } = new List<string>();

        public IList<string> MajorNames { get; set; } = new List<string>();

        public IList<string> ResourceFolderNames { get; set; } = new List<string>();

        public string VideoFolderName { get; set; }

        public string VideoFunctionDisplayName { get; set; }

        public string PluginFolderName { get; } = "Plugin";

        public const string VideoPlayImageName = "VideoPlay.png";

        public const string VideoPauseImageName = "VideoPause.png";

        public const string AppTitleImageName = "App_Title.png";

        public const string AppTopBackgroundImageName = "App_TopBackground.jpg";

        public const string AppBottomBackgroundImageName = "App_BottomBackground.jpg";

        public const string MajorTopBackgroundImageName = "_Major_TopBackground.jpg";

        public const string MajorBottomBackgroundImageName = "_Major_BottomBackground.jpg";

        public const string MajorFolderImageSuffixName = "_MajorFolder.png";

        public const string ResourceFolderImageSuffixName = "_ResourceFolder.png";

        public const string VideoFolderImageSuffixName = "_VideoFolder.png";

        public const string OpenFileFunctionSuffixName = "_OpenFile.json";

        public const string OpenFileFunctionImageSuffixName = "_OpenFile.png";

        public const string AppConfigFileName = "appConfig.json";

        public static string GetMajorResourceFolderImageName(string majorName, string resourceName)
        {
            return $"{majorName}_{resourceName}_{ResourceFolderImageSuffixName}";
        }

        public static string GetMajorOpenFileFunctionImageName(string majorName, string functionName)
        {
            return $"{majorName}_{functionName}_{OpenFileFunctionImageSuffixName}";
        }

        public static string GetConfigFilePath()
        {
            var currentPath = System.Environment.CurrentDirectory;
            return Path.Combine(currentPath, AppConfigFileName);
        }

        public static string GetAppTitleImagePath()
        {
            var currentPath = System.Environment.CurrentDirectory;
            var path = Path.Combine(currentPath, AppTitleImageName);
            return path.GetExistPath();
        }

        public static string GetAppBottomBackgroundImagePath()
        {
            var currentPath = System.Environment.CurrentDirectory;
            var path = Path.Combine(currentPath, AppBottomBackgroundImageName);
            return path.GetExistPath();
        }

        public static string GetAppTopBackgroundImagePath()
        {
            var currentPath = System.Environment.CurrentDirectory;
            var path = Path.Combine(currentPath, AppTopBackgroundImageName);
            return path.GetExistPath();
        }

        public static string GetMajorFolderImagePath(string majorName)
        {
            var currentPath = System.Environment.CurrentDirectory;
            var path = Path.Combine(currentPath, majorName+ MajorFolderImageSuffixName);
            return path.GetExistPath();
        }

        public static string GetResourceFolderPath(string resourceName)
        {
            var currentPath = System.Environment.CurrentDirectory;
            var path = Path.Combine(currentPath, resourceName);
            return path.GetExistPath();
        }

        public static string GetResourceFolderImagePath(string resourceName)
        {
            var currentPath = System.Environment.CurrentDirectory;
            var path = Path.Combine(currentPath, resourceName + ResourceFolderImageSuffixName);
            return path.GetExistPath();
        }
    }
}
