using System.Net;

namespace AppCore.BaseModel
{
    public class ApiResponse
    {
        public HttpStatusCode Status { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = String.Empty;

        public static ApiResponse CreateResponse(HttpStatusCode status, bool success, string message)
        {
            return new ApiResponse
            {
                Status = status,
                Success = success,
                Message = message
            };
        }

        public static ApiResponse CreateErrorResponse(HttpStatusCode status, string message)
        {
            return new ApiResponse
            {
                Status = status,
                Success = false,
                Message = message
            };
        }
        public static ApiResponse CreateErrorResponse(HttpStatusCode status, Exception ex)
        {
            return new ApiResponse
            {
                Status = status,
                Success = false,
                Message = ex.Message
            };
        }

        public static ApiResponse CreateSuccessResponse( string message = "Operation successful")
        {
            return new ApiResponse
            {
                Status = HttpStatusCode.OK,
                Success = true,
                Message = message
            };
        }

        public static ApiResponse CreateNotFoundResponse(string message = "Resource not found")
        {
            return new ApiResponse
            {
                Status = HttpStatusCode.NotFound,
                Success = false,
                Message = message
            };
        }

        public static ApiResponse CreateBadRequestResponse(string message = "Bad request")
        {
            return new ApiResponse
            {
                Status = HttpStatusCode.BadRequest,
                Success = false,
                Message = message
            };
        }

        public static ApiResponse CreateUnauthorizedResponse(string message = "Unauthorized access")
        {
            return new ApiResponse
            {
                Status = HttpStatusCode.Unauthorized,
                Success = false,
                Message = message
            };
        }

        public static ApiResponse CreateForbiddenResponse(string message = "Forbidden access")
        {
            return new ApiResponse
            {
                Status = HttpStatusCode.Forbidden,
                Success = false,
                Message = message
            };
        }

        public static ApiResponse CreateInternalServerErrorResponse(string message = "Internal server error")
        {
            return new ApiResponse
            {
                Status = HttpStatusCode.InternalServerError,
                Success = false,
                Message = message
            };
        }
    }

    public class ApiResponse<T> : ApiResponse where T : BaseDto
    {

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

        public static ApiResponse<T> CreateNotFoundResponse(T? data = null, string message = "Resource not found")
        {
            return new ApiResponse<T>
            {
                Status = HttpStatusCode.NotFound,
                Success = false,
                Message = message,
                Data = data
            };
        }
    }

    public class ApiResponses<T> : ApiResponse where T : BaseDto
    {

        public List<T>? Data { get; set; }

        public static ApiResponses<T> CreateResponse(HttpStatusCode status, bool success, string message, List<T>? data = null)
        {
            return new ApiResponses<T>
            {
                Status = status,
                Success = success,
                Message = message,
                Data = data
            };
        }

        public static ApiResponses<T> CreateNotFoundResponse(List<T>? data = null, string message = "Resource not found")
        {
            return new ApiResponses<T>
            { 
                Status = HttpStatusCode.NotFound,
                Success = false,
                Message = message,
                Data = data
            };
        }
    }
}
