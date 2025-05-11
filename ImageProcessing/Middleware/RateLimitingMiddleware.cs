using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageProcessingApi.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly int _limit;
        private readonly int _intervalSeconds;
        private readonly ConcurrentDictionary<string, List<DateTime>> _requests = new();

        public RateLimitingMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            // Read the maximum number of allowed requests per interval from config (e.g., appsettings.json)
            _limit = int.Parse(config["RateLimiting:Limit"]);
            // Read the interval duration in seconds from config
            _intervalSeconds = int.Parse(config["RateLimiting:IntervalInSeconds"]);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var key = context.Request.Headers["X-Api-Key"].ToString();
            var now = DateTime.UtcNow;

            var requests = _requests.GetOrAdd(key, new List<DateTime>());

            bool rateLimitExceeded;
            lock (requests)
            {
                requests.RemoveAll(t => (now - t).TotalSeconds > _intervalSeconds);

                if (requests.Count >= _limit)
                {
                    rateLimitExceeded = true;
                }
                else
                {
                    requests.Add(now);
                    rateLimitExceeded = false;
                }
            }

            if (rateLimitExceeded)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers["Retry-After"] = _intervalSeconds.ToString();
                await context.Response.WriteAsync("Rate limit exceeded.");
                return;
            }

            await _next(context);
        }
    }
}