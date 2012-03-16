using System.Web.Mvc;

namespace Griffin.Wiki.WebClient.Areas.Wiki
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
            context.MapRoute(
                "Wiki_default",
                "Wiki/{controller}/{action}/{id}",
                new { id = UrlParameter.Optional }
            );
            
            context.Routes.MapWikiRoute(
               "Wiki",
               new { controller = "Page", action = "Show", id = UrlParameter.Optional}
               );
        }
    }
}
