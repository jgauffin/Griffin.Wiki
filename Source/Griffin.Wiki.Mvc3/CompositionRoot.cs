using System.Reflection;
using Griffin.Container;

namespace Griffin.Wiki.Mvc3
{
    public class CompositionRoot : IContainerModule
    {
        /// <summary>
        /// Register all services
        /// </summary>
        /// <param name="registrar">Registrar used for the registration</param>
        public void Register(IContainerRegistrar registrar)
        {
            registrar.RegisterComponents(Lifetime.Scoped, Assembly.GetExecutingAssembly());
        }
    }
}
