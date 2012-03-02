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
            Id(x => x.Page).Column("PageId");
            References(x => x.Page).Column("PageId");
            Map(x => x.Path).Column("Title").Not.Nullable().Length(1000);
            Map(x => x.Lineage).Column("Lineage").Not.Nullable().Length(1000);
        }
    }
}