using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFDemo
{
    public class MajorsContext
    {

        public IDictionary<string, FolderInfo> MajorInfos { get; set; } = new Dictionary<string, FolderInfo>();

        public IDictionary<string, FolderInfo> ToolInfos { get; set; } = new Dictionary<string, FolderInfo>();

        public string AppTitle { get; set; }

        public string AppTitleImagePath { get; set; }

        public string TopBackgroundImagePath { get; set; }

        public string BottomBackgroundImagePath { get; set; }

        public static MajorContext PrepareMajorContext(MajorsContext majorsContext, AppConfig appConfig, KeyValuePair<string, FolderInfo> oneInfoPair)
        {
            var majorName = oneInfoPair.Key;
            var currentPath = System.Environment.CurrentDirectory;
            var majorContext = new MajorContext();
            majorContext.MajorName = majorName;
            majorContext.AppTitle = majorsContext.AppTitle;
            majorContext.AppTitleImagePath = majorsContext.AppTitleImagePath;
            var pluginPath = System.IO.Path.Combine(currentPath, appConfig.PluginFolderName);

            var majroTopImagePath = majorName + AppConfig.MajorTopBackgroundImageName;
            var majroBottomPath = majorName + AppConfig.MajorBottomBackgroundImageName;

            majorContext.TopBackgroundImagePath = majroTopImagePath.GetExistPath();
            majorContext.BottomBackgroundImagePath = majroBottomPath.GetExistPath();
            foreach (var oneInfo in majorsContext.ToolInfos)
            {
                var majorPath = System.IO.Path.Combine(currentPath, majorName);
                if (!Directory.Exists(majorPath))
                {
                    Directory.CreateDirectory(majorPath);
                }

                var majorToolImagePath = AppConfig.GetMajorResourceFolderImageName(majorName, oneInfo.Key);
                majorToolImagePath = majorToolImagePath.GetExistPath();
                majorToolImagePath = string.IsNullOrWhiteSpace(majorToolImagePath) ? oneInfo.Value.ImagePath : majorToolImagePath;
                majorContext.ToolInfos.Add(oneInfo.Key, new FolderInfo() { Name = oneInfo.Key, Path = majorPath, ImagePath = majorToolImagePath });
            }

            var videoFunctionDisplayName = string.IsNullOrWhiteSpace(appConfig.VideoFunctionDisplayName) ? "Video" : appConfig.VideoFunctionDisplayName;
            var videoPath = System.IO.Path.Combine(currentPath, appConfig.VideoFolderName);
            var majorVideoPath = System.IO.Path.Combine(videoPath, majorName);
            if (!Directory.Exists(majorVideoPath))
            {
                Directory.CreateDirectory(majorVideoPath);
            }

            var videoImagePath = System.IO.Path.Combine(pluginPath, majorName + AppConfig.VideoFolderImageSuffixName);
            videoImagePath = videoImagePath.GetExistPath();
            majorContext.FunctionInfos.Add(videoFunctionDisplayName, new FunctionInfo() { Kind = FunctionKind.Video, VideosPath = videoPath, ImagePath = videoImagePath });

            var openFileFunctionFile = System.IO.Path.Combine(pluginPath, majorName + AppConfig.OpenFileFunctionSuffixName);
            if (File.Exists(openFileFunctionFile))
            {
                var jsonStr = File.ReadAllText(openFileFunctionFile);
                var listOpenFileRequest = new List<OpenFileFunctionRequest>();
                try
                {
                    listOpenFileRequest = JsonConvert.DeserializeObject<IEnumerable<OpenFileFunctionRequest>>(jsonStr).ToList();
                }
                catch
                {
                    ;
                }

                foreach (var oneRequest in listOpenFileRequest)
                {
                    if (majorContext.FunctionInfos.ContainsKey(oneRequest.Name))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(oneRequest.ProgramName) || string.IsNullOrWhiteSpace(oneRequest.FileFullName) || !File.Exists(oneRequest.FileFullName))
                    {
                        continue;
                    }

                    var functionImagePath = AppConfig.GetMajorOpenFileFunctionImageName(majorName, oneRequest.Name);
                    functionImagePath = functionImagePath.GetExistPath();
                    majorContext.FunctionInfos.Add(oneRequest.Name, new FunctionInfo() { Kind = FunctionKind.OpenFile, ProgramName = oneRequest.ProgramName, FilePath = oneRequest.FileFullName, ImagePath = functionImagePath });
                }
            }

            return majorContext;
        }
    }
}
