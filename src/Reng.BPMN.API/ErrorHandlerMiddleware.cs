using System;
using System.Net;
using System.Text.Json;
using Reng.BPMN.Domain;

namespace Reng.BPMN.API;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;
    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            switch (error)
            {
                case RengDomainException e:
                    // custom application error
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case KeyNotFoundException e:
                    // not found error
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                case ArgumentOutOfRangeException e:
                    // not found error
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                default:
                    // unhandled error
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var result = JsonSerializer.Serialize(new RestApiResponse
            {
                IsSuccess = false,
                Message = error?.Message
            });

            _logger.LogError(result);

            await response.WriteAsync(result);
        }
    }
}

public class RestApiResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
}