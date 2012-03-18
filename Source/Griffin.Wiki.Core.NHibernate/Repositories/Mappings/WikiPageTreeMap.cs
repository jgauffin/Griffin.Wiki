using FluentNHibernate;
using FluentNHibernate.Mapping;
using Griffin.Wiki.Core.Pages.DomainModels;

namespace Griffin.Wiki.Core.NHibernate.Repositories.Mappings
{
    public class WikiPageTreeMap : ClassMap<WikiPageTreeNode>
    {
        public WikiPageTreeMap()
        {
            Table("WikiPageTree");
            LazyLoad();

            Id(Reveal.Member<WikiPageTreeNode>("PageId")).GeneratedBy.Assigned();
            HasOne(x => x.Page).Constrained().ForeignKey("PageId");

            //Id(x => x.Page).Column("PageId");
            //References(x => x.Page).Column("PageId");
            Map(Reveal.Member<WikiPageTreeNode>("Titles")).Column("Titles").Not.Nullable().Length(1000);
            Map(x => x.Lineage).Column("Ids").Not.Nullable().Length(1000);
            Map(x => x.Depth).Column("Depth").Not.Nullable();
            Map(Reveal.Member<WikiPageTreeNode>("Names")).Column("Names").Not.Nullable().Length(1000);
        }
    }
}