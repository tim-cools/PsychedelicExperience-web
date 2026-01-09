using System.Collections.Generic;

namespace PsychedelicExperience.Membership.Users.Domain
{
    public class RoleNames
    {
        public const string Administrator = "administrator";
        public const string ContentManager = "contentmanager";
    }

    public class Roles
    {
        private static readonly IDictionary<string, Role> _roles = new Dictionary<string, Role>();

        public static readonly Role Administrator = AddRole(RoleNames.Administrator, 1);
        public static readonly Role ContentManager = AddRole(RoleNames.ContentManager, 3);

        private static Role AddRole(string name, int level)
        {
            var role = new Role(name, level);
            _roles.Add(name.ToLowerInvariant(), role);
            return role;
        }

        public static Role Get(string ownRole)
        {
            return _roles.TryGetValue(ownRole.ToLowerInvariant(), out var role) ? role : null;
        }
    }
}