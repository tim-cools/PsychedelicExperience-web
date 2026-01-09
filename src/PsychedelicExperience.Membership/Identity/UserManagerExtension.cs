using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PsychedelicExperience.Common;
using ErrorCodes = PsychedelicExperience.Membership.Messages.ErrorCodes;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Membership.Services
{
    public static class UserManagerExtension
    {
        public static async Task<User> FindByEmailRequiredAsync(this UserManager<User> manager, string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new BusinessException(new ValidationError("UserId", ErrorCodes.UserNotFound, "Email should not be null."));
            }
            var user = await manager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new BusinessException(new ValidationError("UserId", ErrorCodes.UserNotFound, "User not found."));
            }
            return user;
        }
    }
}