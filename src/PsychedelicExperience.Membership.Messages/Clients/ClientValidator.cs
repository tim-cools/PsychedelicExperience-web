using PsychedelicExperience.Common.Messages;

namespace PsychedelicExperience.Membership.Messages.Clients
{
    public class ClientValidator : IRequest<ValidateClientResult>
    {
        public string ClientId { get; }
        public string ClientSecret { get; }

        public ClientValidator(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }
    }
}