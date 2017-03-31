using Microsoft.Owin;
using Owin;
using ReportDotNet.WebApp;

[assembly: OwinStartup(typeof (Startup))]

namespace ReportDotNet.WebApp
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.MapSignalR();
		}
	}
}