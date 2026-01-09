using System;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Events
{
    public class ImageId : Id
    {
        public ImageId(Guid value) : base(value)
        {
        }

        public static ImageId New()
        {
            return new ImageId(CombGuidIdGeneration.NewGuid());
        }

        public static explicit operator ImageId(Guid id)
        {
            return new ImageId(id);
        }
    }
}