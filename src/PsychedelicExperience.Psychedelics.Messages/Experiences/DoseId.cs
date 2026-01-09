using System;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Experiences
{
    public class DoseId : Id
    {
        public DoseId(Guid value) : base(value)
        {
        }

        public static DoseId New()
        {
            return new DoseId(CombGuidIdGeneration.NewGuid());
        }

        public static explicit operator DoseId(Guid id)
        {
            return new DoseId(id);
        }
    }
}