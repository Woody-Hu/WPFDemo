using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace WPFDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var appConfig = new AppConfig();
            var configFilePath = AppConfig.GetConfigFilePath();
            Page expectPage = null;
            if (File.Exists(configFilePath))
            {
                try
                {
                    appConfig = PrepareAppConfig(configFilePath);
                }
                catch
                {
                    appConfig = new AppConfig();
                }
            }


            var majorNames = appConfig.GetMajorNames();
            if (majorNames.Count <= 0)
            {
                expectPage = new MajorsPage(new MajorsContext(), appConfig);
            }
            else if (majorNames.Count == 1)
            {
                var majorsContext = PrepareMajorsContext(appConfig);
                var majorContext = MajorsContext.PrepareMajorContext(majorsContext, appConfig, majorsContext.MajorInfos.First());
                expectPage = new MajorPage(majorContext, appConfig);
            }
            else
            {
                var majorsContext = PrepareMajorsContext(appConfig);

                expectPage = new MajorsPage(majorsContext, appConfig);
            }

            var icoPath = appConfig.GetIcoImagePath().GetExistPath();
            if (!string.IsNullOrWhiteSpace(icoPath))
            {
                Icon = new BitmapImage(new Uri(icoPath));
            }

            if (!string.IsNullOrWhiteSpace(appConfig.AppTitle))
            {
                this.Title = appConfig.AppTitle;
            }

            this.NavigationService.Navigate(expectPage);
        }

        private static AppConfig PrepareAppConfig(string configFilePath)
        {
            var configJson = File.ReadAllText(configFilePath);
            var appConfig = JsonConvert.DeserializeObject<AppConfig>(configJson);
            if (string.IsNullOrWhiteSpace(appConfig.VideoFolderName))
            {
                appConfig.VideoFolderName = "Videos";
            }

            CreateFolderIfNotExist(appConfig.GetVideoFolderPath());
            CreateFolderIfNotExist(appConfig.GetPluginFolderPath());
            var currentPath = Environment.CurrentDirectory;
            var allDirectories = Directory.GetDirectories(currentPath);
            var resourcesDirectories = new List<string>();
            var videoFolderInfo = new DirectoryInfo(appConfig.GetVideoFolderPath());
            foreach (var oneDirectory in videoFolderInfo.GetDirectories())
            {
                appConfig.AddMajorName(oneDirectory.Name);
            }

            foreach (var oneDirectory in allDirectories)
            {
                var directoryInfo = new DirectoryInfo(oneDirectory);
                if (directoryInfo.Name == appConfig.VideoFolderName || directoryInfo.Name == appConfig.PluginFolderName)
                {
                    continue;
                }

                resourcesDirectories.Add(oneDirectory);
                appConfig.AddResourceName(directoryInfo.Name);

                foreach (var majorDirectory in directoryInfo.GetDirectories())
                {
                    appConfig.AddMajorName(majorDirectory.Name);
                }
            }

            var allMajors = appConfig.GetMajorNames();
            foreach (var oneResourcePath in resourcesDirectories)
            {
                foreach (var oneMajorName in allMajors)
                {
                    var majorPath = Path.Combine(oneResourcePath, oneMajorName);
                    CreateFolderIfNotExist(majorPath);
                }
            }

            return appConfig;
        }

        private static string CreateFolderIfNotExist(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return path;
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        private static MajorsContext PrepareMajorsContext(AppConfig appConfig)
        {
            var majorsContext = new MajorsContext
            {
                AppTitle = appConfig.AppTitle,
                AppTitleImagePath = appConfig.GetAppTitleImagePath(),
                TopBackgroundImagePath = appConfig.GetAppTopBackgroundImagePath(),
                BottomBackgroundImagePath = appConfig.GetAppBottomBackgroundImagePath()
            };

            
            foreach (var oneMajorName in appConfig.GetMajorNames())
            {
                if (majorsContext.MajorInfos.ContainsKey(oneMajorName))
                {
                    continue;
                }

                majorsContext.MajorInfos.Add(oneMajorName, new FolderInfo() { Name = oneMajorName, ImagePath = appConfig.GetMajorFolderImagePath(oneMajorName) });
            }

            foreach (var oneResourceFolderName in appConfig.GetResourceNames())
            {
                if (majorsContext.ResourceInfos.ContainsKey(oneResourceFolderName))
                {
                    continue;
                }

                var resourcePath = AppConfig.GetResourceFolderPath(oneResourceFolderName);
                if (string.IsNullOrWhiteSpace(resourcePath))
                {
                    continue;
                }

                majorsContext.ResourceInfos.Add(oneResourceFolderName, new FolderInfo() { Name = oneResourceFolderName, ImagePath = appConfig.GetResourceFolderImagePath(oneResourceFolderName), Path = resourcePath });
            }

            return majorsContext;
        }

        private void VideoPlayButtonExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if ((sender as Window)?.Content is VideosPage page)
            {
                page.VideoPlayerControlMethod();
            }
        }
    }
}
