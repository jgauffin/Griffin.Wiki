﻿using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Griffin.Wiki.Core.Services;
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

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Page", action = "Show", id = "Home"} // Parameter defaults
                );
        }

        protected void Application_Start()
        {
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
            ServiceResolver.Assign(new AutofacServiceResolver(_container));
        }
    }
}