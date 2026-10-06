using LibraryManagement.Application.Common.Exceptions;
using LibraryManagement.Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ValidationException = FluentValidation.ValidationException;

namespace LibraryManagement.Api.ErrorHandling;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails problem = exception switch
        {
            ValidationException validation => new ValidationProblemDetails(
                validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).Distinct().ToArray()))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred."
            },
            DomainValidationException =>
                Create(StatusCodes.Status400BadRequest, "Invalid request.", exception.Message),
            NotFoundException =>
                Create(StatusCodes.Status404NotFound, "Resource not found.", exception.Message),
            ConflictException or BusinessRuleViolationException =>
                Create(StatusCodes.Status409Conflict, "The request conflicts with the current state of the resource.", exception.Message),
            _ =>
                Create(StatusCodes.Status500InternalServerError, "An unexpected error occurred.", detail: null)
        };

        if (problem.Status >= StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception while processing {Method} {Path}",
                httpContext.Request.Method, httpContext.Request.Path);

        httpContext.Response.StatusCode = problem.Status!.Value;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = exception
        });
    }

    private static ProblemDetails Create(int status, string title, string? detail) =>
        new() { Status = status, Title = title, Detail = detail };
}