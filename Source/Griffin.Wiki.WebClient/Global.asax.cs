using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Griffin.Wiki.Core.Infrastructure;
using Griffin.Wiki.Core.Repositories;
using Griffin.Wiki.Core.Services;
using Griffin.Wiki.WebClient.Infrastructure;
using Sogeti.Pattern.InversionOfControl;
using Sogeti.Pattern.InversionOfControl.Autofac;

namespace Griffin.Wiki.WebClient
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        private IContainer _container;

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapWikiRoute(
                "wiki",
                new {controller = "Page", action = "Show"}
                );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Page", action = "Index", id = UrlParameter.Optional} // Parameter defaults
                );

        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var identity = (IIdentity)WindowsIdentity.GetCurrent();
            if (identity == null || !identity.IsAuthenticated)
            {
                identity = HttpContext.Current.User.Identity;
            }
            if (!identity.IsAuthenticated)
                return;

            var name = identity.Name ?? "";
            if (identity is WindowsIdentity)
            {
                var pos = name.IndexOf("\\", System.StringComparison.Ordinal);
                if (pos != -1)
                    name = name.Remove(0, pos + 1);
            }

            var user = DependencyResolver.Current.GetService<IUserRepository>().GetOrCreate(name);
            Thread.CurrentPrincipal = new WikiPrinicpal(new WikiIdentity(user));
            HttpContext.Current.User = Thread.CurrentPrincipal;
        }

        protected void Application_Start()
        {
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            AreaRegistration.RegisterAllAreas();

            RegisterContainer();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        private void RegisterContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterAllComponents(typeof(ITextFormatParser).Assembly);
            builder.RegisterAllModules(typeof(ITextFormatParser).Assembly);
            builder.RegisterType<MarkdownParser>().AsImplementedInterfaces();
            builder.RegisterType<TextFormatAndWikiContentParser>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<MarkdownParser>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<WikiParser>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterInstance(new WikiParserConfiguration
                                         {
                                             RootUri = HostingEnvironment.ApplicationVirtualPath
                                         });
            _container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));
            ServiceResolver.Assign(new DependencyServiceResolver());
        }
    }

    public class DependencyServiceResolver : IServiceResolver
    {
        T IServiceResolver.Resolve<T>()
        {
            return DependencyResolver.Current.GetService<T>();
        }

        object IServiceResolver.Resolve(Type type)
        {
            return DependencyResolver.Current.GetService(type);
        }

        IEnumerable<T> IServiceResolver.ResolveAll<T>()
        {
            return DependencyResolver.Current.GetServices<T>();
        }

        IEnumerable IServiceResolver.ResolveAll(Type type)
        {
            return DependencyResolver.Current.GetServices(type);
        }
    }
}