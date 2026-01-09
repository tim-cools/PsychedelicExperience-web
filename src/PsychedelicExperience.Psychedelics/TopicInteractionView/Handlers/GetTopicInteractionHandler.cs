using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Security;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.TopicInteractions.Queries;
using PsychedelicExperience.Psychedelics.UserInteractions;

namespace PsychedelicExperience.Psychedelics.TopicInteractionView.Handlers
{
    public class GetTopicInteractionHandler : QueryHandler<GetTopicInteraction, TopicInteractionDetails>
    {
        private readonly IUserInfoResolver _userInfoResolver;
        private readonly IUserDataProtector _userDataProtector;

        public GetTopicInteractionHandler(IQuerySession session, IUserInfoResolver userInfoResolver, IUserDataProtector userDataProtector) : base(session)
        {
            _userInfoResolver = userInfoResolver;
            _userDataProtector = userDataProtector;
        }

        protected override async Task<TopicInteractionDetails> Execute(GetTopicInteraction query)
        {
            var user = await Session.LoadUserAsync(query.UserId);

            var topicInteraction = await Session.LoadAsync<TopicInteraction>(query.TopicId);
            var permissions = topicInteraction?.GetPermissions(user);

            var userInteraction = await GetUserInteraction(query);

            return topicInteraction.MapDetails(user, permissions, userInteraction, _userInfoResolver, _userDataProtector);
        }

        private async Task<UserInteraction> GetUserInteraction(GetTopicInteraction query)
        {
            if (query.UserId == null) return null;

            return  await Session.Query<UserInteraction>()
                    .FirstOrDefaultAsync(where => where.TopicId == query.TopicId.Value
                                               && where.UserId == query.UserId.Value);
        }
    }
}
