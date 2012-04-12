using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using Griffin.Wiki.Core;
using Griffin.Wiki.Core.Infrastructure;
using Griffin.Wiki.Core.NHibernate.Repositories;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.Pages.Content.Services;
using Griffin.Wiki.Core.Pages.PreProcessors;
using Griffin.Wiki.Core.Users.Repositories;
using Griffin.Wiki.WebClient.Areas.Wiki;
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


            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
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
            //HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
            AreaRegistration.RegisterAllAreas();

            RegisterContainer();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ModelBinders.Binders.Add(typeof(PagePath), new PagePathModelBinder());
        }

        private void RegisterContainer()
        {
            
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterAllComponents(typeof(UserRepository).Assembly);
            builder.RegisterAllComponents(typeof(IUriHelper).Assembly);
            builder.RegisterAllComponents(Assembly.GetExecutingAssembly());
            builder.RegisterAllModules(typeof(IUriHelper).Assembly);
            builder.RegisterAllModules(typeof(UserRepository).Assembly);

            builder.RegisterType<MarkdownParser>().AsImplementedInterfaces();
            builder.RegisterType<WikiUriHelper>().AsImplementedInterfaces().InstancePerLifetimeScope();
            _container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));
            ServiceResolver.Assign(new DependencyServiceResolver());

            var startables = DependencyResolver.Current.GetServices<Sogeti.Pattern.InversionOfControl.IStartable>();
            foreach (var startable in startables)
            {
                startable.StartComponent();
            }
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

    public class PagePathModelBinder : IModelBinder
    {
        /*protected override object ConvertType(CultureInfo culture, object value, Type destinationType)
        {
            // This class is only meant to convert Customer objects.
            if (destinationType != typeof(Customer))
            {
                return base.ConvertType(culture, value, destinationType);
            }

            // Get the ID that is being passed in.
            string customerIDString = value as string;

            if (customerIDString == null && value is string[])
            {
                customerIDString = ((string[])value)[0];
            }

            customerIDString = customerIDString.Replace("_", "=");

            int customerID = BitConverter.ToInt32(Convert.FromBase64String(customerIDString), 0);

            // Grab the customer from the database.
            return MyDataAccessLayer.GetCustomerByID(customerID);
        }*/


        /// <summary>
        /// Binds the model to a value by using the specified controller context and binding context.
        /// </summary>
        /// <returns>
        /// The bound value.
        /// </returns>
        /// <param name="controllerContext">The controller context.</param><param name="bindingContext">The binding context.</param>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(PagePath))
                return null;

            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName) ??
                        bindingContext.ValueProvider.GetValue("pagePath");

            if (value == null || value.RawValue == null)
                return null;


            var path =
                value.RawValue is string[]
                    ? ((string[])value.RawValue)[0]
                    : value.RawValue.ToString();

            if (path == "/")
                return new PagePath("/");

            if (!path.StartsWith("/") && !path.EndsWith("/"))
                path = string.Format("/{0}/", path);
            else if (!path.StartsWith("/"))
                path = string.Format("/{0}", path);
            else if (!path.EndsWith("/"))
                path = string.Format("{0}/", path);

            return new PagePath(path);
        }
    }
}