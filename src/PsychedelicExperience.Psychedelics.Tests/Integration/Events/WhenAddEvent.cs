using System;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Events;
using PsychedelicExperience.Psychedelics.Messages.Events.Commands;
using PsychedelicExperience.Psychedelics.Messages.Events.Events;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.NotificationView;
using PsychedelicExperience.Psychedelics.NotificationView.Notifications;
using Shouldly;
using Xunit;
using Event = PsychedelicExperience.Psychedelics.Events.Event;
using EventPrivacy = PsychedelicExperience.Psychedelics.Messages.Events.EventPrivacy;
using EventType = PsychedelicExperience.Psychedelics.Messages.Events.EventType;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Events
{
    public class WhenAddEvent : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private OrganisationId _organisationId;
        private EventId _eventId;
        private AddEvent _command;
        private UserId _userId;
        private Result _result;

        public WhenAddEvent(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            _userId = context.AddUser();
            _organisationId = context.AddOrganisation(_userId);

            context.AddOrganisationOwnerAsAdmin(_organisationId, _userId);

            _eventId = EventId.New();

            _command = new AddEvent(_userId, _organisationId, _eventId, EventType.Ceremony, 
                EventPrivacy.Public, new Name("This is a name"), 
                new Description("And teh description with at least 50 long134654676546546546"), 
                DateTime.Today.AddDays(1), 
                DateTime.Today.AddDays(2), new EventLocation(),
                new[] { new Name("tag1") }
                );
            _result = context.Service.ExecuteNowWithTimeout(_command);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheAggregateShouldBeUpdated()
        {
            SessionScope(context =>
            {
                var aggregate = context.Session.Load<Event>(_eventId);

                aggregate.ShouldNotBeNull();
                aggregate.UserId.ShouldBe(_userId);
                aggregate.Tags.Count.ShouldBe(1);
                aggregate.Tags[0].Name.Value.ShouldBe("tag1");
            });
        }


        [Fact]
        public void ThenANotificationShouldBeAdded()
        {
            SessionScope(context =>
            {
                var notifications = context.Session.Query<Notification>()
                    .Where(notification => notification.UserId == _userId.Value)
                    .ToList();

                var last = notifications.Last();
                last.ShouldNotBeNull();
                last.ShouldBeOfType<EventAddedNotification>();
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_eventId).ToArray();
                events.Length.ShouldBe(1);

                var @event = events.LastEventShouldBeOfType<EventAdded>();

                @event.UserId.ShouldBe(_userId);

                @event.EventId.ShouldBe(_eventId);
                @event.OrganisationId.ShouldBe(_command.OrganisationId);
                @event.Privacy.ShouldBe(_command.Privacy);
                @event.EventType.ShouldBe(_command.EventType);
                @event.StartDateTime.ShouldBe(_command.StartDateTime);
                @event.EndDateTime.ShouldBe(_command.EndDateTime);
                //@event.Location.ShouldBe(_command.Location);
                @event.Name.ShouldBe(_command.Name);
                @event.Description.ShouldBe(_command.Description);
                @event.Tags[0].Value.ShouldBe("tag1");
            });
        }
    }
}