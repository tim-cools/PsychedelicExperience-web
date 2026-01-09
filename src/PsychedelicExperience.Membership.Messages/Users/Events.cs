using System;
using PsychedelicExperience.Common.Aggregates;

namespace PsychedelicExperience.Membership.Messages.Users
{
    public class UserRegistered : Event
    {
        public Guid Id { get; set; }
        public UserId CreatorId { get; set; }
        public LoginType LoginType { get; set; }
        public Name DisplayName { get; set; }
        public Name FullName { get; set; }
        public EMail EMail { get; set; }

        public UserRegistered(UserId id, UserId creatorId, LoginType loginType, Name fullName, Name displayName, EMail eMail, DateTime eventTimestamp)
        {
            Id = id.Value;
            CreatorId = creatorId;
            LoginType = loginType;
            FullName = fullName;
            DisplayName = displayName;
            EMail = eMail;
            EventTimestamp = eventTimestamp;
        }
    }

    public enum LoginType
    {
        UserName,
        Facebook,
        Google,
        Unknown
    }

    public class UserToRoleAdded : Event
    {
        public UserId RequesterId { get; set; }
        public UserId UserToChangeId { get; set; }
        public Role Role { get; set; }
    }

    public class UserFromRoleRemoved : Event
    {
        public UserId RequesterId { get; set; }
        public UserId UserToChangeId { get; set; }
        public Role Role { get; set; }
    }

    public class UserEmailConfirmed : Event
    {
        public UserId UserId { get; set; }
    }

    public class UserJoinedExperiencesBeta : Event
    {
        public UserId UserId { get; set; }
    }
}