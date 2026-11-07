using ECommerce.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.CustomMiddlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger
            )
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext httpcontext)
        {
            try
            {
                await _next.Invoke(httpcontext);
                if (httpcontext.Response.StatusCode == StatusCodes.Status404NotFound
                    && !httpcontext.Response.HasStarted
                    )
                {
                    var problem = new ProblemDetails
                    {
                        Title = "Error while processing HTTP Request -End point Not Found",
                        Status = StatusCodes.Status404NotFound,
                        Detail = $"The requested resource / Endpoint '{httpcontext.Request.Path}' was not found.",
                        Instance = httpcontext.Request.Path,
                    };
                    await httpcontext.Response.WriteAsJsonAsync(problem);
                }
            }
            catch (Exception ex)
            {
                // Logging
                _logger.LogError(ex, "An unhandled exception occurred.");
                // Return a generic / custom error response
                //httpcontext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var problem = new ProblemDetails
                {
                    Title = "An unexpected error occurred",

                    Detail = $"An unexpected error occurred while processing the request: {ex.Message}",
                    Instance = httpcontext.Request.Path,
                    //Status = StatusCodes.Status500InternalServerError,
                    Status = ex switch
                    {
                        NotFoundException => StatusCodes.Status404NotFound,
                        _ => StatusCodes.Status500InternalServerError,
                    },
                };
                httpcontext.Response.StatusCode = problem.Status.Value;

                await httpcontext.Response.WriteAsJsonAsync(problem);
            }
        }

    }
}
