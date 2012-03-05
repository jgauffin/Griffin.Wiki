using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Griffin.Wiki.Core.Repositories;
using NHibernate;
using NHibernate.Event;

namespace Griffin.Wiki.Core.Tests.Repositories
{
    public class SessionFactory
    {
        private static FluentConfiguration _fluentConfig;
        private static ISessionFactory _sessionFactory;

        private static void Setup()
        {
            _fluentConfig = Fluently.Configure()
                            .Database(MsSqlConfiguration.MsSql2008
                                          .ConnectionString(@"data source=.\SQLEXPRESS;Integrated Security=SSPI;Initial Catalog=GriffinWiki")
                            )
                            .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()));
            _sessionFactory = _fluentConfig.BuildSessionFactory();            
        }

        public static ISession Create()
        {
            if (_sessionFactory == null)
                Setup();

// ReSharper disable PossibleNullReferenceException
            return _sessionFactory.OpenSession();
// ReSharper restore PossibleNullReferenceException
        }
    }
}
