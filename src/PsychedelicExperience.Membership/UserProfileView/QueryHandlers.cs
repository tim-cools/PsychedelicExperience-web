using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.UserProfiles;

namespace PsychedelicExperience.Membership.UserProfileView
{
    public class UserProfileByIdHandler : QueryHandler<UserProfileByIdQuery, UserProfileDetails>
    {
        public UserProfileByIdHandler(IQuerySession session) : base(session)
        {
        }

        protected override async Task<UserProfileDetails> Execute(UserProfileByIdQuery query)
        {
            var id = (Guid)query.UserProfileId;

            var organisation = await Session.LoadAsync<UserProfile>(id);
            var user = query.UserId != null ? await Session.LoadUserAsync(query.UserId) : null;

            return organisation?.MapDetails(user);
        }
    }

    public class UserProfileByEmailHandler : QueryHandler<UserProfileByEMailQuery, UserProfileDetails>
    {
        public UserProfileByEmailHandler(IQuerySession session) : base(session)
        {
        }

        protected override async Task<UserProfileDetails> Execute(UserProfileByEMailQuery query)
        {
            var email = query.EMail.Value.NormalizeForSearch();

            var organisation = Session.Query<UserProfile>()
                .SingleOrDefault(where => where.EMail == email);

            var user = await Session.LoadUserAsync(query.UserId);

            return organisation?.MapDetails(user);
        }
    }

    public class UserProfileAvatarHandler : QueryHandler<UserProfileAvatarQuery, AvatarDetails>
    {
        public UserProfileAvatarHandler(IQuerySession session) : base(session)
        {
        }

        protected override async Task<AvatarDetails> Execute(UserProfileAvatarQuery query)
        {
            var userProfile = await Session.LoadAsync<UserProfile>(query.UserProfileId);
            return userProfile?.Avatar != null
                ? new AvatarDetails
                {
                    FileName = userProfile.Avatar.FileName,
                    Id = userProfile.Avatar.AvatarId
                }
                : null;
        }
    }
}