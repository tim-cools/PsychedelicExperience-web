using System;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Events.Commands
{
    public class ChangeEventEndDateTime : IRequest<Result>
    {
        public UserId UserId { get; }
        public EventId EventId { get; }
        public DateTime? DateTime { get; }

        public ChangeEventEndDateTime(UserId userId, EventId eventId, DateTime? dateTime)
        {
            UserId = userId;
            EventId = eventId;
            DateTime = dateTime;
        }
    }
}