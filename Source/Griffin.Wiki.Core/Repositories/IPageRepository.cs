using ProjectPortal.Core.DomainModels;
using ProjectPortal.Core.Repositories.Mappings;

namespace ProjectPortal.Core.Repositories
{
    public interface IPageRepository
    {
        bool Exists(string pageName);
        WikiPage Get(string pageName);
    }
}