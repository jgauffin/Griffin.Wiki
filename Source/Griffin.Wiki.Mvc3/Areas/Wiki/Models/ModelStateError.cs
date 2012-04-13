using System.Linq;
using System.Web.Mvc;

namespace Griffin.Wiki.Mvc3.Areas.Wiki.Models
{
    public class ModelStateError : JsonResponse<object>
    {
        public ModelStateError(ModelStateDictionary modelState)
            : base(modelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                       ))
        {

        }
    }
}