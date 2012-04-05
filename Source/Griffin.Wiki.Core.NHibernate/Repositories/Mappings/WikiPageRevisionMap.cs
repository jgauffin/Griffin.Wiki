using FluentNHibernate.Mapping;
using Griffin.Wiki.Core.Pages.DomainModels;

namespace Griffin.Wiki.Core.NHibernate.Repositories.Mappings
{
    public class WikiPageRevisionMap : ClassMap<WikiPageRevision>
    {
        public WikiPageRevisionMap()
        {
            Table("WikiPageRevisions");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            References(x => x.Page).Column("PageId");
            References(x => x.CreatedBy).Column("CreatedBy");
            References(x => x.ReviewedBy).Column("ReviewedBy");
            Map(x => x.IsApproved).Column("Approved");
            Map(x => x.ReviewedAt);
            Map(x => x.ReviewRequired);
            Map(x => x.Reason);
            //Map(x => x.CreatedBy).Column("CreatedBy");
            Map(x => x.CreatedAt).Column("CreatedAt").Not.Nullable();
            Map(x => x.ChangeDescription).Column("ChangeDescription").Not.Nullable().Length(1073741823);
            Map(x => x.RawBody).Column("RawBody").Not.Nullable().Length(1073741823);
            Map(x => x.HtmlBody).Column("HtmlBody").Not.Nullable().Length(1073741823);
        }
    }
}