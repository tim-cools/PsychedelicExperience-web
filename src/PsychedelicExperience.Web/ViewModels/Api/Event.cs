using System;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Events;
using PsychedelicExperience.Psychedelics.Messages.Events.Commands;
using PsychedelicExperience.Psychedelics.Messages.Organisations;

namespace PsychedelicExperience.Web.ViewModels.Api
{
    public class PostEventRequest
    {
        public Name Name { get; set; }
        public Description Description { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public EventType EventType { get; set; }
        public EventPrivacy Privacy { get; set; }

        public Name LocationName { get; set; }
        public GooglePlace Address { get; set; }
        public Name[] Tags { get; set; }

        public AddEvent ToCommand(OrganisationId organisationId, EventId eventId, UserId userId)
        {
            var eventLocation = new EventLocation
            {
                Name = LocationName,
                Address = Address.ToAddress()
            };

            return new AddEvent(
                userId,
                organisationId,
                eventId,
                EventType,
                Privacy,
                Name,
                Description,
                StartDateTime,
                EndDateTime,
                eventLocation,
                Tags);
        }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Description)}: {Description}, {nameof(StartDateTime)}: {StartDateTime}, {nameof(EndDateTime)}: {EndDateTime}, {nameof(EventType)}: {EventType}, {nameof(Privacy)}: {Privacy}, {nameof(LocationName)}: {LocationName}, {nameof(Address)}: {Address}";
        }
    }
}