using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Security;
using PsychedelicExperience.Membership.Clients.Domain;
using PsychedelicExperience.Membership.Messages.Clients;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using ErrorCodes = PsychedelicExperience.Membership.Messages.ErrorCodes;

namespace PsychedelicExperience.Membership.Clients.Handlers
{
    public class ClientValidatorHandler : QueryHandler<ClientValidator, ValidateClientResult>
    {
        public ClientValidatorHandler(IQuerySession session)
              : base(session)
        {
        }

        protected override Task<ValidateClientResult> Execute(ClientValidator query)
        {
            return Task.FromResult(GetResult(query));
        }

        private ValidateClientResult GetResult(ClientValidator query)
        {
            var client = Session.Query<Domain.Client>().FirstOrDefault(criteria => criteria.Key == query.ClientId);
            if (client == null)
            {
                return ValidateClientResult.Failed(new ValidationError("ClientId", ErrorCodes.ClientNotRegistered, $"Client '{query.ClientId}' is not registered in the system."));
            }

            if (client.ApplicationType == ApplicationTypes.NativeConfidential)
            {
                if (string.IsNullOrWhiteSpace(query.ClientSecret))
                {
                    return ValidateClientResult.Failed(new ValidationError("ClientId", Messages.ErrorCodes.ClientAuthicationFailed, "Client secret should be sent."));
                }
                if (client.Secret != Hasher.ComputeSHA256(query.ClientSecret))
                {
                    return ValidateClientResult.Failed(new ValidationError("ClientId", Messages.ErrorCodes.ClientAuthicationFailed, "Client secret is invalid."));
                }
            }

            if (!client.Active)
            {
                return ValidateClientResult.Failed(new ValidationError("ClientId", Messages.ErrorCodes.ClientInactive, "Client is inactive."));
            }

            return new ValidateClientResult(true, client.Id, client.Name, client.AllowedOrigin, client.RedirectUri);
        }
    }
}