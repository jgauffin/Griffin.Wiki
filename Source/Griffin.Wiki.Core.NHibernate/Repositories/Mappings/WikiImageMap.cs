using FluentNHibernate.Mapping;
using Griffin.Wiki.Core.Images.DomainModels;

namespace Griffin.Wiki.Core.NHibernate.Repositories.Mappings
{
    public class WikiImageMap : ClassMap<WikiImage>
    {
        public WikiImageMap()
        {
            Table("WikiImages");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            References(x => x.WikiPage).Column("PageId");
            References(x => x.User).Column("UploadedBy");
            Map(x => x.Filename).Column("Filename").Not.Nullable().Length(255);
            Map(x => x.Title).Column("Title").Not.Nullable().Length(50);
            Map(x => x.Body).Column("Body").Not.Nullable().Length(2147483647).LazyLoad();
            Map(x => x.UploadedAt).Column("UploadedAt").Not.Nullable();
        }
    }
}