using System;
using System.Collections.Generic;
using System.Linq;
using Griffin.Wiki.Core.Services;
using NHibernate;
using NHibernate.Linq;
using Griffin.Wiki.Core.DomainModels;
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
            if (node == null) throw new ArgumentNullException("node");

            _dbSession.SaveOrUpdate(node);
        }

        public void Save(WikiPage page)
        {
            if (page == null) throw new ArgumentNullException("page");

            _dbSession.SaveOrUpdate(page);
        }

        public WikiPage Create(int parentId, string pageName, string title, PageTemplate template)
        {
            if (pageName == null) throw new ArgumentNullException("pageName");
            if (title == null) throw new ArgumentNullException("title");

            return new WikiPage(_dbSession.Load<WikiPage>(parentId), pageName, title, template);
        }

        public IEnumerable<WikiPage> GetPagesLinkingTo(string pageName)
        {
            if (pageName == null) throw new ArgumentNullException("pageName");

            return (from e in _dbSession.Query<WikiPageLink>()
                    where e.LinkedPage.PageName == pageName
                    select e.Page).ToList();
        }

        public WikiPageTreeNode GetTreeNode(int pageId)
        {
            return _dbSession.Query<WikiPageTreeNode>().FirstOrDefault(x => x.Page.Id == pageId);
        }

        /// <summary>
        /// Fetch a collection of pages
        /// </summary>
        /// <param name="pageNames">WikiNames for the wanted pages</param>
        /// <returns>A collection of pages.</returns>
        public IEnumerable<WikiPage> GetPages(IEnumerable<string> pageNames)
        {
            return _dbSession.Query<WikiPage>().Where(x => pageNames.Contains(x.PageName)).ToList();

            /*return (from x in _dbSession.Query<WikiPage>()
                    where pageNames.Contains(x.PageName)
                    select x).ToList();*/
        }

        /// <summary>
        /// Fetch all pages that links to a missing page
        /// </summary>
        /// <param name="pageName">WikiName of the missing page</param>
        /// <returns>A collection of referring pages.</returns>
        public IEnumerable<MissingPageLink> GetMissingLinks(string pageName)
        {
            return (from x in _dbSession.Query<MissingPageLink>()
                    where x.MissingPageName == pageName
                    select x).ToList();
        }

        public void AddMissingLinks(WikiPage wikiPage, IEnumerable<string> missingPages)
        {
            foreach (var missingPage in missingPages)
            {
                _dbSession.Save(new MissingPageLink(wikiPage, missingPage));
            }
        }

        public void RemoveMissingLinks(string pageName)
        {
            foreach (var link in _dbSession.Query<MissingPageLink>().Where(x=> x.MissingPageName == pageName))
            {
                _dbSession.Delete(link);
            }
        }

        public IEnumerable<WikiPage> FindAll()
        {
            return _dbSession.QueryOver<WikiPage>().List();
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

            _dbSession.SaveOrUpdate(history);
        }

        public void Save(WikiPageLink history)
        {
            _dbSession.SaveOrUpdate(history);
        }

        public void Delete(WikiPageLink pageName)
        {
            _dbSession.Delete(pageName);
        }

        #endregion
    }
}