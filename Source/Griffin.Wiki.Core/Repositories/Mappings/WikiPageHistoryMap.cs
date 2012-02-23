using FluentNHibernate.Mapping;
using ProjectPortal.Core.DomainModels;

namespace ProjectPortal.Core.Repositories.Mappings
{
    public class WikiPageHistoryMap : ClassMap<WikiPageHistory>
    {
        public WikiPageHistoryMap()
        {
            Table("WikiPageHistory");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            References(x => x.Page).Column("PageId");
            References(x => x.CreatedBy).Column("CreatedBy");
            Map(x => x.CreatedAt).Column("CreatedAt").Not.Nullable();
            Map(x => x.ChangeDescription).Column("ChangeDescription").Not.Nullable().Length(1073741823);
            Map(x => x.RawBody).Column("RawBody").Not.Nullable().Length(1073741823);
            Map(x => x.HtmlBody).Column("HtmlBody").Not.Nullable().Length(1073741823);
        }
    }
}