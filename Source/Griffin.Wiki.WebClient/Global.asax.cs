using System;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Griffin.Container;
using Griffin.Container.Mvc3;
using Griffin.MvcContrib.VirtualPathProvider;
using Griffin.Wiki.Core;
using Griffin.Wiki.Core.Infrastructure;
using Griffin.Wiki.Core.NHibernate.Repositories;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.Pages.PreProcessors;
using Griffin.Wiki.Core.Users.Repositories;
using Griffin.Wiki.Mvc3.Areas.Wiki.Controllers;
using Griffin.Wiki.Mvc3.Helpers;

namespace Griffin.Wiki.WebClient
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        private IParentContainer _container;

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Home", action = "Index", id = UrlParameter.Optional} // Parameter defaults
                );
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var identity = (IIdentity) WindowsIdentity.GetCurrent();
            if (identity == null || !identity.IsAuthenticated)
            {
                identity = HttpContext.Current.User.Identity;
            }
            if (!identity.IsAuthenticated)
                return;

            var name = identity.Name ?? "";
            if (identity is WindowsIdentity)
            {
                var pos = name.IndexOf("\\", StringComparison.Ordinal);
                if (pos != -1)
                    name = name.Remove(0, pos + 1);
            }

            var user = DependencyResolver.Current.GetService<IUserRepository>().GetOrCreate(name, name);
            Thread.CurrentPrincipal = new WikiPrinicpal(new WikiIdentity(user));
            HttpContext.Current.User = Thread.CurrentPrincipal;
        }

        protected void Application_Start()
        {
            //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            AreaRegistration.RegisterAllAreas();

            RegisterContainer();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            RegisterViews();

            ModelBinders.Binders.Add(typeof (PagePath), new PagePathModelBinder());
        }

        private void RegisterViews()
        {
            var fileProvider = new ViewFileProvider(new DiskFileLocator());
            GriffinVirtualPathProvider.Current.Add(fileProvider);
            HostingEnvironment.RegisterVirtualPathProvider(GriffinVirtualPathProvider.Current);
        }

        private void RegisterContainer()
        {
            var registrar = new ContainerRegistrar();
            //registrar.RegisterControllers(Assembly.GetExecutingAssembly());
            RegisterControllers(registrar, Assembly.GetExecutingAssembly());
            RegisterControllers(registrar, typeof(PageController).Assembly);

            registrar.RegisterModules(typeof (UserRepository).Assembly);
            registrar.RegisterModules(typeof (IUriHelper).Assembly);
            registrar.RegisterModules(typeof (PageController).Assembly);

            registrar.RegisterComponents(Lifetime.Scoped, Assembly.GetExecutingAssembly());
            registrar.RegisterInstance<IExternalViewFixer>(new ExternalViewFixer { LayoutPath = null });
            registrar.RegisterService<IServiceLocator>(x=> x, Lifetime.Scoped);
            registrar.RegisterConcrete<MarkdownParser>();
            registrar.RegisterConcrete<WikiUriHelper>();
            _container = registrar.Build();
            DependencyResolver.SetResolver(new GriffinDependencyResolver(_container));

            var startables = DependencyResolver.Current.GetServices<ISingletonStartable>();
            foreach (var startable in startables)
            {
                startable.StartScoped();
            }
        }

        private static void RegisterControllers(ContainerRegistrar registrar, Assembly assembly)
        {
            var controllerType = typeof (IController);
            foreach (var type in assembly.GetTypes().Where(controllerType.IsAssignableFrom))
            {
                // no public constructors.
                if (type.GetConstructors().Length == 0)
                    continue;

                registrar.RegisterType(type, type, Lifetime.Scoped);
            }
        }
    }
}