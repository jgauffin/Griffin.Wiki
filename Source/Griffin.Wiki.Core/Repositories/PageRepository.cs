using System;
using System.Collections;
using System.Collections.Generic;
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
            return _dbSession.Query<WikiPage>().Any(p => p.PageName == pageName);
        }


        public void Delete(WikiPageTreeNode node)
        {
            _dbSession.Delete(node);
        }

        public WikiPage Get(string pageName)
        {
            var page = _dbSession.Query<WikiPage>().FirstOrDefault(x => x.PageName == pageName);
            if (page == null)
                return null;

            //var wikiPage = new WikiPage(this, 1, pageName, pageName);
            //Mapper.Map(document, wikiPage);
            return page;
        }

        public void Save(WikiPageTreeNode node)
        {
            _dbSession.Save(node);
        }

        public void Save(WikiPage page)
        {
            _dbSession.Save(page);
        }

        public WikiPage Create(int creator, string pageName, string title)
        {
            return new WikiPage(this, creator, pageName, title);
        }

        public IEnumerable<string> GetLinkingPages(string pageName)
        {
            return (from e in _dbSession.Query<WikiPageLink>()
                    where e.LinkedPage.PageName == pageName
                    select e.Page.PageName).ToList();
        }

        public WikiPageTreeNode GetTreeNode(int pageId)
        {
            return _dbSession.Query<WikiPageTreeNode>().FirstOrDefault(x => x.Page.Id == pageId);
        }

        public void Delete(string pageName)
        {
            var page = Get(pageName);
            if (page != null)
                _dbSession.Delete(page);
        }

        public void Save(WikiPageHistory history)
        {
            if (history == null) throw new ArgumentNullException("history");

            _dbSession.Save(history);
        }

        public void Save(WikiPageLink history)
        {
            _dbSession.Save(history);
        }

        public void Delete(WikiPageLink pageName)
        {
            _dbSession.Delete(pageName);
        }

        #endregion
    }
}