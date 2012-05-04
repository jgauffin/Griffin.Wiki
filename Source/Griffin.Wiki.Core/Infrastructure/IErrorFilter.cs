using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Griffin.Wiki.Core.Infrastructure
{
    /// <summary>
    /// Implement this class to take care of all unhandled errors
    /// </summary>
    public interface IErrorFilter
    {
        /// <summary>
        /// Handle the error
        /// </summary>
        /// <param name="context">context information</param>
        /// <remarks>return <c>null</c> as <c>context.Result</c> if you want the default handling (which also includes the detailed logging)</remarks>
        void Handle(ExceptionContext context);
    }

}
