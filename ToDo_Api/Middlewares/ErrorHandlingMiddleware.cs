using Newtonsoft.Json;

namespace ToDo_Api.Middlewares;

/// <summary>
/// Middleware responsible for global error handling in the application.
/// Catches exceptions during the request pipeline and returns a standardized JSON error response.
/// </summary>
public class ErrorHandlingMiddleware : IMiddleware
{
    /// <summary>
    /// Invokes the middleware in the HTTP request pipeline and handles any thrown exceptions.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="next">Delegate for invoking the next middleware in the pipeline.</param>
    /// <returns>A task that represents the completion of the middleware execution.</returns>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (BadHttpRequestException e)
        {
            await ErrorResponse(context, e.StatusCode, e.Message);
        }
        catch (Exception e)
        {
            await ErrorResponse(context, 500, e.Message);
        }
    }

    /// <summary>
    /// Sends a standardized JSON error response with the specified status code and message.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="statusCode">The HTTP status code to return.</param>
    /// <param name="message">The error message to include in the response.</param>
    /// <returns>A task that represents writing the response to the client.</returns>
    private static async Task ErrorResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var errorResponse = new ErrorResponse(statusCode, message);
        var jsonMessage = JsonConvert.SerializeObject(errorResponse);

        await context.Response.WriteAsync(jsonMessage);
    }
}
