using System;
using System.Reflection;
using Autofac;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Event;
using NHibernate.Event.Default;
using Sogeti.Pattern.InversionOfControl;
using Sogeti.Pattern.InversionOfControl.Autofac;

namespace Griffin.Wiki.Core.Repositories
{
    internal class EntityFactory
    {
    }

    public class NhibernateModule : DefaultLoadEventListener, IContainerModule
    {
        private FluentConfiguration _fluentConfig;

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
            //_fluentConfig.ExposeConfiguration(x => x.SetInterceptor(new SqlStatementInterceptor()));
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
    }
}