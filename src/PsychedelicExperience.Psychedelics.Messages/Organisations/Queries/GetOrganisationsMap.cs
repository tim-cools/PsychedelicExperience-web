using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class GetOrganisationsMap : IRequest<OrganisationMapPoint[]>
    {
        public UserId UserId { get; }
        public string[] Tags { get; }
        public string Country { get; }

        public GetOrganisationsMap(UserId userId, string country = null, string[] tags = null)
        {
            UserId = userId;
            Country = country;
            Tags = tags;
        }
    }
}