using System.Threading.Tasks;
using Marten;
using Microsoft.AspNetCore.Identity;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Membership.Users.Handlers
{
    public class QueryHandler : QueryHandler<UserByIdQuery, User>
    {

        public QueryHandler( IDocumentSession session)
              : base(session)
        {
        }

        protected override async Task<User> Execute(UserByIdQuery query)
        {
            var userId = query.Id.Value;
            var result = await Session.LoadAsync<Domain.User>(userId);

            return result?.Map();
        }
    }

    public class UserAvatarByIdQueryHandler : QueryHandler<UserAvatarByIdQuery, UserAvatar>
    {
        public UserAvatarByIdQueryHandler(IDocumentSession session)
              : base(session)
        {
        }

        protected override async Task<UserAvatar> Execute(UserAvatarByIdQuery query)
        {
            var result = await Session.LoadAsync<Domain.UserAvatar>(query.Id);
            return result != null ? Map(result) : null;
        }

        private UserAvatar Map(Domain.UserAvatar result)
        {
            return new UserAvatar
            {
                Id = result.Id,
                UserId = result.UserId,
                Data = result.Data
            };
        }
    }

    public class UserByEMailHandler : QueryHandler<UserByEMailQuery, User>
    {
        private readonly ILookupNormalizer _lookupNormalizer;

        public UserByEMailHandler(IDocumentSession session, ILookupNormalizer lookupNormalizer)
              : base(session)
        {
            _lookupNormalizer = lookupNormalizer;
        }

        protected override async Task<User> Execute(UserByEMailQuery query)
        {
            var email = _lookupNormalizer.Normalize(query.EMail.Value);

            var result = await Session
                .Query<Domain.User>()
                .SingleOrDefaultAsync(where => where.NormalizedEmail == email);

            return result?.Map();
        }
    }
}