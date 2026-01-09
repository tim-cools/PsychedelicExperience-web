using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Membership.Users.Domain
{
    public class UserAvatar
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public byte[] Data { get; set; }
    }
}