using System;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership.Messages.UserProfiles;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Membership.UserProfileView
{
    internal static class Mapping
    {
        internal static UserProfileSummary MapSummary(this UserProfile userProfile, User user)
        {
            if (userProfile == null) throw new ArgumentNullException(nameof(userProfile));

            return new UserProfileSummary
            {
                UserProfileId = userProfile.Id,
                DisplayName = userProfile.DisplayName
            };
        }

        internal static UserProfileDetails MapDetails(this UserProfile userProfile, User user)
        {
            if (userProfile == null) throw new ArgumentNullException(nameof(userProfile));

            if (!userProfile.CanView(user))
            {
                throw new BusinessException("UserProfile not available.");
            }

            var isOwner = userProfile.Is(user);
            var privileges = new UserProfilePrivileges
            {
                IsAdministrator = user != null && user.IsAdministrator(),
                Editable = isOwner || user != null && user.IsAdministrator(),
                IsOwner = isOwner
            };

            return new UserProfileDetails
            {
                UserProfileId = userProfile.Id,
                DisplayName = userProfile.DisplayName,
                EmailConfirmed = userProfile.EmailConfirmed,
                FullName = privileges.Editable ? userProfile.FullName : null,
                EMail = privileges.Editable ? userProfile.EMail : null,
                NotificationEmail = privileges.Editable ? MapNotificationEmail(userProfile) : null,
                TagLine = userProfile.TagLine,
                Description = userProfile.Description,
                Privileges = privileges,
                Roles = MapRoles(userProfile, privileges)
            };
        }

        private static Messages.UserProfiles.NotificationEmail MapNotificationEmail(UserProfile user)
        {
            return user.NotificationEmail != null
                ? new Messages.UserProfiles.NotificationEmail
                {
                    Enabled = user.NotificationEmail.Enabled,
                    Minutes = (int) user.NotificationEmail.Interval.TotalMinutes
                } : new Messages.UserProfiles.NotificationEmail
                {
                    Enabled = true,
                    Minutes = (int) TimeSpan.FromDays(1).TotalMinutes
                };
        }

        private static string[] MapRoles(UserProfile userProfile, UserProfilePrivileges privileges)
        {
            return privileges.IsAdministrator ? userProfile.Roles.Select(role => role.ToString()).ToArray() : null;
        }
    }
}