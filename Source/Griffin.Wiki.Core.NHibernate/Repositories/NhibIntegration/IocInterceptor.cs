namespace Griffin.Wiki.Core.NHibernate.Repositories.NhibIntegration
{
    /*
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Credits: http://kozmic.pl/2011/03/20/working-with-nhibernate-without-default-constructors/</remarks>
    public class ExtensibleInterceptor : EmptyInterceptor
    {
        private readonly INHibernateActivator activator;

        public ISessionFactory SessionFactory { get; set; }

        public ExtensibleInterceptor(INHibernateActivator activator)
        {
            this.activator = activator;
        }

        public override object Instantiate(string clazz, EntityMode entityMode, object id)
        {
            if (entityMode == EntityMode.Poco)
            {
                var type = Type.GetType(clazz);
                if (type != null && activator.CanInstantiate(type))
                {
                    var instance = activator.Instantiate(type);
                    SessionFactory.GetClassMetadata(clazz).SetIdentifier(instance, id, entityMode);
                    return instance;
                }
            }
            return base.Instantiate(clazz, entityMode, id);
        }
    }

    public class CustomProxyValidator : DynProxyTypeValidator
    {
        private const bool iDontCare = true;

        protected override bool HasVisibleDefaultConstructor(Type type)
        {
            return iDontCare;
        }
    }

    public class CustomProxyFactory : AbstractProxyFactory
    {
        protected static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(CustomProxyFactory));
        private static readonly DefaultProxyBuilder proxyBuilder = new DefaultProxyBuilder();

        public CustomProxyFactory()
        {
        }

        public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
        {
            try
            {
                var proxyType = IsClassProxy
                                    ? proxyBuilder.CreateClassProxyType(
                                        PersistentClass,
                                        Interfaces,
                                        ProxyGenerationOptions.Default)
                                    : proxyBuilder.CreateInterfaceProxyTypeWithoutTarget(
                                        Interfaces[0],
                                        Interfaces,
                                        ProxyGenerationOptions.Default);

                var proxy = ServiceResolver.Current.Resolve(proxyType);

                var initializer = new LazyInitializer(EntityName, PersistentClass, id, GetIdentifierMethod, SetIdentifierMethod,
                                                        ComponentIdType, session);
                SetInterceptors(proxy, initializer);
                initializer._constructed = true;
                return (INHibernateProxy)proxy;
            }
            catch (Exception e)
            {
                log.Error("Creating a proxy instance failed", e);
                throw new HibernateException("Creating a proxy instance failed", e);
            }
        }

        public override object GetFieldInterceptionProxy()
        {
            var proxyGenerationOptions = new ProxyGenerationOptions();
            var interceptor = new LazyFieldInterceptor();
            proxyGenerationOptions.AddMixinInstance(interceptor);
            var proxyType = proxyBuilder.CreateClassProxyType(PersistentClass, Interfaces, proxyGenerationOptions);
            var proxy = activator.Instantiate(proxyType);
            SetInterceptors(proxy, interceptor);
            SetMixin(proxy, interceptor);

            return proxy;
        }

        private void SetInterceptors(object proxy, params IInterceptor[] interceptors)
        {
            var field = proxy.GetType().GetField("__interceptors");
            field.SetValue(proxy, interceptors);
        }

        private void SetMixin(object proxy, LazyFieldInterceptor interceptor)
        {
            var fields = proxy.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            var mixin = fields.Where(f => f.Name.StartsWith("__mixin")).Single();
            mixin.SetValue(proxy, interceptor);
        }
    }

    public class CustomProxyFactoryFactory : IProxyFactoryFactory
    {
        public IProxyFactory BuildProxyFactory()
        {
            return new CustomProxyFactory();
        }

        public bool IsInstrumented(Type entityClass)
        {
            return true;
        }

        public bool IsProxy(object entity)
        {
            return (entity is INHibernateProxy);
        }

        public IProxyValidator ProxyValidator
        {
            get { return new CustomProxyValidator(); }
        }
    }

*/
}