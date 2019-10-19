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

        public IDictionary<string, FolderInfo> ResourceInfos { get; set; } = new Dictionary<string, FolderInfo>();

        public string AppTitle { get; set; }

        public string AppTitleImagePath { get; set; }

        public string TopBackgroundImagePath { get; set; }

        public string BottomBackgroundImagePath { get; set; }

        public static MajorContext PrepareMajorContext(MajorsContext majorsContext, AppConfig appConfig, KeyValuePair<string, FolderInfo> oneInfoPair)
        {
            var majorName = oneInfoPair.Key;
            var majorContext = new MajorContext
            {
                MajorName = majorName,
                AppTitle = majorsContext.AppTitle,
                AppTitleImagePath = majorsContext.AppTitleImagePath
            };

            var majorTopImagePath = appConfig.GetMajorTopBackgroundImagePath(majorName);
            var majorBottomPath = appConfig.GetMajorBottomBackgroundImagePath(majorName);
            majorContext.TopBackgroundImagePath = majorTopImagePath.GetExistPath();
            majorContext.BottomBackgroundImagePath = majorBottomPath.GetExistPath();
            foreach (var oneInfo in majorsContext.ResourceInfos)
            {
                var majorPath = Path.Combine(oneInfo.Value.Path, majorName);
                if (!Directory.Exists(majorPath))
                {
                    majorPath = oneInfo.Value.Path;
                }

                var majorToolImagePath = appConfig.GetMajorResourceImagePath(majorName, oneInfo.Key);
                majorToolImagePath = majorToolImagePath.GetExistPath();
                majorToolImagePath = string.IsNullOrWhiteSpace(majorToolImagePath) ? oneInfo.Value.ImagePath : majorToolImagePath;
                var majorToolImageMouseEnterPath = AppConfig.GetMouseEnterImagePath(majorToolImagePath);
                majorContext.ResourceInfos.Add(oneInfo.Key, new FolderInfo() { Name = oneInfo.Key, Path = majorPath, ImagePath = majorToolImagePath, MoveEnterImagePath = majorToolImageMouseEnterPath });
            }

            var videoFunctionDisplayName = string.IsNullOrWhiteSpace(appConfig.VideoFunctionDisplayName) ? "Video" : appConfig.VideoFunctionDisplayName;
            var majorVideoPath = appConfig.GetMajorVideoFolderPath(majorName);
            var videoImagePath = appConfig.GetMajorVideoImagePath(majorName).GetExistPath();
            var mouseEnterImagePath = AppConfig.GetMouseEnterImagePath(videoImagePath);
            majorContext.FunctionInfos.Add(videoFunctionDisplayName, new FunctionInfo() { Kind = FunctionKind.Video, VideosPath = majorVideoPath, ImagePath = videoImagePath, MouseEnterImagePath = mouseEnterImagePath });
            var openFileFunctionFile = appConfig.GetMajorOpeFileFunctionFilePath(majorName);

            if (!File.Exists(openFileFunctionFile)) return majorContext;
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

                if (string.IsNullOrWhiteSpace(oneRequest.ProgramName) && string.IsNullOrWhiteSpace(oneRequest.FileFullName))
                {
                    continue;
                }

                var functionImagePath = appConfig.GetMajorOpenFileFunctionImagePath(majorName, oneRequest.Name);
                functionImagePath = functionImagePath.GetExistPath();
                var mouseImagePath = AppConfig.GetMouseEnterImagePath(functionImagePath);
                majorContext.FunctionInfos.Add(oneRequest.Name, new FunctionInfo() { Kind = FunctionKind.OpenFile, FunctionName = oneRequest.Name, ProgramName = oneRequest.ProgramName, FilePath = oneRequest.FileFullName, ImagePath = functionImagePath, MouseEnterImagePath = mouseImagePath, WorkDirection = oneRequest.WorkDirection });
            }

            return majorContext;
        }
    }
}
