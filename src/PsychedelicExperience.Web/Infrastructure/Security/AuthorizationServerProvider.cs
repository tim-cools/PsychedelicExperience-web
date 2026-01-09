using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using PsychedelicExperience.Common.Messages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Clients;
using PsychedelicExperience.Membership.Messages.RefreshTokens;
using PsychedelicExperience.Membership.Messages.Users;
using AuthenticationProperties = Microsoft.AspNetCore.Authentication.AuthenticationProperties;

namespace PsychedelicExperience.Web.Infrastructure.Security
{
    public class AuthorizationServerProvider : OpenIdConnectServerProvider
    {
        private const string AuthorizationRequestKey = "authorization-request-";
     
        /// <summary>
        /// Validates whether the client is a valid known application in our system.
        /// </summary>
        public override async Task ValidateTokenRequest(ValidateTokenRequestContext context)
        {
            var query = new ClientValidator(context.ClientId, context.ClientSecret);
            var result = await ExecuteMessage(context, query);
            if (!result.Succeeded)
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AuthorizationServerProvider>>();
                logger.LogError("Client not found in the database: ensure that your client_id is correct: {ClientId}.", context.ClientId);

                context.Reject(error: "invalid_client", description: "Client not found in the database: ensure that your client_id is correct");
                return;
            }

            context.HttpContext.Items.Add("as:clientAllowedOrigin", result.AllowedOrigin);

            context.Validate();
        }


        public override async Task ExtractAuthorizationRequest(ExtractAuthorizationRequestContext context)
        {
            var cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AuthorizationServerProvider>>();

            // Reject requests using the unsupported request parameter.
            var request = context.Request.GetParameter(OpenIdConnectConstants.Parameters.Request);
            if (request.HasValue && !string.IsNullOrEmpty(request.Value.ToString()))
            {
                logger.LogError("The authorization request was rejected because it contained " +
                                         "an unsupported parameter: {Parameter}.", "request");

                context.Reject(
                    error: OpenIdConnectConstants.Errors.RequestNotSupported,
                    description: "The request parameter is not supported.");

                return;
            }

            // Reject requests using the unsupported request_uri parameter.
            if (!string.IsNullOrEmpty(context.Request.RequestUri))
            {
                logger.LogError("The authorization request was rejected because it contained " +
                                         "an unsupported parameter: {Parameter}.", "request_uri");

                context.Reject(
                    error: OpenIdConnectConstants.Errors.RequestUriNotSupported,
                    description: "The request_uri parameter is not supported.");

                return;
            }

            // If a request_id parameter can be found in the authorization request,
            // restore the complete authorization request stored in the distributed cache.
            if (!string.IsNullOrEmpty(context.Request.RequestId))
            {
                var payload = await cache.GetAsync(AuthorizationRequestKey + context.Request.RequestId);
                if (payload == null)
                {
                    logger.LogError("The authorization request was rejected because an unknown " +
                                             "or invalid request_id parameter was specified.");

                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidRequest,
                        description: "Invalid request: timeout expired.");

                    return;
                }

                RestoreSerializedRequest(context, payload);
            }
        }

        private static void RestoreSerializedRequest(ExtractAuthorizationRequestContext context, byte[] payload)
        {
            var json = Encoding.UTF8.GetString(payload);
            var message = JsonConvert.DeserializeObject<OpenIdConnectRequest>(json);
            foreach (var (key, value) in message.GetParameters())
            {
                context.Request.SetParameter(key, value);
            }
        }


        /// <summary>
        /// Validates whether the client can redirect to the given uri
        /// </summary>
        public override async Task ValidateAuthorizationRequest(ValidateAuthorizationRequestContext context)
        {
            var query = new ClientRedirectUriValidator(context.ClientId, context.RedirectUri);
            var result = await ExecuteMessage(context, query);

            if (!result.Succeeded)
            {
                context.Reject("invalid_client", "Invalid redirect uri");
                return;
            }

            context.Validate();
        }

        /// <summary>
        /// Creates a valid authentication token used to create the access_token.
        /// </summary>
        private static AuthenticationTicket CreateAuthenticationTicket(Name userName, string userId, string[] roles, HandleTokenRequestContext context)
        {
            var identity = ClaimsIdentityFactory.Create(userName.Value, userId, context.Scheme.Name);

            foreach (var role in roles)
            {
                identity.AddClaim(new Claim(identity.RoleClaimType, role));
            }

            var properties = new AuthenticationProperties();
            var principal = new ClaimsPrincipal(new[] { identity });

            return CreateAuthenticationTicket(principal, properties, context.Options, context);
        }

        public override async Task HandleAuthorizationRequest(HandleAuthorizationRequestContext context)
        {
            var cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AuthorizationServerProvider>>();

            // If no request_id parameter can be found in the current request, assume the OpenID Connect request
            // was not serialized yet and store the entire payload in the distributed cache to make it easier
            // to flow across requests and internal/external authentication/registration workflows.
            if (string.IsNullOrEmpty(context.Request.RequestId))
            {
                // Generate a request identifier. Note: using a crypto-secure
                // random number generator is not necessary in this case.
                var identifier = Guid.NewGuid().ToString();

                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = context.Options.SystemClock.UtcNow + TimeSpan.FromMinutes(30),
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };

                // Store the serialized authorization request parameters in the distributed cache.
                var json = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(context.Request));
                await cache.SetAsync(AuthorizationRequestKey + identifier, json, options);

                context.Request.RequestId = identifier;
            }

            context.SkipHandler();
        }

        public override async Task HandleTokenRequest(HandleTokenRequestContext context)
        {
            if (context.Request.IsPasswordGrantType())
            {
                await GrantPassword(context);
            }
            else if (context.Request.IsRefreshTokenGrantType())
            {
                await GrantRefreshToken(context);
            }
        }

        /// <summary>
        /// Validates the userName and password provided by the user.
        /// </summary>
        private async Task GrantPassword(HandleTokenRequestContext context)
        {
            var clientValidator = new ClientValidator(context.Request.ClientId, context.Request.ClientSecret);
            var applicationResult = await ExecuteMessage(context, clientValidator);
            if (!applicationResult.Succeeded)
            {
                context.Reject("invalid_client", "Client application not validated");
                return;
            }

            var query = new UserEMailPasswordLoginCommand(
                new EMail(context.Request.Username), 
                new Password(context.Request.Password));
            var result = await ExecuteMessage(context, query);

            if (!result.Succeeded)
            {
                context.Reject("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            SetCorsHeader(context);

            var ticket = CreateAuthenticationTicket(result.DisplayName, result.UserId.ToString(), result.Roles, context);
            context.Validate(ticket);
        }

        /// <summary>
        /// Set cross-origin HTTP request (Cors) header to allow requests from a different domains. 
        /// This Cors value is specific to an Application and set by when validating the client application (ValidateClientAuthenticationp).
        /// </summary>
        private static void SetCorsHeader(HandleTokenRequestContext context)
        {
            const string key = "Access-Control-Allow-Origin";
            if (context.HttpContext.Items["as:clientAllowedOrigin"] is string allowedOrigin
             && context.HttpContext.Response.Headers.ContainsKey("key"))
            {
                context.HttpContext.Response.Headers.Add(key, new StringValues(allowedOrigin));
            }
        }

        /// <summary>
        /// Grant a new access_token based on the current refresh_token. Here we couldvalidate whether the 
        /// refresh token is still valid or revoked.
        /// </summary>
        private async Task GrantRefreshToken(HandleTokenRequestContext context)
        {
            var validator = new RefreshTokenValidator(
                context.Ticket.GetTokenId(),
                context.Request.ClientId,
                context.Ticket.Principal.GetClaim(ClaimTypes.NameIdentifier));

            var result = await ExecuteMessage(context, validator);
            if (!result.Succeeded)
            {
                context.Reject(OpenIdConnectConstants.Errors.InvalidRequest, "Could not validate refresh_token.");
                return;
            }

            var principal = new ClaimsPrincipal(context.Ticket.Principal);
            var ticket = CreateAuthenticationTicket(principal, context.Ticket.Properties, context.Options, context);

            context.Validate(ticket);
        }

        private static AuthenticationTicket CreateAuthenticationTicket<TOptions>(ClaimsPrincipal principal, AuthenticationProperties authenticationProperties, OpenIdConnectServerOptions options, BaseContext<TOptions> context)
            where TOptions : AuthenticationSchemeOptions
        {
            var configuration = Configuration(context);
            var ticket = new AuthenticationTicket(principal, authenticationProperties, context.Scheme.Name);
            ticket.SetResources(configuration.ApiHostName());
            ticket.SetScopes(
                OpenIdConnectConstants.Scopes.OpenId, 
                OpenIdConnectConstants.Scopes.Email,
                OpenIdConnectConstants.Scopes.Profile, 
                OpenIdConnectConstants.Scopes.OfflineAccess);
            return ticket;
        }

        public override Task ApplyTokenResponse(ApplyTokenResponseContext context)
        {
            AddCustomPropertiesTokenResponsePayload(context);
            return Task.FromResult(true);
        }

        private static void AddCustomPropertiesTokenResponsePayload(ApplyTokenResponseContext context)
        {
            foreach (var property in context.HttpContext.Items.Where(item => item.Key.ToString().StartsWith("as:")))
            {
                context.Response.AddProperty(property.Key as string, new JValue(property.Value));
            }
        }

        public override async Task SerializeRefreshToken(SerializeRefreshTokenContext context)
        {
            await StoreRefreshToken(context);
        }

        private async Task StoreRefreshToken(SerializeRefreshTokenContext context)
        {
            var principal = context.Ticket.Principal;
            var properties = context.Ticket.Properties;

            var command = new CreateRefreshTokenCommand(
                context.Ticket.GetTokenId(),
                context.Request.ClientId,
                principal.GetClaim(ClaimTypes.NameIdentifier),
                principal.GetClaim(ClaimTypes.Name),
                context.HttpContext.Connection.RemoteIpAddress?.ToString(),
                properties.IssuedUtc.GetValueOrDefault(),
                properties.ExpiresUtc.GetValueOrDefault());

            var result = await ExecuteMessage(context, command);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Could not store the refreshtoken");
            }
        }

        /// <summary>
        /// Validate whether the requested endpoint is the authentication endpoint. 
        /// This supports all end-points starting with AuthorizationEndpointPath (eg "/account/authorize"
        /// </summary>
        public override Task MatchEndpoint(MatchEndpointContext context)
        {
            if (context.Options.AuthorizationEndpointPath.HasValue
             && context.Request.Path.StartsWithSegments(context.Options.AuthorizationEndpointPath))
            {
                context.MatchAuthorizationEndpoint();
            }

            return Task.FromResult(true);
        }

        private static async Task<TResult> ExecuteMessage<TOptions, TResult>(BaseContext<TOptions> context, IRequest<TResult> message) 
            where TOptions : AuthenticationSchemeOptions
        {
            var messageDispatcher = context.HttpContext.RequestServices.GetMessageDispatcher();
            return await messageDispatcher.Send(message);
        }

        private static IConfiguration Configuration<TOptions>(BaseContext<TOptions> context)
            where TOptions : AuthenticationSchemeOptions
        {
            return context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
        }
    }
}