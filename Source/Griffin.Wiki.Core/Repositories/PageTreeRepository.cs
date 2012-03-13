using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Griffin.Wiki.Core.DomainModels;
using NHibernate;
using NHibernate.Linq;
using Sogeti.Pattern.InversionOfControl;

namespace Griffin.Wiki.Core.Repositories
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
    }
}
