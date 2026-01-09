using System;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates
{
    public class OrganisationUpdateId : Id
    {
        public OrganisationUpdateId(Guid value) : base(value)
        {
        }

        public static OrganisationUpdateId New()
        {
            return new OrganisationUpdateId(CombGuidIdGeneration.NewGuid());
        }

        public static explicit operator OrganisationUpdateId(Guid id)
        {
            return new OrganisationUpdateId(id);
        }
    }
}