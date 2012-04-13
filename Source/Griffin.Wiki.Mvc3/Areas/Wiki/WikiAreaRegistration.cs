using System.Web.Mvc;

namespace Griffin.Wiki.Mvc3.Areas.Wiki
{
    public class WikiAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Wiki";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            var constraint = new RouteListConstraint(new[] { "page", "template", "sitemap", "image" });


            context.Routes.MapWikiRoute(
               "Wiki",
               new { controller = "Page", action = "Show", id = UrlParameter.Optional }
               );

            context.MapRoute(
                "WikiAdmin",
                "wiki/adm/{controller}/{action}/{*id}",
                new { action = "Index", id = UrlParameter.Optional }
                //new{controller=constraint}
            );

        }
    }
}
