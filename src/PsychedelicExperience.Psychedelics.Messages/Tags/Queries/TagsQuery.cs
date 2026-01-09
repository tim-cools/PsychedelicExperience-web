using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Tags.Queries
{
    public class TagsQuery : IRequest<Tag[]>
    {
        public UserId UserId { get; }
        public TagsDomain Domain { get; }
        public string QueryString { get; }

        public TagsQuery(UserId userId, string queryString, TagsDomain domain)
        {
            UserId = userId;
            QueryString = queryString;
            Domain = domain;
        }

        public bool QueryStringEmpty()
        {
            return string.IsNullOrEmpty(QueryString);
        }
    }
}