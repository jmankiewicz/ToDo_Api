namespace ToDo_Api.Middlewares;

/// <summary>
/// Represents a standardized error response returned in JSON format.
/// </summary>
public class ErrorResponse
{
    public int StatusCode { get; }
    public string Message { get; }

    public ErrorResponse(int statusCode = 500, string message = "Not provided error message.")
    {
        StatusCode = statusCode;
        Message = message;
    }
}
