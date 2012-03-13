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
            if (page == null) throw new ArgumentNullException("page");
            if (parentNode == null)
            {
                Ids = string.Format("/{0}/", page.Id);
                Titles = page.Title;
                Names = string.Format("/{0}/", page.PageName);
            }
            else
            {
                Ids = string.Format("{0}{1}/", parentNode.Ids, page.Id);
                Titles = string.Format("{0}{{#}}{1}", parentNode.Page.Title, page.Title);
                Names = string.Format("{0}{1}/", parentNode.Names, page.PageName);
            }

            Page = page;
        }

        public virtual int PageId { get; protected set; }
        private WikiPage _page;
        public virtual WikiPage Page
        {
            get { return _page; }
            protected set { _page = value;
                PageId = value.Id;
            }
        }

        public virtual string Ids { get; protected set; }
        protected virtual string Titles { get;  set; }
        protected virtual string Names { get; set; }

        public virtual string MakePath(string pageUri)
        {
            if (pageUri == null) throw new ArgumentNullException("pageUri");
            var titles = Titles.Split(new[] {"{#}"}, StringSplitOptions.RemoveEmptyEntries);
            var names = Names.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            var result = "";
            for (int i = 0; i < titles.Length; i++)
            {
                result += string.Format(@"<a href=""{0}{1}"">{2}</a> / ", pageUri, names[i], titles[i]);
            }

            return result == "" ? result : result.Remove(result.Length - 3, 3);
        }
    }
}
