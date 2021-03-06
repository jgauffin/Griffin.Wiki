﻿using System.Collections.Generic;
using System.Linq;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.SiteMaps.DomainModels;
using Griffin.Wiki.Core.SiteMaps.Repositories;
using NHibernate;
using NHibernate.Linq;
using Griffin.Container;

namespace Griffin.Wiki.Core.NHibernate.Repositories
{
    /// <summary>
    /// Repository for the page tree
    /// </summary>
    /// <remarks>Create a tree structure of all pages.</remarks>
    [Component]
    public class PageTreeRepository : IPageTreeRepository
    {
        private readonly ISession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageTreeRepository"/> class.
        /// </summary>
        /// <param name="session">DB session.</param>
        public PageTreeRepository(ISession session)
        {
            _session = session;
        }

        public WikiPageTreeNode Create(WikiPage page)
        {
            WikiPageTreeNode parent = null;
            if (page.Parent != null)
            {
                parent = _session.Query<WikiPageTreeNode>().FirstOrDefault(x => x.PageId == page.Parent.Id);
            }

            var node = new WikiPageTreeNode(page, parent);
            _session.Save(node);
            return node;
        }

        public void Delete(WikiPageTreeNode node)
        {
            _session.Delete(node);
        }

        public void DeleteAll()
        {
            _session.Delete("from WikiPageTreeNode e");
        }

        public void Save(WikiPageTreeNode node)
        {
            _session.Update(node);
        }

        public WikiPageTreeNode Get(int id)
        {
            return _session.Load<WikiPageTreeNode>(id);
        }

        /// <summary>
        /// Find all items
        /// </summary>
        /// <returns>Sorted by depth and titles.</returns>
        public IEnumerable<WikiPageTreeNode> FindAll(FindOption option)
        {
            if (option == FindOption.LoadPages)
                return (from x in _session.Query<WikiPageTreeNode>().Fetch(x => x.Page)
                        orderby x.Depth, x.Titles
                        select x).ToList();
            else
                return (from x in _session.Query<WikiPageTreeNode>()
                        orderby x.Depth, x.Titles
                        select x).ToList();
        }

        /// <summary>
        /// Find three depths (-1, current, children)
        /// </summary>
        /// <param name="pagePath">Page to get map from</param>
        /// <returns>Items sorted by depths and titles</returns>
        public IEnumerable<WikiPageTreeNode> GetPartial(PagePath pagePath)
        {
            var myNode = _session.Query<WikiPageTreeNode>().First(x => x.Page.PagePath == pagePath);
            return (from x in _session.Query<WikiPageTreeNode>().Fetch(x => x.Page)
                    where (x.Depth == myNode.Depth - 1 && x.Lineage.StartsWith(myNode.ParentLinage))
                          || (x.Depth == myNode.Depth && x.Lineage.StartsWith(myNode.ParentLinage))
                          || (x.Depth == myNode.Depth + 1 && x.Lineage.StartsWith(myNode.Lineage))
                    orderby x.Depth, x.Titles
                    select x).ToList();
        }

        public WikiPageTreeNode GetByPath(string wikiPath)
        {
            var path = new PagePath(wikiPath);
            return _session.Query<WikiPageTreeNode>().FirstOrDefault(x => x.Path == path);
        }

        public WikiPageTreeNode GetByPath(PagePath pageName)
        {
            return _session.Query<WikiPageTreeNode>().FirstOrDefault(x => x.Page.PagePath == pageName);
        }

        public IEnumerable<WikiPageTreeNode> Find(IEnumerable<PagePath> pageNames)
        {
            return (from x in _session.Query<WikiPageTreeNode>()
                    where pageNames.Contains(x.Page.PagePath)
                    select x).ToList();
        }

        public bool Exists(PagePath pagePath)
        {
            return _session.Query<WikiPageTreeNode>().Any(x => x.Page.PagePath == pagePath);
        }
    }
}