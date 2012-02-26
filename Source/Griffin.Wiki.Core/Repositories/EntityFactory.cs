using System;
using System.Diagnostics;
using System.Reflection;
using Autofac;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Event;
using NHibernate.Event.Default;
using Sogeti.Pattern.Data.NHibernate;
using Sogeti.Pattern.InversionOfControl;
using Sogeti.Pattern.InversionOfControl.Autofac;

namespace Griffin.Wiki.Core.Repositories
{
    public class NhibernateModule : DefaultLoadEventListener, IAutofacContainerModule
    {
        private FluentConfiguration _fluentConfig;
        private ISessionFactory _sessionFactory;

        #region IContainerModule Members

        /// <summary>
        /// Add registrations to the container builder.
        /// </summary>
        /// <param name="builder">Builder to add registrations to.</param>
        public void BuildContainer(ContainerBuilder builder)
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

            _fluentConfig.ExposeConfiguration(x => x.EventListeners.LoadEventListeners = new ILoadEventListener[] {this});
            //_fluentConfig.ExposeConfiguration(x => x. = new ILoadEventListener[] { this });
            _fluentConfig.ExposeConfiguration(x => x.SetInterceptor(new SqlStatementInterceptor()));
            //_fluentConfig.ExposeConfiguration(x=>x.l)
            _sessionFactory = _fluentConfig.BuildSessionFactory();

            builder.RegisterType<NHibernateUnitOfWork>().AsImplementedInterfaces();
            builder.Register(k => _sessionFactory.OpenSession()).As<ISession>().InstancePerLifetimeScope();
            
        }

        #endregion

        // this is the single method defined by the LoadEventListener interface
        public override void OnLoad(LoadEvent theEvent, LoadType loadType)
        {
            var type = Type.GetType(theEvent.EntityClassName);

            //_fluentConfig.Mappings(m => m.FluentMappings.)
            if (null == theEvent.InstanceToLoad)
            {
                theEvent.InstanceToLoad = ServiceResolver.Current.Resolve(type);
            }
        }

        public class SqlStatementInterceptor : EmptyInterceptor
        {
            public override object Instantiate(string clazz, EntityMode entityMode, object id)
            {
                var entity = base.Instantiate(clazz, entityMode, id);
                if(entity == null)
                    return entity;

                InjectProperties(entity);

                return entity;
            }

            private static void InjectProperties(object entity)
            {
                foreach (var property in entity.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    var isOur = property.GetCustomAttributes(typeof (InjectMemberAttribute), true).Length > 0;
                    if (!isOur)
                        continue;

                    property.SetValue(entity, ServiceResolver.Current.Resolve(property.PropertyType), null);
                }
            }

            public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames, NHibernate.Type.IType[] types)
            {
                if (entity != null)
                    InjectProperties(entity);

                return base.OnLoad(entity, id, state, propertyNames, types);
            }
            public override NHibernate.SqlCommand.SqlString OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
            {
                Trace.WriteLine(sql.ToString());
                return sql;
            }
        }
    }
}