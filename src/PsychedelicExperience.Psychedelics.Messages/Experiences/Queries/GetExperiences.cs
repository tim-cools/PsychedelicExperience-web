using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences.Queries
{
    public class GetExperiences : IRequest<ExperiencesResult>
    {
        public UserId UserId { get; }
        public string Query { get; }
        public string[] Substances { get; }
        public string[] Tags { get; }
        public int Page { get; }
        public bool FilterByUser { get; set; }

        public GetExperiences(UserId userId, string query = null, string[] substances = null, string[] tags = null, int page = 0, bool filterByUser = false)
        {
            UserId = userId;
            Substances = substances;
            Query = query;
            Tags = tags;
            Page = page;
            FilterByUser = filterByUser;
        }
    }
}