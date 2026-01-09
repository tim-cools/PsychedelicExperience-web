using Marten;
using PsychedelicExperience.Membership.Messages.Users;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Membership.UserInfo
{
    public class UserInfoResolver : IUserInfoResolver
    {
        private readonly IQuerySession _querySession;

        public UserInfoResolver(IQuerySession querySession)
        {
            _querySession = querySession;
        }

        //warning big N+1 problem
        //todo this should come from in-memory instead of from db
        public UserInfo GetInfo(UserId userId)
        {
            var user = userId != null 
                ? _querySession.Load<User>(userId.Value) 
                : null;

            return user != null 
                ? new UserInfo(user.Id, user.DisplayName)
                : new UserInfo(null, "unknown");
        }
    }
}