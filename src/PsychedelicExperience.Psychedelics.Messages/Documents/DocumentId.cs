using System;
using Marten.Schema.Identity;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Documents
{
    public class DocumentId : Id
    {
        public DocumentId(Guid value) : base(value)
        {
        }

        public static DocumentId New()
        {
            return new DocumentId(CombGuidIdGeneration.NewGuid());
        }

        public static explicit operator DocumentId(Guid id)
        {
            return new DocumentId(id);
        }
    }
}