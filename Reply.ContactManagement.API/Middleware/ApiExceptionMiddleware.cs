using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reply.ContactManagement.Core.Exceptions;

namespace Reply.ContactManagement.API.Middleware;

public sealed class ApiExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (EntityNotFoundException exception)
        {
            await WriteProblemDetailsAsync(context, StatusCodes.Status404NotFound, exception.Message);
        }
        catch (InvalidRequestException exception)
        {
            await WriteProblemDetailsAsync(context, StatusCodes.Status400BadRequest, exception.Message);
        }
        catch (ConflictException exception)
        {
            await WriteProblemDetailsAsync(context, StatusCodes.Status409Conflict, exception.Message);
        }
    }

    private static Task WriteProblemDetailsAsync(HttpContext context, int statusCode, string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        return context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Detail = detail
        });
    }
}
