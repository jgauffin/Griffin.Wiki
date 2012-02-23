using FluentNHibernate.Mapping;
using ProjectPortal.Core.DomainModels;

namespace ProjectPortal.Core.Repositories.Mappings
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