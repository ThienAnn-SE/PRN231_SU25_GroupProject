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

        public static ApiResponse<T> CreateErrorResponse(HttpStatusCode status, string message)
        {
            return new ApiResponse<T>
            {
                Status = status,
                Success = false,
                Message = message
            };
        }
        public static ApiResponse<T> CreateErrorResponse(HttpStatusCode status, Exception ex)
        {
            return new ApiResponse<T>
            {
                Status = status,
                Success = false,
                Message = ex.Message,
                Data = null
            };
        }

        public static ApiResponse<T> CreateSuccessResponse(T data, string message = "Operation successful")
        {
            return new ApiResponse<T>
            {
                Status = HttpStatusCode.OK,
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> CreateNotFoundResponse(string message = "Resource not found")
        {
            return new ApiResponse<T>
            {
                Status = HttpStatusCode.NotFound,
                Success = false,
                Message = message
            };
        }

        public static ApiResponse<T> CreateBadRequestResponse(string message = "Bad request")
        {
            return new ApiResponse<T>
            {
                Status = HttpStatusCode.BadRequest,
                Success = false,
                Message = message
            };
        }

        public static ApiResponse<T> CreateUnauthorizedResponse(string message = "Unauthorized access")
        {
            return new ApiResponse<T>
            {
                Status = HttpStatusCode.Unauthorized,
                Success = false,
                Message = message
            };
        }

        public static ApiResponse<T> CreateForbiddenResponse(string message = "Forbidden access")
        {
            return new ApiResponse<T>
            {
                Status = HttpStatusCode.Forbidden,
                Success = false,
                Message = message
            };
        }

        public static ApiResponse<T> CreateInternalServerErrorResponse(string message = "Internal server error")
        {
            return new ApiResponse<T>
            {
                Status = HttpStatusCode.InternalServerError,
                Success = false,
                Message = message
            };
        }

    }
}
