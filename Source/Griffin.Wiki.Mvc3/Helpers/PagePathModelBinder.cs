using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Griffin.Wiki.Core.Pages;

namespace Griffin.Wiki.Mvc3.Helpers
{
    /// <summary>
    /// Used to load the page path
    /// </summary>
    public class PagePathModelBinder : IModelBinder
    {
        /// <summary>
        /// Binds the model to a value by using the specified controller context and binding context.
        /// </summary>
        /// <returns>
        /// The bound value.
        /// </returns>
        /// <param name="controllerContext">The controller context.</param><param name="bindingContext">The binding context.</param>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof (PagePath))
                return null;

            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName) ??
                        bindingContext.ValueProvider.GetValue("pagePath");

            if (value == null || value.RawValue == null)
                return null;


            var path =
                value.RawValue is string[]
                    ? ((string[]) value.RawValue)[0]
                    : value.RawValue.ToString();

            if (path == "/")
                return new PagePath("/");

            if (!path.StartsWith("/") && !path.EndsWith("/"))
                path = string.Format("/{0}/", path);
            else if (!path.StartsWith("/"))
                path = string.Format("/{0}", path);
            else if (!path.EndsWith("/"))
                path = string.Format("{0}/", path);

            return new PagePath(path);
        }
    }
}