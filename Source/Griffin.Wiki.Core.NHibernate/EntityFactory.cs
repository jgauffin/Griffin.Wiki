using System;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Griffin.Container;
using Griffin.Wiki.Core.Data;
using NHibernate;
using NHibernate.Event.Default;
using NHibernate.Type;

namespace Griffin.Wiki.Core.NHibernate
{
    public class NhibernateModule : DefaultLoadEventListener, IContainerModule
    {
        private readonly FluentConfiguration _fluentConfig;
        private readonly ISessionFactory _sessionFactory;

        public NhibernateModule()
        {
            _fluentConfig = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008
                              .ConnectionString(c => c
                                                         .FromConnectionStringWithKey("GriffinWiki"))
                )
                .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()));
            /*m.FluentMappings.Conventions.AddFromAssemblyOf<PrimaryKeyConvention>();
m.FluentMappings.Conventions.Add<PrimaryKeyConvention>();
m.FluentMappings.Conventions.Add<ForeignKeyNameConvention>();
m.FluentMappings.AddFromAssemblyOf<Program>();*/

            //_fluentConfig.ExposeConfiguration(x => x.EventListeners.LoadEventListeners = new ILoadEventListener[] {this});
            //_fluentConfig.ExposeConfiguration(x => x. = new ILoadEventListener[] { this });
            //_fluentConfig.ExposeConfiguration(x => x.SetInterceptor(new SqlStatementInterceptor()));
            //_fluentConfig.ExposeConfiguration(x=>x.l)
            _sessionFactory = _fluentConfig.BuildSessionFactory();
        }

        // this is the single method defined by the LoadEventListener interface
        /*public override void OnLoad(LoadEvent theEvent, LoadType loadType)
        {
            var type = Type.GetType(theEvent.EntityClassName);

            //_fluentConfig.Mappings(m => m.FluentMappings.)
            if (null == theEvent.InstanceToLoad)
            {
                theEvent.InstanceToLoad = ServiceResolver.Current.Resolve(type);
            }
        }*/

        #region IContainerModule Members

        /// <summary>
        /// Register all services
        /// </summary>
        /// <param name="registrar">Registrar used for the registration</param>
        public void Register(IContainerRegistrar registrar)
        {
            registrar.RegisterService<ISession>(x=> _sessionFactory.OpenSession(), Lifetime.Scoped);
            registrar.RegisterService<IUnitOfWork>(x => new NhibernateUow(x.Resolve<ISession>()),
                                                   Lifetime.Scoped);
        }

        #endregion

        #region Nested type: NhibernateUow

        private class NhibernateUow : IUnitOfWork
        {
            private readonly ISession _session;
            private ITransaction _transaction;

            public NhibernateUow(ISession session)
            {
                _session = session;
                _transaction = _session.BeginTransaction();
            }

            #region IUnitOfWork Members

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public void Dispose()
            {
                if (_transaction == null)
                    return;

                _transaction.Dispose();
                _transaction = null;
            }

            public void SaveChanges()
            {
                if (_transaction == null)
                    throw new InvalidOperationException("UoW has already been saved.");

                _transaction.Commit();
                _transaction = null;
            }

            #endregion
        }

        #endregion

        #region Nested type: SqlStatementInterceptor

        public class SqlStatementInterceptor : EmptyInterceptor
        {
            public override object Instantiate(string clazz, EntityMode entityMode, object id)
            {
                var entity = base.Instantiate(clazz, entityMode, id);
                if (entity == null)
                    return entity;

                InjectProperties(entity);

                return entity;
            }

            private static void InjectProperties(object entity)
            {
                return; //TODO: required?

                foreach (var property in entity.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    /*var isOur = property.GetCustomAttributes(typeof (InjectMemberAttribute), true).Length > 0;
                    if (!isOur)
                        continue;
                    */
                    //property.SetValue(entity, ServiceResolver.Current.Resolve(property.PropertyType), null);
                }
            }

            public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
            {
                if (entity != null)
                    InjectProperties(entity);

                return base.OnLoad(entity, id, state, propertyNames, types);
            }

            /*public override NHibernate.SqlCommand.SqlString OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
            {
                Trace.WriteLine(sql.ToString());
                return sql;
            }*/
        }

        #endregion
    }
}