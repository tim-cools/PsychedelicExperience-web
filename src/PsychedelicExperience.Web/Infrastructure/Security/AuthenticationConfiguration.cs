using System;
using System.Net.Http;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Web.Infrastructure.Security
{
    public static class AuthenticationConfiguration
    {
        public const string Bearer = "Bearer";
        public const string ServerSchema = "ServerCookie";

        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services
                .AddAuthentication()
                .AddOAuthValidation(options =>
                {
                    options.Audiences.Add(configuration.ApiHostName());
                })
                .AddCookie(ServerSchema, options =>
                {
                    options.Cookie.Name = "_pex.s3";
                    options.ExpireTimeSpan = TimeSpan.FromDays(30);
                    options.LoginPath = new PathString("/signin");
                })
                .ConfigureWhen(configuration.AuthenticationFacebookConfigured(), FacebookAuthentication(configuration))
                .ConfigureWhen(configuration.AuthenticationGoogleConfigured(), GoogleAuthentication(configuration))
                .AddOpenIdConnectServer(options =>
                {
                    options.Provider = new AuthorizationServerProvider();
                    options.AllowInsecureHttp = true;
                    options.AuthorizationEndpointPath = "/account/authorize";
                    options.TokenEndpointPath = "/token";
                    options.AccessTokenLifetime = TimeSpan.FromMinutes(20);
                    options.RefreshTokenLifetime = TimeSpan.FromDays(30);
                });

            return services;
        }

        private static Action<AuthenticationBuilder> GoogleAuthentication(IConfiguration configuration)
        {
            return app => app.AddGoogle(options =>
            {
                options.SignInScheme = ServerSchema;
                options.ClientId = configuration.AuthenticationGoogleClientId();
                options.ClientSecret = configuration.AuthenticationGoogleClientSecret();
            });
        }

        private static Action<AuthenticationBuilder> FacebookAuthentication(IConfiguration configuration)
        {
            return app => app.AddFacebook(options =>
            {
                options.SignInScheme = ServerSchema;
                options.AppId = configuration.AuthenticationFacebookAppId();
                options.AppSecret = configuration.AuthenticationFacebookAppSecret();
                options.BackchannelHttpHandler = new HttpClientHandler();
                options.UserInformationEndpoint = "https://graph.facebook.com/v2.5/me?fields=id,name,email";
                options.Scope.Add("email");
            });
        }

        public static AuthenticationBuilder ConfigureWhen(this AuthenticationBuilder authentication, bool condition, Action<AuthenticationBuilder> configuration)
        {
            if (authentication == null) throw new ArgumentNullException(nameof(authentication));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            if (condition)
            {
                configuration(authentication);
            }
            return authentication;
        }
    }
}
