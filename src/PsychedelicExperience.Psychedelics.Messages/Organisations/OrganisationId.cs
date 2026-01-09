using System;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations
{
    public class OrganisationId : Id
    {
        public OrganisationId(Guid value) : base(value)
        {
        }

        public static OrganisationId New()
        {
            return new OrganisationId(CombGuidIdGeneration.NewGuid());
        }

        public static explicit operator OrganisationId(Guid id)
        {
            return new OrganisationId(id);
        }
    }
}