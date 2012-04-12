namespace Griffin.Wiki.Core
{
    public interface IUriHelper
    {
        string GetWikiRoot();
        string CreateLinkFromRoute(object route);
    }
}