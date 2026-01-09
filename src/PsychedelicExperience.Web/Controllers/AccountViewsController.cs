using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Clients;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Web.Infrastructure;
using PsychedelicExperience.Web.Infrastructure.Security;
using AuthenticationProperties = Microsoft.AspNetCore.Authentication.AuthenticationProperties;

namespace PsychedelicExperience.Web.Controllers
{
    [ApiExplorerSettings(GroupName = "Views")]
    public class AccountViewsController : ViewController
    {        
        private readonly ILogger<AccountViewsController> _logger;
        private readonly IMediator _messageDispatcher;
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

        public AccountViewsController(IMediator messageDispatcher, IConfiguration configuration, 
            IAuthenticationSchemeProvider authenticationSchemeProvider, 
            ILogger<AccountViewsController> logger) : base(messageDispatcher, configuration)
        {
            _messageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));
            _authenticationSchemeProvider = authenticationSchemeProvider ?? throw new ArgumentNullException(nameof(authenticationSchemeProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("~/register")]
        public IActionResult Register()
        {
            return ViewWithState();
        }

        [HttpGet("~/register/e")]
        public IActionResult RegisterE()
        {
            return ViewWithState();
        }

        [HttpGet("~/logon")]
        public IActionResult Logon()
        {
            return ViewWithState();
        }

        [HttpGet("~/account/authorize/connect")]
        public async Task<ActionResult> Connect(string provider)
        {
            if (await ProviderNotSupported(provider))
            {
                return InvalidRequest($"An internal error has occurred (provider not supported '{provider}')");
            }

            var response = HttpContext.GetOpenIdConnectResponse();
            if (response != null)
            {
                return BadRequest(response);
            }

            var request = HttpContext.GetOpenIdConnectRequest();
            if (request == null)
            {
                return InvalidRequest("An internal error has occurred");
            }

            var redirectUri = "/account/authorize/complete?request_id=" + request.RequestId;
            if (User.Identities.Any(identity => identity.IsAuthenticated))
            {
                return Redirect(redirectUri);
            }

            return RedirectToProviderAuthtication(provider, redirectUri);
        }

        private async Task<bool> ProviderNotSupported(string provider)
        {
            return string.IsNullOrEmpty(provider) || !await _authenticationSchemeProvider.IsProviderSupported(provider);
        }

        private static ChallengeResult RedirectToProviderAuthtication(string provider, string redirectUri)
        {
            var properties = new AuthenticationProperties { RedirectUri = redirectUri };
            return new ChallengeResult(provider, properties);
        }

        [HttpGet("~/account/authorize/complete")]
        public async Task<ActionResult> Complete(CancellationToken cancellationToken)
        {
            var request = HttpContext.GetOpenIdConnectRequest();
            if (request == null)
            {
                return InvalidRequest("An internal error has occurred (No OpenIdConnectRequest)");
            }

            if (!User.Claims.Any())
            {
                return InvalidRequest("An internal error has occurred (No Claims)");
            }

            var query = new ClientValidator(request.ClientId, request.ClientSecret);
            var applicationResult = await _messageDispatcher.Send(query);
            if (!applicationResult.Succeeded)
            {
                return InvalidRequest("invalid_client", "Client application not validated"); 
            }

            var type = User.Identity.AuthenticationType;
            var userName = new Name(User.FindFirstValue(ClaimTypes.Name));
            var externalIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = new EMail(User.FindFirstValue(ClaimTypes.Email));

            var command = new ExternalLoginCommand(
                type, userName, externalIdentifier, email);
            var result = await _messageDispatcher.Send(command);
            if (!result.Succeeded)
            {
                return InvalidRequest("Could not login external");
            }

            var principal = CreateClaimsPrincipal(userName.Value, result.UserId.ToString(), applicationResult.Name, applicationResult.Id.ToString());
            var properties = CreateAuthenticationProperties(principal);

            await HttpContext.SignInAsync(OpenIdConnectServerDefaults.AuthenticationScheme, principal, properties);

            return new EmptyResult();
        }

        private AuthenticationProperties CreateAuthenticationProperties(ClaimsPrincipal principal)
        {
            var ticket = new AuthenticationTicket(principal, null, OpenIdConnectServerDefaults.AuthenticationScheme);
            ticket.SetScopes(OpenIdConnectConstants.Scopes.OpenId, OpenIdConnectConstants.Scopes.Email, OpenIdConnectConstants.Scopes.Profile, OpenIdConnectConstants.Scopes.OfflineAccess);
            ticket.SetResources(Configuration.WebHostName());
            return ticket.Properties;
        }

        private ClaimsPrincipal CreateClaimsPrincipal(string userName, string userId, string clientName, string clientId)
        {
            var identity = ClaimsIdentityFactory.Create(userName, userId, OpenIdConnectServerDefaults.AuthenticationScheme);
            identity.Actor = ClaimsIdentityFactory.Create(clientName, clientId, OpenIdConnectServerDefaults.AuthenticationScheme);

            return new ClaimsPrincipal(identity);
        }

        //Authorization token is pushed to main window in JS (see view)
        [HttpGet("~/account/authorized")]
        public ActionResult Authorized() => View("js-{auto}");

        [HttpGet("~/account/reset-password")]
        public ActionResult ResetPassword() => View("js-{auto}");

        [HttpGet("~/account/confirm")]
        public async Task<ActionResult> Confirm(UserId userId, string token)
        {
            var command = new ConfirmEmailCommand(token, userId);
            var result = await _messageDispatcher.Send(command);

            return View("js-{auto}", new { succeed = result.Succeeded });
        }

        private BadRequestObjectResult InvalidRequest(string message)
        {
            return BadRequest(new
            {
                Error = "invalid_request",
                ErrorDescription = message
            });
        }

        private BadRequestObjectResult InvalidRequest(string error, string message)
        {
            return BadRequest(new
            {
                Error = error,
                ErrorDescription = message
            });
        }
    }
}