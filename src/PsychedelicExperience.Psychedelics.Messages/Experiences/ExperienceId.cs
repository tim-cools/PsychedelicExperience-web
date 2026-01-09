using System;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences
{
    public class ExperienceId : Id
    {
        public ExperienceId(Guid value) : base(value)
        {
        }

        public static ExperienceId New()
        {
            return new ExperienceId(CombGuidIdGeneration.NewGuid());
        }

        public static explicit operator ExperienceId(Guid id)
        {
            return new ExperienceId(id);
        }
    }
}