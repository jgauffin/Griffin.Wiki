using System.Linq;
using AutoMapper;
using NHibernate;
using NHibernate.Linq;
using Griffin.Wiki.Core.DomainModels;
using Griffin.Wiki.Core.Repositories.Documents;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Repositories
{
    [Component]
    public class PageRepository : IPageRepository
    {
        private readonly ISession _dbSession;

        public PageRepository(ISession dbSession)
        {
            _dbSession = dbSession;
        }

        #region IPageRepository Members

        public bool Exists(string pageName)
        {
            return _dbSession.Query<PageDocument>().Any(p => p.Name == pageName);
        }


        public WikiPage Get(string pageName)
        {
            var document = _dbSession.Query<PageDocument>().FirstOrDefault(x => x.Name == pageName);
            if (document == null)
                return null;

            var wikiPage = new WikiPage(this);
            Mapper.Map(document, wikiPage);
            return wikiPage;
        }

        #endregion
    }
}