using ECommerce.Services.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace ECommerce.Presentation.Attributes
{
    public class RedisCacheAttribute : ActionFilterAttribute
    {
        private readonly int _dutationInMins;

        public RedisCacheAttribute(int dutationInMins = 5)
        {
            _dutationInMins = dutationInMins;
        }
        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next
            )
        {
            // Get CashService from DI container
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<ICacheService>();
            // Check if the response is already cached based on the request path and query parameters
            var cacheKey = CreateCacheKey(context.HttpContext.Request);
            // Check if data is already cached
            var cacheValue = await cacheService.GetAsync(cacheKey);
            // if it is, return the cached response
            if (cacheValue is not null)
            {
                context.Result = new ContentResult()
                {
                    Content = cacheValue,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };
                return;
            }
            // if not, execute the action and cache the response
            var ExecutedContext = await next.Invoke();
            if (ExecutedContext.Result is OkObjectResult result)
            {
                // Cache the response for future requests
                await cacheService.SetAsync(
                    cacheKey,
                    result.Value!,
                    TimeSpan.FromMinutes(_dutationInMins)
                );
            }

        }

        private string CreateCacheKey(HttpRequest request)
        {
            StringBuilder key = new StringBuilder();
            key.Append(request.Path);
            foreach (var item in request.Query.OrderBy(x => x.Key))
            {
                key.Append($"|{item.Key}-{item.Value}");
            }
            return key.ToString();
        }
    }
}
