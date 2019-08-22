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
            AppConfig appConfig = new AppConfig();
            var configFilePath = AppConfig.GetConfigFilePath();
            Page expectPage = null;
            if (File.Exists(configFilePath))
            {
                try
                {
                    var configJson = File.ReadAllText(configFilePath);
                    appConfig = JsonConvert.DeserializeObject<AppConfig>(configJson);
                }
                catch
                {
                    appConfig = new AppConfig();
                }
            }

            if (appConfig.MajorNames.Count <= 0)
            {
                expectPage = new MajorsPage(new MajorsContext(), appConfig);
            }
            else if (appConfig.MajorNames.Count == 1)
            {
                MajorsContext majorsContext = PrepareMajorsContext(appConfig);
                var majorContext = MajorsContext.PrepareMajorContext(majorsContext, appConfig, majorsContext.MajorInfos.First());
                expectPage = new MajorPage(majorContext, appConfig);
            }
            else
            {
                MajorsContext majorsContext = PrepareMajorsContext(appConfig);

                expectPage = new MajorsPage(majorsContext, appConfig);
            }

            this.NavigationService.Navigate(expectPage);
        }

        private static MajorsContext PrepareMajorsContext(AppConfig appConfig)
        {
            var majorsContext = new MajorsContext();
            majorsContext.AppTitle = appConfig.AppTitle;
            majorsContext.AppTitleImagePath = AppConfig.GetAppTitleImagePath();
            majorsContext.TopBackgroundImagePath = AppConfig.GetAppTopBackgroundImagePath();
            majorsContext.BottomBackgroundImagePath = AppConfig.GetAppBottomBackgroundImagePath();

            foreach (var oneMajorName in appConfig.MajorNames)
            {
                if (majorsContext.MajorInfos.ContainsKey(oneMajorName))
                {
                    continue;
                }

                majorsContext.MajorInfos.Add(oneMajorName, new FolderInfo() { Name = oneMajorName, ImagePath = AppConfig.GetMajorFolderImagePath(oneMajorName) });
            }

            foreach (var oneResourceFolderName in appConfig.ResourceFolderNames)
            {
                if (majorsContext.ToolInfos.ContainsKey(oneResourceFolderName))
                {
                    continue;
                }

                var resourcePath = AppConfig.GetResourceFolderPath(oneResourceFolderName);
                if (string.IsNullOrWhiteSpace(resourcePath))
                {
                    continue;
                }

                majorsContext.ToolInfos.Add(oneResourceFolderName, new FolderInfo() { Name = oneResourceFolderName, ImagePath = AppConfig.GetResourceFolderImagePath(oneResourceFolderName), Path = resourcePath });
            }

            return majorsContext;
        }
    }
}
