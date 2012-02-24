using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using Sogeti.Pattern.InversionOfControl.Autofac;

namespace Griffin.Wiki.Core.Repositories
{
    /*
    class FluentNhibernateContainerModule : IContainerModule
    {
        private ISessionFactory _sessionFactory;
        private FluentConfiguration _fluentConfig;

        public void BuildContainer(ContainerBuilder builder)
        {
            _fluentConfig = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008
                              .ConnectionString(c => c.FromConnectionStringWithKey("YourConnectionStringName"))
                )
                .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()));

            // to trace SQL statements.
            //_fluentConfig.ExposeConfiguration(x => x.SetInterceptor(new SqlStatementInterceptor()));

            _sessionFactory = _fluentConfig.BuildSessionFactory();

            builder.Register(container => _sessionFactory.OpenSession()).As<ISession>().InstancePerLifetimeScope();
        }

    }
     * */
}
