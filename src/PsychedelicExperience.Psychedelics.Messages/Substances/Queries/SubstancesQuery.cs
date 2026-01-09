using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Substances.Queries
{
    public class SubstancesQuery : IRequest<SubstanceResult>
    {
        public UserId UserId { get; }
        public string QueryString { get; }

        public SubstancesQuery(UserId userId, string queryString)
        {
            UserId = userId;
            QueryString = queryString;
        }

        public bool QueryStringEmpty()
        {
            return string.IsNullOrEmpty(QueryString);
        }
    }
}