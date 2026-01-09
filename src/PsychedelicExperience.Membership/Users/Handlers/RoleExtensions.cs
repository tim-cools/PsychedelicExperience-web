using System;
using System.Collections.Generic;
using PsychedelicExperience.Membership.Users.Domain;
using Role = PsychedelicExperience.Membership.Messages.Users.Role;

namespace PsychedelicExperience.Membership.Users.Handlers
{
    public static class RoleExtensions
    {
        private static readonly Dictionary<Role, string> _mapping = new Dictionary<Role, string>
        {
            { Role.ContentManager, Roles.ContentManager.Name },
            { Role.Administrator, Roles.Administrator.Name }
        };

        public static string GetName(this Role commandRole)
        {
            return _mapping.TryGetValue(commandRole, out var result)
                ? result
                : throw new ArgumentOutOfRangeException(nameof(commandRole), commandRole, null);
        }
    }
}