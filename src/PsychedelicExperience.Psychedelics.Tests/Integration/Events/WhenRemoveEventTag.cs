using System;
using System.Linq;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Events;
using PsychedelicExperience.Psychedelics.Messages.Events;
using PsychedelicExperience.Psychedelics.Messages.Events.Commands;
using PsychedelicExperience.Psychedelics.Messages.Events.Events;
using Shouldly;
using Xunit;
using EventSourceExtensions = PsychedelicExperience.Common.Aggregates.EventSourceExtensions;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Events
{
    public class WhenRemoveEventTag : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private EventId _eventId;
        private RemoveEventTag _command;
        private UserId _userId;
        private Result _result;

        public WhenRemoveEventTag(PsychedelicsIntegrationTestFixture fixture)
            : base((IntegrationTestFixture) fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            _userId = context.AddUser();
            _eventId = context.AddEvent(_userId);
            context.AddEventTag(_eventId, _userId, new Name("Tag1"));

            _command = new RemoveEventTag(_eventId, _userId, new Name("Tag1"));
            _result = context.Service.ExecuteNowWithTimeout<Result>(_command);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            ResultExtensions.ShouldBeSuccess(_result);
        }

        [Fact]
        public void ThenTheAggregateShouldBeUpdated()
        {
            SessionScope(context =>
            {
                var aggregate = EventSourceExtensions.Load<Event>(context.Session, _eventId);

                aggregate.ShouldNotBeNull();
                aggregate.UserId.ShouldBe(_userId);
                aggregate.Tags.ShouldNotContain(tag => Object.Equals(tag.Name, _command.TagName));
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = EventSourceExtensions.LoadEvents((IDocumentSession) context.Session, (Id) _eventId).ToArray();
                events.Length.ShouldBe(3);

                var @event = events.LastEventShouldBeOfType<EventTagRemoved>();

                @event.UserId.ShouldBe(_userId);

                @event.EventId.ShouldBe(_eventId);
                @event.TagName.ShouldBe(_command.TagName);

            });
        }
    }
}