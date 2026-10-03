using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Exceptions
{
    public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is ValidationException validationException)
            {
                httpContext.Response.StatusCode =
                    StatusCodes.Status400BadRequest;

                var errors = validationException.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group
                            .Select(error => error.ErrorMessage)
                            .Distinct()
                            .ToArray());

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation failed.",
                    Detail = "One or more validation errors occurred.",
                    Instance = httpContext.Request.Path
                };

                problemDetails.Extensions["errors"] = errors;

                await problemDetailsService.WriteAsync(
                    new ProblemDetailsContext
                    {
                        HttpContext = httpContext,
                        ProblemDetails = problemDetails
                    });

                return true;
            }

            if (exception is ArgumentException)
            {
                httpContext.Response.StatusCode =
                    StatusCodes.Status400BadRequest;

                await problemDetailsService.WriteAsync(
                    new ProblemDetailsContext
                    {
                        HttpContext = httpContext,
                        ProblemDetails = new ProblemDetails
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Title = "Invalid request.",
                            Detail = exception.Message,
                            Instance = httpContext.Request.Path
                        }
                    });

                return true;
            }

            logger.LogError(
                exception,
                "Unhandled exception while processing {Path}.",
                httpContext.Request.Path);

            httpContext.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            await problemDetailsService.WriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "An unexpected error occurred.",
                        Detail = "The server could not complete the request.",
                        Instance = httpContext.Request.Path
                    }
                });

            return true;
        }
    }
}
