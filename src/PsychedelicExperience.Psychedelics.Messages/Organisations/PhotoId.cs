using System;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations
{
    public class PhotoId : Id
    {
        public PhotoId(Guid value) : base(value)
        {
        }

        public static PhotoId New()
        {
            return new PhotoId(CombGuidIdGeneration.NewGuid());
        }

        public static explicit operator PhotoId(Guid id)
        {
            return new PhotoId(id);
        }
    }
}