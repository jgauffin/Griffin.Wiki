using FluentNHibernate.Mapping;
using Griffin.Wiki.Core.DomainModels;

namespace Griffin.Wiki.Core.Repositories.Mappings
{
    public class PageTemplateMap : ClassMap<PageTemplate>
    {
        public PageTemplateMap()
        {
            Table("WikiTemplates");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            Map(x => x.CreatedBy).Column("CreatedBy");
            Map(x => x.UpdatedBy).Column("UpdatedBy");
            Map(x => x.Title).Column("Title").Not.Nullable().Length(50);
            Map(x => x.Content).Column("Template").Not.Nullable().Length(99999);
            Map(x => x.CreatedAt).Column("CreatedAt").Not.Nullable();
            Map(x => x.UpdatedAt).Column("UpdatedAt").Not.Nullable();
        }
    }
}