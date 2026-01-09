using System;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations
{
    public class OrganisationReviewId : Id
    {
        public OrganisationReviewId(Guid value) : base(value)
        {
        }

        public static OrganisationReviewId New()
        {
            return new OrganisationReviewId(CombGuidIdGeneration.NewGuid());
        }

        public static explicit operator OrganisationReviewId(Guid id)
        {
            return new OrganisationReviewId(id);
        }
    }
}