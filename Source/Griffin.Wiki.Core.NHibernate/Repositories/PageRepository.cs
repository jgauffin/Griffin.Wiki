using System;
using System.Collections.Generic;
using System.Linq;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.Pages.Repositories;
using Griffin.Wiki.Core.SiteMaps.DomainModels;
using Griffin.Wiki.Core.Templates.DomainModels;
using NHibernate;
using NHibernate.Linq;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.NHibernate.Repositories
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

        public bool Exists(PagePath pagePath)
        {
            return _dbSession.Query<WikiPage>().Any(p => p.PagePath == pagePath);
        }


        public WikiPage Get(PagePath pagePath)
        {
            var page = _dbSession.Query<WikiPage>().FirstOrDefault(x => x.PagePath == pagePath);
            if (page == null)
                return null;

            //var wikiPage = new WikiPage(this, 1, pagePath, pagePath);
            //Mapper.Map(document, wikiPage);
            return page;
        }

        public void Save(WikiPage page)
        {
            if (page == null) throw new ArgumentNullException("page");

            _dbSession.SaveOrUpdate(page);
        }

        public IEnumerable<WikiPageRevision> GetRevisionsToApprove()
        {
            return _dbSession.Query<WikiPageRevision>().Where(x=>x.IsApproved == false && x.ReviewRequired).Take(30);
        }

        public WikiPage Create(int parentId, PagePath pagePath, string title, PageTemplate template)
        {
            if (pagePath == null) throw new ArgumentNullException("pagePath");
            if (title == null) throw new ArgumentNullException("title");

            return new WikiPage(_dbSession.Get<WikiPage>(parentId), pagePath, title, template);
        }

        public IEnumerable<WikiPage> GetPagesLinkingTo(PagePath pagePath)
        {
            if (pagePath == null) throw new ArgumentNullException("pagePath");

            return (from e in _dbSession.Query<WikiPageLink>()
                    where e.LinkedPage.PagePath == pagePath
                    select e.Page).ToList();
        }

        /// <summary>
        /// Fetch a collection of pages
        /// </summary>
        /// <param name="pagePaths">WikiNames for the wanted pages</param>
        /// <returns>A collection of pages.</returns>
        public IEnumerable<WikiPage> GetPages(IEnumerable<PagePath> paths)
        {
            return _dbSession.Query<WikiPage>().Where(x => paths.Contains(x.PagePath)).ToList();

            /*return (from x in _dbSession.Query<WikiPage>()
                    where pagePaths.Contains(x.pagePath)
                    select x).ToList();*/
        }

        /// <summary>
        /// Fetch all pages that links to a missing page
        /// </summary>
        /// <param name="pagePath">WikiName of the missing page</param>
        /// <returns>A collection of referring pages.</returns>
        public IEnumerable<MissingPageLink> GetMissingLinks(PagePath pagePath)
        {
            return (from x in _dbSession.Query<MissingPageLink>()
                    where x.MissingPagePath == pagePath.ToString()
                    select x).ToList();
        }

        public void AddMissingLinks(WikiPage wikiPage, IEnumerable<PagePath> missingPages)
        {
            foreach (var missingPage in missingPages)
            {
                _dbSession.Save(new MissingPageLink(wikiPage, missingPage));
            }
        }

        public void RemoveMissingLinks(PagePath pagePath)
        {
            foreach (var link in _dbSession.Query<MissingPageLink>().Where(x => x.MissingPagePath == pagePath.ToString()))
            {
                _dbSession.Delete(link);
            }
        }

        public IEnumerable<WikiPage> FindAll()
        {
            return _dbSession.QueryOver<WikiPage>().List();
        }

        public IEnumerable<WikiPage> FindTop10(string term)
        {
            return
                _dbSession.Query<WikiPage>().Where(x => x.Title.Contains(term)).Take(10).
                    ToList();
        }

        public void Delete(PagePath path)
        {
            var page = Get(path);
            if (page != null)
                _dbSession.Delete(page);
        }

        public void Save(WikiPageRevision revision)
        {
            if (revision == null) throw new ArgumentNullException("revision");

            _dbSession.Save(revision);
        }

        public void Save(WikiPageLink link)
        {
            _dbSession.SaveOrUpdate(link);
        }

        public void Delete(WikiPageLink link)
        {
            _dbSession.Delete(link);
        }

        #endregion

        public void Delete(WikiPageTreeNode node)
        {
            _dbSession.Delete(node);
        }

        public void Save(WikiPageTreeNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            _dbSession.SaveOrUpdate(node);
        }

        public WikiPageTreeNode GetTreeNode(int pageId)
        {
            return _dbSession.Query<WikiPageTreeNode>().FirstOrDefault(x => x.Page.Id == pageId);
        }
    }
}