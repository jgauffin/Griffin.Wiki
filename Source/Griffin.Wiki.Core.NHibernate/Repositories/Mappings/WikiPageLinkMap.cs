using FluentNHibernate.Mapping;
using Griffin.Wiki.Core.Pages.DomainModels;

namespace Griffin.Wiki.Core.NHibernate.Repositories.Mappings
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