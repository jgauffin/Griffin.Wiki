using FluentNHibernate;
using FluentNHibernate.Mapping;
using Griffin.Wiki.Core.Pages.DomainModels;

namespace Griffin.Wiki.Core.NHibernate.Repositories.Mappings
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
            References(x => x.Parent).Column("ParentId");
            //Map(x => x.CreatedBy).Column("CreatedBy");
            //Map(x => x.UpdatedBy).Column("UpdatedBy");

            Map(x => x.PageName).Column("PageName").Not.Nullable().Length(50);
            Map(x => x.Title).Column("Title").Not.Nullable().Length(50);
            Map(x => x.CreatedAt).Column("CreatedAt").Not.Nullable();
            Map(x => x.UpdatedAt).Column("UpdatedAt").Not.Nullable();
            Map(x => x.HtmlBody).Column("HtmlBody").Not.Nullable().Length(1073741823);
            Map(x => x.RawBody).Column("RawBody").Not.Nullable().Length(1073741823);
            References(x => x.ChildTemplate).Column("TemplateId");


            HasMany<WikiPageHistory>(Reveal.Member<WikiPage>("_revisions")).KeyColumn("PageId").Inverse().Cascade.All().OrderBy("CreatedAt desc");
            HasManyToMany<WikiPage>(Reveal.Member<WikiPage>("_references")).Table("WikiPageLinks").ParentKeyColumn
                ("Page").ChildKeyColumn("LinkedPage").OrderBy("PageName");
            HasManyToMany<WikiPage>(Reveal.Member<WikiPage>("_backReferences")).Table("WikiPageLinks").ParentKeyColumn
                ("LinkedPage").ChildKeyColumn("Page").OrderBy("LinkedPage.Title");
            HasMany<WikiPage>(Reveal.Member<WikiPage>("_children")).KeyColumn("ParentId").Inverse().Cascade.
                AllDeleteOrphan().OrderBy("PageName");

            //HasMany<WikiPage>(Reveal.Member<WikiPage>("ReferencesInternal")).KeyColumn("LinkedPage");
            //HasMany<WikiPage>(Reveal.Member<WikiPage>("BackReferencesInternal")).KeyColumn("SourcePage");
        }
    }
}