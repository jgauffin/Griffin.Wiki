using Griffin.Wiki.Core.DomainModels;

namespace Griffin.Wiki.Core.Repositories
{
    public interface IPageRepository
    {
        bool Exists(string pageName);
        WikiPage Get(string pageName);
    }
}