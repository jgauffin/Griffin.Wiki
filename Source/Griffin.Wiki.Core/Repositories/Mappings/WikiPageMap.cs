using FluentNHibernate;
using FluentNHibernate.Mapping;
using Griffin.Wiki.Core.DomainModels;

namespace Griffin.Wiki.Core.Repositories.Mappings
{
    public class WikiPageMap : ClassMap<WikiPage>
    {
        public WikiPageMap()
        {
            Table("WikiPages");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            References(x => x.CreatedBy).Column("CreatedBy");
            References(x => x.UpdatedBy).Column("UpdatedBy");
            Map(x => x.PageName).Column("PageName").Not.Nullable().Length(50);
            Map(x => x.Title).Column("Title").Not.Nullable().Length(50);
            Map(x => x.CreatedAt).Column("CreatedAt").Not.Nullable();
            Map(x => x.UpdatedAt).Column("UpdatedAt").Not.Nullable();
            Map(x => x.HtmlBody).Column("HtmlBody").Not.Nullable().Length(1073741823);
            Map(x => x.RawBody).Column("RawBody").Not.Nullable().Length(1073741823);

            HasMany<WikiPageHistory>(Reveal.Member<WikiPage>("HistoryInternal")).KeyColumn("PageId");
            HasMany<WikiPage>(Reveal.Member<WikiPage>("ReferencesInternal")).KeyColumn("LinkedPage");
            HasMany<WikiPage>(Reveal.Member<WikiPage>("BackReferencesInternal")).KeyColumn("SourcePage");
        }
    }
}