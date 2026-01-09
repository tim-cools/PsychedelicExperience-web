using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Clients;

namespace PsychedelicExperience.Membership.Clients.Handlers
{
    public class GetClientsHandler : QueryHandler<GetClients, Client[]>
    {
        public GetClientsHandler(IQuerySession session)
              : base(session)
        {
        }

        protected override Task<Client[]> Execute(GetClients query)
        {
            return Task.FromResult(GetResult(query));
        }

        private Client[] GetResult(GetClients query)
        {
            return Session.Query<Domain.Client>()
                .ToArray()
                .Select(client => new Client {
                    Id = client.Id,
                    Active = client.Active,
                    Name = client.Name,
                    AllowedOrigin = client.AllowedOrigin,
                    Key = client.Key,
                    RedirectUri = client.RedirectUri,
                })
                .ToArray();
        }
    }
}