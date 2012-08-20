using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Griffin.Container;

namespace Griffin.Wiki.Core
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
