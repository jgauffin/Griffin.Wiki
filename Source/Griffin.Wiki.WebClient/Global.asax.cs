using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.WebPages;
using Autofac;
using Autofac.Integration.Mvc;
using Griffin.MvcContrib.VirtualPathProvider;
using Griffin.Wiki.Core;
using Griffin.Wiki.Core.Infrastructure;
using Griffin.Wiki.Core.NHibernate.Repositories;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.Pages.PreProcessors;
using Griffin.Wiki.Core.Users.Repositories;
using Griffin.Wiki.Mvc3.Areas.Wiki.Controllers;
using Griffin.Wiki.Mvc3.Helpers;
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
            RegisterViews();

            ModelBinders.Binders.Add(typeof(PagePath), new PagePathModelBinder());
        }

        private void RegisterViews()
        {
            var fileProvider = new ViewFileProvider(new DiskFileLocator(VirtualPathUtility.ToAbsolute("~")));
            GriffinVirtualPathProvider.Current.Add(fileProvider);
            HostingEnvironment.RegisterVirtualPathProvider(GriffinVirtualPathProvider.Current);
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
            builder.RegisterAllComponents(typeof(PageController).Assembly);
            builder.RegisterControllers(typeof(PageController).Assembly);

            builder.RegisterInstance(new MyViewFixer { LayoutPath = null }).AsImplementedInterfaces();

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

    public class MyViewFixer : IEmbeddedViewFixer
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="EmbeddedViewFixer" /> class.
        /// </summary>
        public MyViewFixer()
        {
            WebViewPageClassName = "Griffin.MvcContrib.GriffinWebViewPage";
            LayoutPath = "~/Views/Shared/_Layout.cshtml";
        }

        /// <summary>
        ///   Base view class to inherit.
        /// </summary>
        /// <example>
        ///   <code>GriffinVirtualPathProvider.Current.LayoutPath = "Griffin.MvcContrib.GriffinWebViewPage";</code>
        /// </example>
        /// <value> Default is Griffin.MvcContrib.GriffinWebViewPage </value>
        public string WebViewPageClassName { get; set; }

        /// <summary>
        ///   Gets or sets relative path to the layout file to use
        /// </summary>
        /// <example>
        ///   <code>GriffinVirtualPathProvider.Current.LayoutPath = "~/Views/Shared/_Layout.cshtml";</code>
        /// </example>
        /// <value> Default is "~/Views/Shared/_Layout.cshtml" </value>
        public string LayoutPath { get; set; }

        /// <summary>
        ///   Modify the view
        /// </summary>
        /// <param name="virtualPath"> Path to view </param>
        /// <param name="stream"> Stream containing the original view </param>
        /// <returns> Stream with modified contents </returns>
        public Stream CorrectView(string virtualPath, Stream stream)
        {
            var reader = new StreamReader(stream, Encoding.UTF8);
            var view = reader.ReadToEnd();
            stream.Close();
            var ourStream = new MemoryStream();
            var writer = new StreamWriter(ourStream, Encoding.UTF8);

            var modelString = "";
            var modelPos = view.IndexOf("@model");
            if (modelPos != -1)
            {
                writer.Write(view.Substring(0, modelPos));
                var modelEndPos = view.IndexOfAny(new[] {'\r', '\n'}, modelPos);
                modelString = view.Substring(modelPos, modelEndPos - modelPos);
                view = view.Remove(0, modelEndPos);
            }

            writer.WriteLine("@using System.Web.Mvc");
            writer.WriteLine("@using System.Web.Mvc.Ajax");
            writer.WriteLine("@using System.Web.Mvc.Html");
            writer.WriteLine("@using System.Web.Routing");

            var basePrefix = "@inherits " + WebViewPageClassName;

            if (virtualPath.ToLower().Contains("_viewstart"))
                writer.WriteLine("@inherits System.Web.WebPages.StartPage");
            else if (modelString == "@model object")
                writer.WriteLine(basePrefix + "<dynamic>");
            else if (!string.IsNullOrEmpty(modelString))
                writer.WriteLine(basePrefix + "<" + modelString.Substring(7) + ">");
            else
                writer.WriteLine(basePrefix);

            // partial views should not have a layout
            if (!string.IsNullOrEmpty(LayoutPath) && !virtualPath.Contains("/_"))
            {
                writer.WriteLine(string.Format("@{{ Layout = \"{0}\"; }}", LayoutPath));
            }
            writer.Write(view);
            writer.Flush();
            ourStream.Position = 0;
            return ourStream;
        }

    }
    public class DiskFileLocator : IViewFileLocator
    {
        private readonly string _rootUri;
        private const string WikiAreaUriPart = "/areas/wiki";

        public DiskFileLocator(string rootUri)
        {
            _rootUri = rootUri;
        }

        #region IViewFileLocator Members

        /// <summary>
        ///   Get full path to a file
        /// </summary>
        /// <param name="uri"> Requested uri </param>
        /// <returns> Full disk path if found; otherwise null. </returns>
        public string GetFullPath(string uri)
        {
            if (uri.StartsWith(_rootUri, StringComparison.OrdinalIgnoreCase))
                uri = uri.Remove(0, _rootUri.Length);
            if (uri.StartsWith("~") && uri.Contains("ViewStart"))
                uri = uri.Remove(0, 1);
            if (uri.StartsWith(WikiAreaUriPart, StringComparison.OrdinalIgnoreCase))
                uri = uri.Remove(0, WikiAreaUriPart.Length);
            else
                return null;

            var parts = uri.TrimStart('/').Split('/');
            if (!parts[0].Equals("views", StringComparison.OrdinalIgnoreCase))
                return null;

            var path = string.Format(@"{0}\..\Griffin.Wiki.Mvc3\Areas\Wiki\{1}",
                                            HostingEnvironment.MapPath("~"),
                                            uri);
            path = Path.GetFullPath(path);
            return File.Exists(path) ? path : null;
        }

        #endregion
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