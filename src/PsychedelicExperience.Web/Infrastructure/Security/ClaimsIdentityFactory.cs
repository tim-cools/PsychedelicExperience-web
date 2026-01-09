using System.Security.Claims;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;

namespace PsychedelicExperience.Web.Infrastructure.Security
{
    internal static class ClaimsIdentityFactory
    {
        public static ClaimsIdentity Create(string name, string userId, string authenticationType)
        {
            var identity = new ClaimsIdentity(authenticationType);
            identity.AddClaim(OpenIdConnectConstants.Claims.Name, name,
                OpenIdConnectConstants.Destinations.AccessToken,
                OpenIdConnectConstants.Destinations.IdentityToken);

            identity.AddClaim(OpenIdConnectConstants.Claims.Subject, userId,
                OpenIdConnectConstants.Destinations.AccessToken,
                OpenIdConnectConstants.Destinations.IdentityToken);
            return identity;
        }
    }
}