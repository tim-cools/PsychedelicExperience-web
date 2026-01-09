using System;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.TopicInteractions
{
    public class TopicId : Id
    {
        public TopicId(Guid value) : base(value)
        {
        }

        public static TopicId New()
        {
            return new TopicId(CombGuidIdGeneration.NewGuid());
        }

        public static explicit operator TopicId(Guid id)
        {
            return new TopicId(id);
        }
    }
}