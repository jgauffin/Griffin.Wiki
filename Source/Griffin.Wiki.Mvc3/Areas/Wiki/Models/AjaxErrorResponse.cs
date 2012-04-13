namespace Griffin.Wiki.Mvc3.Areas.Wiki.Models
{
    public class AjaxErrorResponse : JsonResponse<string>
    {
        public AjaxErrorResponse(string errorMessage)
            : base(false, errorMessage)
        {
        }


    }
}