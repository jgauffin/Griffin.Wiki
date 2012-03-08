using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Griffin.Wiki.WebClient.Models
{

    public class JsonResponse<T>
    {
        public JsonResponse(bool success, T body)
        {
            Success = success;
            Body = body;
        }

        public JsonResponse(T body)
        {
            Success = true;
            Body = body;
        }

        public bool Success { get; set; }
        public T Body { get; set; }
    }

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

    public class AjaxErrorResponse : JsonResponse<string>
    {
        public AjaxErrorResponse(string errorMessage)
            : base(false, errorMessage)
        {
        }


    }
}