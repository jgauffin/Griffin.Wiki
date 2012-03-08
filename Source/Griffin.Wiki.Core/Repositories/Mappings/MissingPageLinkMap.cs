using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate;
using FluentNHibernate.Mapping;
using Griffin.Wiki.Core.DomainModels;

namespace Griffin.Wiki.Core.Repositories.Mappings
{

    public class MissingPageLinkMap : ClassMap<MissingPageLink>
    {
        public MissingPageLinkMap()
        {
            Table("WikiMissingPageLinks");
            LazyLoad();
            Id(Reveal.Member<MissingPageLink>("Id")).GeneratedBy.Identity().Column("Id");
            References(x => x.Page).Column("PageId");
            Map(x => x.MissingPageName).Column("MissingPageName").Not.Nullable().Length(50);
        }
    }
}
