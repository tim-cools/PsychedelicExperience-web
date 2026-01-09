using System;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.TopicInteractions
{
    public class UserInteractionId : Id
    {
        public UserInteractionId(Guid value) : base(value)
        {
        }

        public static UserInteractionId New()
        {
            return new UserInteractionId(CombGuidIdGeneration.NewGuid());
        }

        public static explicit operator UserInteractionId(Guid id)
        {
            return new UserInteractionId(id);
        }
    }
}