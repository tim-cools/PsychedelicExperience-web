using System;

namespace PsychedelicExperience.Membership.Users.Domain
{
    public class Role
    {        
        public Guid Id { get; set; }

        public string Name { get; set; }
        public int Level { get; set; }

        public Role(string name, int level)
        {
            Name = name;
            Level = level;
        }

        public bool IsAllowedBy(string ownRole)
        {
            var role = Roles.Get(ownRole);
            return role != null && role.Level <= Level;
        }

        public static implicit operator string(Role role)
        {
            return role.Name;
        }
    }
}