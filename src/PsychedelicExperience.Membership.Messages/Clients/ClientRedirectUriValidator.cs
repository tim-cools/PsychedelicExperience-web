using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Membership.Messages.Clients
{
    public class ClientRedirectUriValidator : IRequest<Result>
    {
        public string ClientId { get; }
        public string RedirectUri { get; }

        public ClientRedirectUriValidator(string clientId, string redirectUri)
        {
            ClientId = clientId;
            RedirectUri = redirectUri;
        }
    }
}