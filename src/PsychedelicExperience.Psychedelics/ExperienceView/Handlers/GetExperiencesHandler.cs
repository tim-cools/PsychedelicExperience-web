using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;

namespace PsychedelicExperience.Psychedelics.ExperienceView.Handlers
{
    public class GetExperiencesHandler : QueryHandler<GetExperiences, ExperiencesResult>
    {
        private readonly IUserInfoResolver _userInfoResolver;

        public GetExperiencesHandler(IQuerySession session, IUserInfoResolver userInfoResolver) : base(session)
        {
            _userInfoResolver = userInfoResolver;
        }

        protected override async Task<ExperiencesResult> Execute(GetExperiences getExperiencesQuery)
        {
            var user = getExperiencesQuery.UserId != null ? await Session.LoadUserAsync(getExperiencesQuery.UserId) : null;

            var query = new GetExperiencesQueryBuilder(Session, _userInfoResolver)
                .WithPrivacy(getExperiencesQuery.UserId)
                .FilterByUser(getExperiencesQuery.FilterByUser, getExperiencesQuery.UserId)
                .FilterQueryString(getExperiencesQuery.Query)
                .FilterSubstance(getExperiencesQuery.Substances)
                .FilterTags(getExperiencesQuery.Tags)
                .Paging(getExperiencesQuery.Page);

            return await query.Execute();
        }
    }
}
