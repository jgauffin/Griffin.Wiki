using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Griffin.Wiki.Core.DomainModels;
using Griffin.Wiki.Core.Pages.DomainModels;
using Griffin.Wiki.Core.Repositories;
using NHibernate;
using NHibernate.Bytecode;
using NHibernate.Event;
using NHibernate.Properties;
using NHibernate.Type;

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
                            .Mappings(m => m.FluentMappings.AddFromAssembly(typeof(WikiPage).Assembly));
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

    /*
    public class BytecodeProvider : IBytecodeProvider
    {
        private readonly IContainer container;

        public BytecodeProvider(IContainer container)
        {
            this.container = container;
        }

        #region IBytecodeProvider Members

        public IReflectionOptimizer GetReflectionOptimizer(Type clazz, IGetter[] getters, ISetter[] setters)
        {
            return new ReflectionOptimizer(container, clazz, getters, setters);
        }

        public IProxyFactoryFactory ProxyFactoryFactory
        {
            get { return new ProxyFactoryFactory(); }
        }

        #endregion
    }
    */
/*
    //http://nhforge.org/blogs/nhibernate/archive/2008/12/12/entities-behavior-injection.aspx
    public class ReflectionOptimizer : NHibernate.Bytecode.Lightweight.ReflectionOptimizer
    {
        private readonly IContainer container;

        public ReflectionOptimizer(IContainer container, Type mappedType, IGetter[] getters, ISetter[] setters)
            : base(mappedType, getters, setters)
        {
            this.container = container;
        }

        public override object CreateInstance()
        {
            var instance = container.ResolveOptional(mappedType);
            if (instance != null)
                return instance;

            return base.CreateInstance();
        }

        protected override void ThrowExceptionForNoDefaultCtor(Type type)
        {
        }
    }
*/
}
