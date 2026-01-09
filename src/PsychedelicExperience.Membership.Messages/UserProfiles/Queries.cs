using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Membership.Messages.UserProfiles
{
    public class UserProfileByIdQuery : IRequest<UserProfileDetails>
    {
        public UserId UserId { get; }
        public UserProfileId UserProfileId { get; }

        public UserProfileByIdQuery(UserProfileId userProfileId, UserId userId)
        {
            UserProfileId = userProfileId;
            UserId = userId;
        }
    }

    public class UserProfileByEMailQuery : IRequest<UserProfileDetails>
    {
        public UserId UserId { get; }
        public EMail EMail { get; }

        public UserProfileByEMailQuery(UserId userId, EMail email)
        {
            EMail = email;
            UserId = userId;
        }
    }

    public class UserProfileAvatarQuery : IRequest<AvatarDetails>
    {
        public UserProfileId UserProfileId { get; }

        public UserProfileAvatarQuery(UserProfileId userProfileId)
        {
            UserProfileId = userProfileId;
        }
    }

    public class AvatarDetails
    {
        public ShortGuid Id { get; set; }
        public string FileName { get; set; }
    }
}