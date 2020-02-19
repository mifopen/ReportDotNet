using System.IO;
using Microsoft.AspNetCore.SignalR;
using ReportDotNet.Web.Controllers;

namespace ReportDotNet.Web.App
{
    public class DirectoryWatcher
    {
        private readonly IHubContext<ReportHub> hubContext;

        public DirectoryWatcher(IHubContext<ReportHub> hubContext)
        {
            this.hubContext = hubContext;
        }

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

        private void Handler(object sender,
                             FileSystemEventArgs e)
        {
            hubContext.Clients.All.SendCoreAsync("reportUpdated", new object[0]);
        }
    }
}