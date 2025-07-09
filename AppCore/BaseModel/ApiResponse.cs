using System.Net;

namespace AppCore.BaseModel
{
    public class ApiResponse<T> where T : BaseDto
    {
        public HttpStatusCode Status { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = String.Empty;
        public T? Data { get; set; }

        public static ApiResponse<T> CreateResponse(HttpStatusCode status, bool success, string message, T? data = null)
        {
            return new ApiResponse<T>
            {
                Status = status,
                Success = success,
                Message = message,
                Data = data
            };
        }
    }
}
