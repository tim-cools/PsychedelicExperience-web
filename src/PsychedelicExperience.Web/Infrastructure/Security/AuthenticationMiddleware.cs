using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace PsychedelicExperience.Web.Infrastructure.Security
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        
        public IAuthenticationSchemeProvider Schemes { get; set; }

        public AuthenticationMiddleware(RequestDelegate next, IAuthenticationSchemeProvider schemes)
        {
            _next = next ?? throw new ArgumentNullException(nameof (next));
            Schemes = schemes ?? throw new ArgumentNullException(nameof (schemes));
        }

        public async Task Invoke(HttpContext context)
        {
            SetAuthenticationFeature(context);

            if (!await ExecuteRequestHandlers(context)) return;

            await Authenticate(context);
            
            await _next(context);
        }

        private static void SetAuthenticationFeature(HttpContext context)
        {
            context.Features.Set<IAuthenticationFeature>(new AuthenticationFeature
            {
                OriginalPath = context.Request.Path,
                OriginalPathBase = context.Request.PathBase
            });
        }

        private async Task<bool> ExecuteRequestHandlers(HttpContext context)
        {
            var handlers = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            
            foreach (var authenticationScheme in await Schemes.GetRequestHandlerSchemesAsync())
            {
                var handler = await handlers.GetHandlerAsync(context, authenticationScheme.Name);
                if (handler is IAuthenticationRequestHandler handlerAsync && await handlerAsync.HandleRequestAsync())
                {
                    return false;
                }
            }

            return true;
        }

        private async Task Authenticate(HttpContext context)
        {
            var authenticateSchemeAsync = await GetAuthenticateSchemeAsync(context);

            if (authenticateSchemeAsync != null)
            {
                var authenticateResult = await context.AuthenticateAsync(authenticateSchemeAsync.Name);
                if (authenticateResult?.Principal != null)
                    context.User = authenticateResult.Principal;
            }
        }

        private async Task<AuthenticationScheme> GetAuthenticateSchemeAsync(HttpContext context)
        {
            var scheme = context.Request.Path.Value.ToLowerInvariant().StartsWith("/api") 
                ? AuthenticationConfiguration.Bearer
                : AuthenticationConfiguration.ServerSchema;
            
            return await Schemes.GetSchemeAsync(scheme);
        }
    }
}