using Microsoft.Owin;
using Owin;
using ReportDotNet.Web;

[assembly: OwinStartup(typeof (Startup))]

namespace ReportDotNet.Web
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.MapSignalR();
		}
	}
}