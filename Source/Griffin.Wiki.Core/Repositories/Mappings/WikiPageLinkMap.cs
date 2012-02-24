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
            CompositeId().KeyReference(k => k.Page).KeyReference(x => x.LinkedPage);
            //References(x => x.Page).Column("Page");
            //References(x => x.LinkedPage).Column("LinkedPage");
        }
    }
}