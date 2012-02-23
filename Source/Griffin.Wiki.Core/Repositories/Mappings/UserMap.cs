using FluentNHibernate.Mapping;
using Griffin.Wiki.Core.DomainModels;

namespace Griffin.Wiki.Core.Repositories.Mappings
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("Users");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Identity().Column("Id");
            Map(x => x.DisplayName).Column("DisplayName").Not.Nullable().Length(50);
            Map(x => x.AccountName).Column("AccountName").Not.Nullable().Length(50);
        }
    }
}