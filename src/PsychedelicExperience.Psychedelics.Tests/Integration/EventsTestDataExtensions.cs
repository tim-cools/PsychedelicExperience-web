using System;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Events;
using PsychedelicExperience.Psychedelics.Messages.Events.Commands;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;
using Xunit.Abstractions;
using EventMember = PsychedelicExperience.Psychedelics.Events.EventMember;
using EventPrivacy = PsychedelicExperience.Psychedelics.Messages.Events.EventPrivacy;
using EventType = PsychedelicExperience.Psychedelics.Messages.Events.EventType;

namespace PsychedelicExperience.Psychedelics.Tests.Integration
{
    public static class EventsTestDataExtensions
    {
        public static EventId AddEvent(this TestContext<IMediator> context, UserId userId, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var organisationId = context.AddOrganisation(userId);
            context.AddOrganisationOwnerAsAdmin(organisationId, userId);

            var id = EventId.New();

            var type = EventType.Ceremony;
            var privacy = EventPrivacy.Public;
            var name = new Name("This is the event");
            var description = new Description("This is the event description with a valid long description. 12345678900");
            var dateTime = DateTime.Now.AddDays(1);
            var location = new EventLocation();
            var tags = new[] {new Name("tag1") };

            var command = new AddEvent(userId, organisationId, id, type, privacy, name, description, dateTime, dateTime.AddDays(1), location, tags);
            context.ShouldSucceed(command, testOutputHelper);

            return id;
        }

        public static void AddEventTag(this TestContext<IMediator> context, EventId eventId, UserId userId, Name name, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var command = new AddEventTag(eventId, userId, name);
            context.ShouldSucceed(command, testOutputHelper);
        }

        public static Guid InviteEventMember(this TestContext<IMediator> context, EventId eventId, UserId userId, UserId memberId, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var command = new InviteEventMember(eventId, userId, memberId);
            context.ShouldSucceed(command, testOutputHelper);

            return context.GetEventMemberId(eventId, memberId);
        }

        public static Guid GetEventMemberId(this TestContext<IMediator> context, EventId eventId, UserId memberId)
        {
            var memberIdValue = memberId.Value;
            var eventIdValue = eventId.Value;
            var member = context.Session.Query<EventMember>()
                .FirstOrDefault(where => where.EventId == eventIdValue
                                         && where.MemberId == memberIdValue);
            if (member == null)
            {
                throw new BusinessException();
            }
            return member.Id;
        }

        public static EventMembersResult GetEventMembers(this TestContext<IMediator> context, UserId userId, EventId eventId)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var command = new GetEventMembers(userId, eventId);
            return context.Service.ExecuteNowWithTimeout(command);
        }
    }
}
