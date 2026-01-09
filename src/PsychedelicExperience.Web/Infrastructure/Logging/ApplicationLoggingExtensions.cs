using System;
using Microsoft.AspNetCore.Builder;

namespace PsychedelicExperience.Web.Infrastructure.Logging
{
    internal static class ApplicationLoggingExtensions
    {
        public static IApplicationBuilder UseLoggingHandler(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            return app.UseMiddleware<ApplicationLoggingMiddleware>();
        }
    }
}