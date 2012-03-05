using FluentNHibernate;
using FluentNHibernate.Mapping;
using Griffin.Wiki.Core.DomainModels;

namespace Griffin.Wiki.Core.Repositories.Mappings
{
    public class WikiPageTreeMap : ClassMap<WikiPageTreeNode>
    {
        public WikiPageTreeMap()
        {
            Table("WikiPageTree");
            LazyLoad();

            Id(Reveal.Member<WikiPageTreeNode>("PageId")).GeneratedBy.Foreign("Page");

            HasOne(x=>x.Page).Constrained().ForeignKey();

            //Id(x => x.Page).Column("PageId");
            //References(x => x.Page).Column("Id");
            Map(x => x.Path).Column("Title").Not.Nullable().Length(1000);
            Map(x => x.Lineage).Column("Lineage").Not.Nullable().Length(1000);
        }
    }
}