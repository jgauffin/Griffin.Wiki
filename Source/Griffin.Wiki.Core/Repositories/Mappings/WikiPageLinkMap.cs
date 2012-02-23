using FluentNHibernate.Mapping;
using Griffin.Wiki.Core.DomainModels;

namespace Griffin.Wiki.Core.Repositories.Mappings
{
    public class WikiPageLinkMap : ClassMap<WikiPageLink>
    {
        public WikiPageLinkMap()
        {
            Table("WikiPageLinks");
            LazyLoad();
            CompositeId();
            References(x => x.Page).Column("Page");
            References(x => x.LinkedPage).Column("LinkedPage");
        }
    }
}