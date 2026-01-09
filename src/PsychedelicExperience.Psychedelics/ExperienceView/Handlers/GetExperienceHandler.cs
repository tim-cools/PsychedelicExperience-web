using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Security;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;

namespace PsychedelicExperience.Psychedelics.ExperienceView.Handlers
{
    public class GetExperienceHandler : QueryHandler<GetExperience, ExperienceDetails>
    {
        private readonly IUserInfoResolver _userInfoResolver;
        private readonly IUserDataProtector _userDataProtector;

        public GetExperienceHandler(IQuerySession session, IUserInfoResolver userInfoResolver, IUserDataProtector userDataProtector) : base(session)
        {
            _userInfoResolver = userInfoResolver;
            _userDataProtector = userDataProtector;
        }

        protected override async Task<ExperienceDetails> Execute(GetExperience query)
        {
            var experience = await Session.LoadAsync<Experience>(query.ExperienceId);
            var user = await Session.LoadUserAsync(query.UserId);

            return experience?.MapDetails(user, _userInfoResolver, _userDataProtector);
        }
    }
}
