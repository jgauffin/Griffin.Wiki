using System.Collections.Generic;
using Griffin.Wiki.Core.Templates.DomainModels;

namespace Griffin.Wiki.Core.Templates.Repositories
{
    public interface ITemplateRepository
    {
        PageTemplate Create(string title, string contents);
        IEnumerable<PageTemplate> Find();
        PageTemplate Get(int templateId);
    }
}