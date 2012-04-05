using System;
using System.Data;
using FluentNHibernate;
using FluentNHibernate.Mapping;
using Griffin.Wiki.Core.Pages;
using Griffin.Wiki.Core.Pages.DomainModels;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

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
            References(x => x.Parent).Column("ParentId").Nullable();
            //Map(x => x.CreatedBy).Column("CreatedBy");
            //Map(x => x.UpdatedBy).Column("UpdatedBy");

            
            //Map(Reveal.Member<WikiPage>("_pagePath")).Column("PageName").CustomType<PagePathType>().Not.Nullable().Length(50);
            Map(x => x.PagePath).Column("PageName").CustomType<PagePathType>().Not.Nullable().Length(50);
            Map(x => x.Title).Column("Title").Not.Nullable().Length(50);
            Map(x => x.CreatedAt).Column("CreatedAt").Not.Nullable();
            Map(x => x.UpdatedAt).Column("UpdatedAt").Not.Nullable();
            Map(x => x.HtmlBody).Column("HtmlBody").Not.Nullable().Length(1073741823);
            Map(x => x.RawBody).Column("RawBody").Not.Nullable().Length(1073741823);
            References(x => x.ChildTemplate).Column("TemplateId");


            HasMany<WikiPageRevision>(Reveal.Member<WikiPage>("_revisions")).KeyColumn("PageId").Inverse().Cascade.All().OrderBy("CreatedAt desc");
            HasManyToMany<WikiPage>(Reveal.Member<WikiPage>("_references")).Table("WikiPageLinks").ParentKeyColumn
                ("Page").ChildKeyColumn("LinkedPage");
            HasManyToMany<WikiPage>(Reveal.Member<WikiPage>("_backReferences")).Table("WikiPageLinks").ParentKeyColumn
                ("LinkedPage").ChildKeyColumn("Page");
            HasMany<WikiPage>(Reveal.Member<WikiPage>("_children")).KeyColumn("ParentId").Inverse().Cascade.
                AllDeleteOrphan();

            //HasMany<WikiPage>(Reveal.Member<WikiPage>("ReferencesInternal")).KeyColumn("LinkedPage");
            //HasMany<WikiPage>(Reveal.Member<WikiPage>("BackReferencesInternal")).KeyColumn("SourcePage");
        }
    }

    public class PagePathType : IUserType
    {
        public SqlType[] SqlTypes
        {
            get
            {
                //We store our Uri in a single column in the database that can contain a string
                SqlType[] types = new SqlType[1];
                types[0] = new SqlType(DbType.String);
                return types;
            }
        }

        public System.Type ReturnedType
        {
            get { return typeof(PagePath); }
        }

        public new bool Equals(object x, object y)
        {
            //Uri implements Equals it self by comparing the Uri's based 
            // on value so we use this implementation
            if (x == null)
            {
                return false;
            }
            else
            {
                return x.Equals(y);
            }
        }

        public int GetHashCode(object x)
        {
            //Again URL itself implements GetHashCode so we use that
            return x.GetHashCode();
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            //We get the string from the database using the NullSafeGet used to get strings 
            string value = (string)NHibernateUtil.String.NullSafeGet(rs, names[0]);

            //And save it in the Uri object. This would be the place to make sure that your string 
            //is valid for use with the System.Uri class, but i will leave that to you
            var result = new PagePath(value);
            return result;
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            //Set the value using the NullSafeSet implementation for string from NHibernateUtil
            if (value == null)
            {
                NHibernateUtil.String.NullSafeSet(cmd, null, index);
                return;
            }
            value = value.ToString(); //ToString called on Uri instance
            NHibernateUtil.String.NullSafeSet(cmd, value, index);
        }

        public object DeepCopy(object value)
        {
            //We deep copy the uri by creating a new instance with the same contents
            if (value == null) return null;
            return new PagePath(value.ToString());
        }

        public bool IsMutable
        {
            get { return false; }
        }

        public object Replace(object original, object target, object owner)
        {
            //As our object is immutable we can just return the original
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            //Used for casching, as our object is immutable we can just return it as is
            return cached;
        }

        public object Disassemble(object value)
        {
            //Used for casching, as our object is immutable we can just return it as is
            return value;
        }
    }

}