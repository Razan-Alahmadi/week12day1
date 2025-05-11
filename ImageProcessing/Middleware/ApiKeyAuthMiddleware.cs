using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using ImageProcessingApi.Controllers;

namespace ImageProcessingApi.Middleware
{
    public class ApiKeyAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiKeyAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            if (path != null && (path.StartsWith("/index.html") ||
                                 path.StartsWith("/css") ||
                                 path.StartsWith("/js") ||
                                 path.StartsWith("/images") ||
                                 path.StartsWith("/favicon.ico") ||
                                 path.StartsWith("/api/apikeys/generate")))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("X-Api-Key", out var apiKey) ||
                !ApiKeysController.IsValidApiKey(apiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid or missing API Key.");
                return;
            }

            await _next(context);
        }

    }
}

