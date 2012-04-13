using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace Griffin.Wiki.Mvc3.Areas.Wiki
{
    /// <summary>
    /// List constraint for routing.
    /// </summary>
    public class RouteListConstraint : IRouteConstraint
    {
        private readonly IEnumerable<string> _values;

        public RouteListConstraint(IEnumerable<string> values)
        {
            this._values = values;
        }

        public bool Match(System.Web.HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (!values.ContainsKey(parameterName)) 
                return false;

            var value = values[parameterName].ToString();
            return !string.IsNullOrEmpty(value) && _values.Contains(value, StringComparer.OrdinalIgnoreCase);
        }
    }
}