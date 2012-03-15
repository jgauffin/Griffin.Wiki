namespace Griffin.Wiki.WebClient.Areas.Wiki.Models
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
}