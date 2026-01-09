using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Membership.Users.Handlers
{
    internal static class Mapping
    {
        internal static User Map(this Domain.User result)
        {
            return new User
            {
                Id = result.Id,
                Name = result.DisplayName,
                Confirmed = result.EmailConfirmed
            };
        }
    }
}