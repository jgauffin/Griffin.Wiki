using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Griffin.Wiki.Core.DomainModels
{
    public class WikiPageTreeNode
    {
        public WikiPageTreeNode()
        {
            
        }

        public WikiPageTreeNode(WikiPage page, WikiPageTreeNode parentNode)
        {
            if (parentNode == null)
            {
                Lineage = "/" + page.PageName + "/";
                Path = "/" + page.Title + "/";
            }
            else
            {
                Lineage = parentNode.Lineage + page.Id + "/";
                Path = parentNode.Page + page.Title + "/";
            }
        }

        protected int PageId { get; set; }
        public virtual WikiPage Page { get; protected set; }
        public virtual string Lineage { get; protected set; }
        public virtual string Path { get; protected set; }
    }
}
