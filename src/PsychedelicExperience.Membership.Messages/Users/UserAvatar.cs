using System;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Membership.Messages.Users
{
    public class AvatarId : Id
    {
        public AvatarId(Guid value) : base(value)
        {
        }

        public static AvatarId New()
        {
            return new AvatarId(CombGuidIdGeneration.NewGuid());
        }

        public static explicit operator AvatarId(Guid id)
        {
            return new AvatarId(id);
        }
    }

    public class UserAvatar
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public byte[] Data { get; set; }
    }
}