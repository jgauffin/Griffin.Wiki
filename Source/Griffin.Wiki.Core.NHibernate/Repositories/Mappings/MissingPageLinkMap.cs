using FluentNHibernate;
using FluentNHibernate.Mapping;
using Griffin.Wiki.Core.Pages.DomainModels;

namespace Griffin.Wiki.Core.NHibernate.Repositories.Mappings
{
    public class MissingPageLinkMap : ClassMap<MissingPageLink>
    {
        public MissingPageLinkMap()
        {
            Table("WikiMissingPageLinks");
            LazyLoad();
            Id(Reveal.Member<MissingPageLink>("Id")).GeneratedBy.Identity().Column("Id");
            References(x => x.Page).Column("PageId");
            Map(x => x.MissingPagePath).Column("MissingPageName").Not.Nullable().Length(50);
        }
    }
}