using System;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;

namespace PsychedelicExperience.Psychedelics.Messages.Tags.Commands
{
    public class UpdateTagName : IRequest<Result>
    {
        public Guid UserId { get; }
        public Guid TagId { get; }
        public String Name { get; }

        public UpdateTagName(Guid userId, Guid tagId, string name)
        {
            UserId = userId;
            TagId = tagId;
            Name = name;
        }
    }
    
}