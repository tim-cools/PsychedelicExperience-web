using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Clients;
using PsychedelicExperience.Membership.Messages.Users;
using ErrorCodes = PsychedelicExperience.Membership.Messages.ErrorCodes;

namespace PsychedelicExperience.Membership.Clients.Handlers
{
    public class ClientRedirectUriValidatorHandler : QueryHandler<ClientRedirectUriValidator, Result>
    {
        public ClientRedirectUriValidatorHandler(IQuerySession session)
              : base(session)
        {
        }

        protected override Task<Result> Execute(ClientRedirectUriValidator query)
        {
            return Task.FromResult(GetResult(query));
        }

        private Result GetResult(ClientRedirectUriValidator query)
        {
            var client = Session.Query<Domain.Client>().FirstOrDefault(criteria => criteria.Key == query.ClientId);
            if (client == null)
            {
                return Result.Failed("clientId", ErrorCodes.ClientNotRegistered, $"Client '{query.ClientId}' is not registered in the system.");
            }

            if (client.RedirectUri != "*" && !client.RedirectUri.StartsWith(query.RedirectUri))
            {
                return Result.Failed("redirectUri", Messages.ErrorCodes.RedirectUriInvalid, $"Invalid redirect uri '{query.RedirectUri} for client '{query.ClientId}'");
            }

            if (!client.Active)
            {
                return Result.Failed("clientId", Messages.ErrorCodes.ClientInactive, "Client is inactive.");
            }

            return Result.Success;
        }
    }
}