using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Membership.UserInfo
{
    public interface IUserInfoResolver
    {
        UserInfo GetInfo(UserId userId);
    }
}