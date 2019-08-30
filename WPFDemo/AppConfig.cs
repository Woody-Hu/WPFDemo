using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFDemo
{
    public class AppConfig
    {
        private const string VideoPlayImageName = "VideoPlay.png";

        private const string VideoPauseImageName = "VideoPause.png";

        private const string HomeButtonImageName = "Home.png";

        private const string MinimizeButtonImageName = "Minimize.png";

        private const string CloseButtonImageName = "Close.png";

        private const string AppTitleImageName = "App_Title.png";

        private const string TitleBarBackgroundImageName = "Title_Background.jpg";

        private const string APPBackgroundImageName = "App_Background.jpg";

        private const string MajorBackgroundImageName = "_Major_Background.jpg";

        private const string AppTopBackgroundImageName = "App_TopBackground.jpg";

        private const string AppBottomBackgroundImageName = "App_BottomBackground.jpg";

        private const string MajorTopBackgroundImageName = "_Major_TopBackground.jpg";

        private const string MajorBottomBackgroundImageName = "_Major_BottomBackground.jpg";

        private const string MajorImageSuffixName = "_Major.png";

        private const string ResourceImageSuffixName = "_Resource.png";

        private const string VideoImageSuffixName = "_Video.png";

        private const string OpenFileFunctionSuffixName = "_OpenFile.json";

        private const string OpenFileFunctionImageSuffixName = "_OpenFile.png";

        private const string MouseMoveEnterImageSuffixName = "_MouseMoveEnter";

        private const string AppConfigFileName = "appConfig.json";

        private const string IcoFileName = "app.ico";

        private readonly ISet<string> _majorNames = new HashSet<string>();

        private readonly ISet<string> _resourceFolderNames = new HashSet<string>();

        public string PluginFolderName { get; } = "Plugin";

        public string AppTitle { get; set; } = "App";

        public string VideoPageTitle { get; set; } = "Videos";

        public string VideoPlayToolTip { get; set; } = "Play";

        public string VideoPauseToolTip { get; set; } = "Pause";

        public string HomePageToolTip { get; set; } = "Homepage";

        public string MinimumToolTip { get; set; } = "Minimum";

        public string CloseToolTip { get; set; } = "Close";

        public IList<string> VideoFormats { get; set; } = new List<string>();

        public string VideoFolderName { get; set; }

        public string VideoFunctionDisplayName { get; set; }

        public IList<string> GetMajorNames()
        {
            return _majorNames.ToList();
        }

        public IList<string> GetResourceNames()
        {
            return _resourceFolderNames.ToList();
        }

        public string GetVideoPlayImagePath()
        {
            return Path.Combine(GetPluginFolderPath(), AppConfig.VideoPlayImageName);
        }

        public string GetVideoPauseImagePath()
        {
            return Path.Combine(GetPluginFolderPath(), AppConfig.VideoPauseImageName);
        }

        public string GetHomeButtonImagePath()
        {
            return Path.Combine(GetPluginFolderPath(), AppConfig.HomeButtonImageName);
        }

        public string GetMinimizeButtonImagePath()
        {
            return Path.Combine(GetPluginFolderPath(), AppConfig.MinimizeButtonImageName);
        }

        public string GetCloseButtonImagePath()
        {
            return Path.Combine(GetPluginFolderPath(), AppConfig.CloseButtonImageName);
        }

        public string GetAPPBackgroundImagePath()
        {
            return Path.Combine(GetPluginFolderPath(), AppConfig.APPBackgroundImageName).GetExistPath();
        }

        public string GetMajorBackgroundImagePath(string majorName)
        {
            return Path.Combine(GetPluginFolderPath(), $"{majorName}{MajorBackgroundImageName}").GetExistPath();
        }

        public string GetIcoImagePath()
        {
            return Path.Combine(GetPluginFolderPath(), IcoFileName);
        }

        public void AddMajorName(string majorName)
        {
            if (!string.IsNullOrWhiteSpace(majorName))
            {
                _majorNames.Add(majorName);
            }
        }

        public void AddResourceName(string resourceName)
        {
            if (!string.IsNullOrWhiteSpace(resourceName))
            {
                _resourceFolderNames.Add(resourceName);
            }
        }

        public string GetMajorResourceImagePath(string majorName, string resourceName)
        {
            return Path.Combine(GetPluginFolderPath(), $"{majorName}_{resourceName}{ResourceImageSuffixName}") ;
        }

        public string GetMajorOpenFileFunctionImagePath(string majorName, string functionName)
        {
            return Path.Combine(GetPluginFolderPath(), $"{majorName}_{functionName}{OpenFileFunctionImageSuffixName}") ;
        }

        public static string GetConfigFilePath()
        {
            var currentPath = Environment.CurrentDirectory;
            return Path.Combine(currentPath, AppConfigFileName);
        }

        public static string GetResourceFolderPath(string resourceName)
        {
            var currentPath = Environment.CurrentDirectory;
            var path = Path.Combine(currentPath, resourceName);
            return path;
        }

        public static string GetMouseEnterImagePath(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return string.Empty;
            }

            var fileInfo = new FileInfo(imagePath);
            if (!fileInfo.Exists || string.IsNullOrWhiteSpace(fileInfo.Extension))
            {
                return string.Empty;
            }

            var baseName = fileInfo.FullName.TrimEnd(fileInfo.Extension.ToCharArray());
            var name = baseName + MouseMoveEnterImageSuffixName + fileInfo.Extension;
            return name.GetExistPath();
        }

        public string GetPluginFolderPath()
        {
            var currentPath = Environment.CurrentDirectory;
            return Path.Combine(currentPath, PluginFolderName);
        }

        public string GetVideoFolderPath()
        {
            var currentPath = Environment.CurrentDirectory;
            return Path.Combine(currentPath, VideoFolderName);
        }

        public string GetMajorVideoFolderPath(string majorName)
        {
            var currentPath = Environment.CurrentDirectory;
            return Path.Combine(Path.Combine(currentPath, VideoFolderName), majorName);
        }

        public string GetAppTitleImagePath()
        {
            var path = Path.Combine(GetPluginFolderPath(), AppTitleImageName);
            return path.GetExistPath();
        }

        public string GetTitleBarBackgroundImagePath()
        {
            var path = Path.Combine(GetPluginFolderPath(), TitleBarBackgroundImageName);
            return path.GetExistPath();
        }

        public string GetAppBottomBackgroundImagePath()
        {
            var path = Path.Combine(GetPluginFolderPath(), AppBottomBackgroundImageName);
            return path.GetExistPath();
        }

        public  string GetAppTopBackgroundImagePath()
        {
            var path = Path.Combine(GetPluginFolderPath(), AppTopBackgroundImageName);
            return path.GetExistPath();
        }

        public string GetMajorBottomBackgroundImagePath(string majorName)
        {
            var path = Path.Combine(GetPluginFolderPath(), majorName + MajorBottomBackgroundImageName);
            return path.GetExistPath();
        }

        public string GetMajorTopBackgroundImagePath(string majorName)
        {
            var path = Path.Combine(GetPluginFolderPath(), majorName + MajorTopBackgroundImageName);
            return path.GetExistPath();
        }

        public string GetMajorFolderImagePath(string majorName)
        {
            var path = Path.Combine(GetPluginFolderPath(), majorName+ MajorImageSuffixName);
            return path.GetExistPath();
        }

        public string GetMajorVideoImagePath(string majorName)
        {
            var currentPath = Environment.CurrentDirectory;
            var path = Path.Combine(GetPluginFolderPath(), majorName + VideoImageSuffixName);
            return path.GetExistPath();
        }

        public string GetMajorOpeFileFunctionFilePath(string majorName)
        {
            var path = Path.Combine(GetPluginFolderPath(), majorName + OpenFileFunctionSuffixName);
            return path.GetExistPath();
        }

        public string GetResourceFolderImagePath(string resourceName)
        {
            var currentPath = Environment.CurrentDirectory;
            var path = Path.Combine(GetPluginFolderPath(), resourceName + ResourceImageSuffixName);
            return path.GetExistPath();
        }
    }
}
