namespace AiMeetingSummariser.Api.Application.Common;

public class ResponseWrapper<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    public static ResponseWrapper<T> SuccessResponse(T data, string? message = null)
    {
        return new ResponseWrapper<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ResponseWrapper<T> ErrorResponse(string message, List<string>? errors = null)
    {
        return new ResponseWrapper<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }

    public static ResponseWrapper<T> ErrorResponse(string message)
    {
        return new ResponseWrapper<T>
        {
            Success = false,
            Message = message
        };
    }
}
