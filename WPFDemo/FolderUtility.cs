using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFDemo
{
    internal static class FolderUtility
    {
        internal static void ExploreFolder(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        internal static void OpenFile(string programName, string path)
        {
            if (!string.IsNullOrWhiteSpace(programName) && !string.IsNullOrWhiteSpace(path))
            {
                System.Diagnostics.Process.Start(programName, path);
            }
            else if (!string.IsNullOrWhiteSpace(programName))
            {
                System.Diagnostics.Process.Start(programName);
            }
            else if (!string.IsNullOrWhiteSpace(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        }

        internal static void ShowSearchResultsInExplorer(string folderPath, string searchString)
        {
            string sFilter = "search-ms:displayname=";
            sFilter += Uri.EscapeDataString("Tagged files in " + folderPath);
            sFilter += @"&crumb=" + searchString;
            sFilter += @"&crumb=location:" + Uri.EscapeDataString(folderPath);

            System.Diagnostics.Process.Start("IExplore.exe", sFilter);
        }
    }
}
