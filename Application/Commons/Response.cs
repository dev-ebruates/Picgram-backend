namespace Application.Commons;

public class Response<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }

    public Response(bool success, string message, T? data)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    public static Response<T> CreateSuccessResponse(T? data = default, string message = "Request was successful.")
    {
        return new Response<T>(true, message, data);
    }

    public static Response<T> CreateErrorResponse(string message)
    {
        return new Response<T>(false, message, default);
    }
}

