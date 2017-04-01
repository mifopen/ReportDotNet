using System.IO;
using Microsoft.AspNet.SignalR;
using ReportDotNet.Web.Controllers;

namespace ReportDotNet.Web.App
{
	public class ReportWatcher
	{
		private FileSystemWatcher currentWatcher;

		public void Watch(string filePath)
		{
			currentWatcher?.Dispose();
			var directoryName = Path.GetDirectoryName(filePath);
			var fileName = Path.GetFileName(filePath);
			currentWatcher = new FileSystemWatcher(directoryName, fileName) { EnableRaisingEvents = true };
			currentWatcher.Changed += Handler;
			currentWatcher.Renamed += Handler;
		}

		private static void Handler(object sender, FileSystemEventArgs e)
		{
			var context = GlobalHost.ConnectionManager.GetHubContext<ReportHub>();
			context.Clients.All.reportUpdated();
		}
	}
}