using FluentNHibernate;
using FluentNHibernate.Mapping;
using Griffin.Wiki.Core.Images.DomainModels;
using Griffin.Wiki.Core.Pages;

namespace Griffin.Wiki.Core.NHibernate.Repositories.Mappings
{
    public class WikiImageMap : ClassMap<WikiImage>
    {
        public WikiImageMap()
        {
            Table("WikiImages");
            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            References(x => x.UploadedBy).Column("UploadedBy");
            Map(x => x.Path).CustomType<PagePathType>().Not.Nullable().Length(255);
            Map(x => x.Filename).Column("Filename").Not.Nullable().Length(255);
            Map(x => x.Title).Column("Title").Not.Nullable().Length(50);
            Map(x => x.ContentType).Column("ContentType").Not.Nullable().Length(50);
            Map(Reveal.Member<WikiImage>("Body")).Column("Body").Not.Nullable().Length(int.MaxValue);
            Map(x => x.UploadedAt).Column("UploadedAt").Not.Nullable();
        }
    }
}