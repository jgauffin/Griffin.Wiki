using System.Collections.Generic;
using System.Linq;
using Griffin.Wiki.Core.Pages.DomainModels;
using NHibernate;
using NHibernate.Linq;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.NHibernate.Repositories
{
    [Component]
    public class PageTreeRepository
    {
        private readonly ISession _session;

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

        public WikiPageTreeNode Get(int id)
        {
            return _session.Load<WikiPageTreeNode>(id);
        }

        /// <summary>
        /// Find all items
        /// </summary>
        /// <returns>Sorted by depth and titles.</returns>
        public IEnumerable<WikiPageTreeNode> FindAll()
        {
            return (from x in _session.Query<WikiPageTreeNode>().Fetch(x => x.Page)
                    orderby x.Depth , x.Titles
                    select x).ToList();
        }

        /// <summary>
        /// Find three depths (-1, current, children)
        /// </summary>
        /// <param name="pageName">Page to get map from</param>
        /// <returns>Items sorted by depths and titles</returns>
        public IEnumerable<WikiPageTreeNode> GetPartial(string pageName)
        {
            var myNode = _session.Query<WikiPageTreeNode>().First(x => x.Page.PageName == pageName);
            return (from x in _session.Query<WikiPageTreeNode>().Fetch(x => x.Page)
                    where (x.Depth == myNode.Depth - 1 && x.Lineage.StartsWith(myNode.ParentLinage))
                          || (x.Depth == myNode.Depth && x.Lineage.StartsWith(myNode.ParentLinage))
                          || (x.Depth == myNode.Depth + 1 && x.Lineage.StartsWith(myNode.Lineage))
                    orderby x.Depth , x.Titles
                    select x).ToList();
        }

        public WikiPageTreeNode GetByPath(string relativeUrl)
        {
            return _session.Query<WikiPageTreeNode>().FirstOrDefault(x => x.Names == relativeUrl);
        }

        public WikiPageTreeNode GetByName(string pageName)
        {
            return _session.Query<WikiPageTreeNode>().FirstOrDefault(x => x.Page.PageName == pageName);
        }

        public IEnumerable<WikiPageTreeNode> Find(IEnumerable<string> pageNames)
        {
            return (from x in _session.Query<WikiPageTreeNode>()
                    where pageNames.Contains(x.Page.PageName)
                    select x).ToList();
        }

        public bool Exists(string pageName)
        {
            return _session.Query<WikiPageTreeNode>().Any(x => x.Page.PageName == pageName);
        }
    }
}