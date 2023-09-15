using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Errors.Errors.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Errors.Errors.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder MapProblemDetails(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>()!;
                var exception = exceptionHandlerPathFeature.Error;

                if (exception is ValidationErrorsException validationErrorsException)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Title = "Validation failed",
                        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                        Detail = validationErrorsException.Message,
                        Status = StatusCodes.Status400BadRequest,
                    };

                    problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.TraceIdentifier);

                    problemDetails.Extensions["errors"] = validationErrorsException.Errors
                        .Select(x => new ErrorData(x.Field, x.Message));

                    context.Response.ContentType = "application/problem+json";
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;

                    await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
                }
            });
        });

        return app;
    }

    public static IApplicationBuilder MapProblemDetailsWithCodes(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>()!;
                var exception = exceptionHandlerPathFeature.Error;

                if (exception is ValidationErrorsException validationErrorsException)
                {
                    var problemDetails = new ProblemDetails
                    {
                        Title = "Validation failed",
                        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                        Detail = validationErrorsException.Message,
                        Status = StatusCodes.Status400BadRequest,
                    };

                    problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.TraceIdentifier);

                    problemDetails.Extensions["errors"] = validationErrorsException.Errors
                        .Select(x => new ErrorDataWithCode(x.Field, x.Message, x.Code));

                    context.Response.ContentType = "application/problem+json";
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;

                    await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
                }
            });
        });

        return app;
    }

    public static IApplicationBuilder MapProblemDetailsWithLogicConflicts(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>()!;
                var exception = exceptionHandlerPathFeature.Error;

                switch (exception)
                {
                    case ValidationErrorsException validationErrorsException:
                    {
                        var validationProblemDetails = new ProblemDetails
                        {
                            Title = "Validation failed",
                            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400",
                            Detail = validationErrorsException.Message,
                            Status = StatusCodes.Status400BadRequest,
                        };

                        validationProblemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.TraceIdentifier);

                        validationProblemDetails.Extensions["errors"] = validationErrorsException.Errors
                            .Select(x => new ErrorDataWithCode(x.Field, x.Message, x.Code));

                        context.Response.ContentType = "application/problem+json";
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;

                        await context.Response.WriteAsync(JsonSerializer.Serialize(validationProblemDetails));
                        break;
                    }
                    case LogicConflictException logicConflictException:
                        var logicConflictProblemDetails = new ProblemDetails
                        {
                            Title = "Logic conflict",
                            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/422",
                            Detail = logicConflictException.Message,
                            Status = StatusCodes.Status422UnprocessableEntity,
                        };

                        logicConflictProblemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.TraceIdentifier);

                        logicConflictProblemDetails.Extensions["code"] = logicConflictException.Code;

                        context.Response.ContentType = "application/problem+json";
                        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

                        await context.Response.WriteAsync(JsonSerializer.Serialize(logicConflictProblemDetails));
                        break;
                }
            });
        });

        return app;
    }

    public static IApplicationBuilder MapProblemDetailsComplete(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>()!;
                var exception = exceptionHandlerPathFeature.Error;

                switch (exception)
                {
                    case ValidationErrorsException validationErrorsException:
                    {
                        var validationProblemDetails = new ProblemDetails
                        {
                            Title = "Validation failed",
                            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/400",
                            Detail = validationErrorsException.Message,
                            Status = StatusCodes.Status400BadRequest,
                        };

                        validationProblemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.TraceIdentifier);

                        validationProblemDetails.Extensions["errors"] = validationErrorsException.Errors
                            .Select(x => new ErrorDataWithCode(x.Field, x.Message, x.Code));

                        context.Response.ContentType = "application/problem+json";
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;

                        await context.Response.WriteAsync(JsonSerializer.Serialize(validationProblemDetails));
                        break;
                    }
                    case LogicConflictException logicConflictException:
                        var logicConflictProblemDetails = new ProblemDetails
                        {
                            Title = "Logic conflict",
                            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/422",
                            Detail = logicConflictException.Message,
                            Status = StatusCodes.Status422UnprocessableEntity,
                        };

                        logicConflictProblemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.TraceIdentifier);

                        logicConflictProblemDetails.Extensions["code"] = logicConflictException.Code;

                        context.Response.ContentType = "application/problem+json";
                        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

                        await context.Response.WriteAsync(JsonSerializer.Serialize(logicConflictProblemDetails));
                        break;
                    case TooManyRequestsException tooManyRequestsException:
                        var tooManyRequestsProblemDetails = new ProblemDetails
                        {
                            Title = "Too many requests",
                            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/429",
                            Detail = tooManyRequestsException.Message,
                            Status = StatusCodes.Status429TooManyRequests,
                        };

                        tooManyRequestsProblemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.TraceIdentifier);

                        context.Response.ContentType = "application/problem+json";
                        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                        await context.Response.WriteAsync(JsonSerializer.Serialize(tooManyRequestsProblemDetails));
                        break;
                    case OperationCanceledException:
                        var operationCanceledProblemDetails = new ProblemDetails
                        {
                            Title = "Timeout",
                            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/504",
                            Detail = "Request timed out",
                            Status = StatusCodes.Status504GatewayTimeout,
                        };

                        operationCanceledProblemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.TraceIdentifier);

                        context.Response.ContentType = "application/problem+json";
                        context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;

                        await context.Response.WriteAsync(JsonSerializer.Serialize(operationCanceledProblemDetails));
                        break;
                    case InternalErrorException internalErrorException:
                        var internalErrorExProblemDetails = new ProblemDetails
                        {
                            Title = "Internal error",
                            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/504",
                            Detail = internalErrorException.Message,
                            Status = StatusCodes.Status500InternalServerError,
                        };

                        internalErrorExProblemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.TraceIdentifier);

                        context.Response.ContentType = "application/problem+json";
                        context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;

                        await context.Response.WriteAsync(JsonSerializer.Serialize(internalErrorExProblemDetails));
                        break;
                    default:
                        var internalErrorProblemDetails = new ProblemDetails
                        {
                            Title = "Internal server error",
                            Type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/500",
                            Detail = "Interval server error has occured",
                            Status = StatusCodes.Status500InternalServerError,
                        };

                        internalErrorProblemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.TraceIdentifier);

                        context.Response.ContentType = "application/problem+json";
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                        await context.Response.WriteAsync(JsonSerializer.Serialize(internalErrorProblemDetails));
                        break;
                }
            });
        });

        return app;
    }
}

internal record ErrorData(
    [property: JsonPropertyName("field")] string Field,
    [property: JsonPropertyName("message")] string Message);

internal record ErrorDataWithCode(
    [property: JsonPropertyName("field")] string Field,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("code")] string Code);