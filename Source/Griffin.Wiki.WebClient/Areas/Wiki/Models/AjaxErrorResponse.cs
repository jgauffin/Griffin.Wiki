namespace Griffin.Wiki.WebClient.Areas.Wiki.Models
{
    public class AjaxErrorResponse : JsonResponse<string>
    {
        public AjaxErrorResponse(string errorMessage)
            : base(false, errorMessage)
        {
        }


    }
}