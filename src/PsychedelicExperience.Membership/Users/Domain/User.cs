using System;
using System.Collections.Generic;
using System.Linq;
using Marten.Schema.Identity;
using PsychedelicExperience.Membership.Security;

namespace PsychedelicExperience.Membership.Users.Domain
{
    public class User
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }
        public EncryptionKey EncryptionKey { get; set; }
        public bool EmailConfirmed { get; set; }
        public string NormalizedEmail { get; set; }

        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        public IList<string> Roles { get; set; }

        public User()
        {
            Roles = new List<string>();
        }

        public User(string displayName, string eMail) : this()
        {
            Id = CombGuidIdGeneration.NewGuid();
            UserName = Id.ToString();
            DisplayName = displayName;
            Email = eMail;
            EncryptionKey = EncryptionKey.New();
        }

        public bool IsAdministrator()
        {
            return Roles.Contains(Domain.Roles.Administrator);
        }

        public bool IsAtLeast(Role role)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));

            return Roles.Any(role.IsAllowedBy);
        }

        public bool IsDisabled()
        {
            return LockoutEnabled;
        }

        public bool EnsureKey()
        {
            if (EncryptionKey != null) return false;

            EncryptionKey = EncryptionKey.New();
            return true;
        }

        public bool HasEmail(string email) 
            => email?.ToLowerInvariant() == Email.ToLowerInvariant();
    }
}