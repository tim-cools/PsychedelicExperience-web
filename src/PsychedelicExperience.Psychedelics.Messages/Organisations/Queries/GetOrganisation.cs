using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Queries;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class GetOrganisation : IRequest<OrganisationDetails>
    {
        public UserId UserId { get; }
        public OrganisationId OrganisationId { get; }

        public GetOrganisation(UserId userId, OrganisationId organisationId)
        {
            UserId = userId;
            OrganisationId = organisationId;
        }
    }
    
    public class GetOrganisationCountries : IRequest<Country[]>
    {
        public UserId UserId { get; }
        public string QueryString { get; }
 
        public GetOrganisationCountries(UserId userId, string queryString)
        {
            UserId = userId;
            QueryString = queryString;
        }
 
        public bool QueryStringEmpty()
        {
            return string.IsNullOrEmpty(QueryString);
        }
    }

    public class Country
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
