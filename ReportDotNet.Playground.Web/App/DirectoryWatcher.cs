using System.IO;
using Microsoft.AspNet.SignalR;
using ReportDotNet.Web.Controllers;

namespace ReportDotNet.Web.App
{
    public class DirectoryWatcher
    {
        private FileSystemWatcher currentWatcher;
        private string currentFolder;

        public void Watch(string templateFolder)
        {
            if (currentFolder == templateFolder)
                return;

            lock (this)
            {
                if (currentFolder == templateFolder)
                    return;

                currentFolder = templateFolder;

                currentWatcher?.Dispose();
                currentWatcher = new FileSystemWatcher(templateFolder)
                                 {
                                     EnableRaisingEvents = true,
                                     IncludeSubdirectories = true
                                 };
                currentWatcher.Changed += Handler;
                currentWatcher.Renamed += Handler;
            }
        }

        private static void Handler(object sender,
                                    FileSystemEventArgs e)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ReportHub>();
            context.Clients.All.reportUpdated();
        }
    }
}