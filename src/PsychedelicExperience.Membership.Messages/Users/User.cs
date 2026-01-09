
using System;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Membership.Messages.Users
{
    public class UserId : Id
    {
        public UserId(Guid value) : base(value)
        {
        }

        public static UserId New()
        {
            return new UserId(CombGuidIdGeneration.NewGuid());
        }

        public static explicit operator UserId(Guid id)
        {
            return new UserId(id);
        }
    }

    public class User
    {
        public ShortGuid Id { get; set; }
        public string Name { get; set; }
        public bool Confirmed { get; set; }
    }
}