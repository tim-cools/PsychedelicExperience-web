using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Marten;
using Marten.Linq;
using Marten.Services.Includes;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Security;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.TopicInteractions.Queries;

namespace PsychedelicExperience.Psychedelics.TopicInteractionView.Handlers
{
    public class GetTopicFollowersHandler : QueryHandler<GetTopicFollowers, TopicFollowersDetails>
    {
        public class GetFollowersQuery : ICompiledQuery<TopicInteraction>
        {
            private readonly Guid _topicId;

            public List<TopicFollower> TopicId { get; set; }

            public GetFollowersQuery(Guid topicId)
            {
                _topicId = topicId;
            }

            public Expression<Func<IQueryable<TopicInteraction>, TopicInteraction>> QueryIs()
            {
                return query => query
                    .Include<TopicInteraction, TopicFollower>(topic => topic.Id, follower => follower.TopicId, JoinType.Inner)
                    .Single(x => x.Id == _topicId);
            }
        }

        private readonly IUserInfoResolver _userInfoResolver;

        public GetTopicFollowersHandler(IQuerySession session, IUserInfoResolver userInfoResolver, IUserDataProtector userDataProtector) : base(session)
        {
            _userInfoResolver = userInfoResolver;
        }

        protected override async Task<TopicFollowersDetails> Execute(GetTopicFollowers query)
        {
            var user = await Session.LoadUserAsync(query.UserId);

            var getFollowersQuery = new GetFollowersQuery((Guid) query.TopicId);
            var topicInteraction = await Session.QueryAsync(getFollowersQuery);

            var permissions = topicInteraction?.GetPermissions(user);

            return topicInteraction.MapFollowers(user, permissions, getFollowersQuery.TopicId, _userInfoResolver);
        }
    }
}