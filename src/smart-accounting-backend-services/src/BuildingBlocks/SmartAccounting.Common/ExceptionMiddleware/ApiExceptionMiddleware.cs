using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartAccounting.Common.ExceptionMiddleware
{
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiExceptionMiddleware> _logger;

        public ApiExceptionMiddleware(RequestDelegate next,
                                      ILogger<ApiExceptionMiddleware> logger)
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var error = new ApiRequestProcessingError
            {
                Id = Guid.NewGuid().ToString(),
                Code = (short)HttpStatusCode.InternalServerError,
                Title = "Some kind of error occurred in the API.",
                Detail = "Please use the id and contact our support team if the problem persists."
            };

            var innerExMessage = exception.GetBaseException().Message;

            _logger.LogError(exception, "API Error: " + "{ErrorMessage} -- {ErrorId}.", innerExMessage, error.Id);

            var result = JsonSerializer.Serialize(error);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync(result);
        }
    }
}
