using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ReportDotNet.Web.App;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;

namespace ReportDotNet.Web
{
    public class MvcApplication: HttpApplication
    {
        protected void Application_Start()
        {
            ConfigureDI();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        private static void ConfigureDI()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.RegisterSingleton<WordToPdfConverter>();
            container.RegisterSingleton<ReportRenderer>();
            container.RegisterSingleton<DirectoryWatcher>();
            container.Verify();
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }
    }
}