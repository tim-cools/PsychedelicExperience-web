using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PsychedelicExperience.Web.Controllers.Api;

namespace PsychedelicExperience.Web.Infrastructure.Logging
{
    public class ApplicationLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApplicationLoggingMiddleware> _logger;

        public ApplicationLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));

            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = loggerFactory.CreateLogger<ApplicationLoggingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                if (context.Request.Query.ContainsKey("diagnostics"))
                {
                    await DiagnosticsData(context);
                }
                else
                {
                    await _next(context);
                }
            }
            catch (Exception ex)
            {
                var userId = context.User.GetUserId();
                var errorMessage = $"(User id: '{userId}') An unhandled exception has occurred while executing the request: {context.Request.GetEncodedPathAndQuery()}";
                _logger.LogError(0, ex, errorMessage);
                throw;
            }
        }

        private static async Task DiagnosticsData(HttpContext context)
        {
            var request = context.Request;
            var data = new
            {
                Environment = Environment.GetEnvironmentVariables(),
                Request = new
                {
                    request.Scheme,
                    request.Host,
                    request.IsHttps,
                    request.Path,
                    request.Headers,
                    request.QueryString,
                    request.Cookies
                }
            };

            context.Response.StatusCode = 200;
            var json = JsonConvert.SerializeObject(data);
            await context.Response.WriteAsync(json);
        }
    }
}