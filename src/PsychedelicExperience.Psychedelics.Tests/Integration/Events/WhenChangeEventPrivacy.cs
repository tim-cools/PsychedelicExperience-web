using System.Linq;
using Marten;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Events;
using PsychedelicExperience.Psychedelics.Messages.Events;
using PsychedelicExperience.Psychedelics.Messages.Events.Commands;
using PsychedelicExperience.Psychedelics.Messages.Events.Events;
using Shouldly;
using Xunit;
using EventPrivacy = PsychedelicExperience.Psychedelics.Messages.Events.EventPrivacy;
using EventSourceExtensions = PsychedelicExperience.Common.Aggregates.EventSourceExtensions;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Events
{
    public class WhenChangeEventPrivacy : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private EventId _eventId;
        private ChangeEventPrivacy _command;
        private UserId _userId;
        private Result _result;

        public WhenChangeEventPrivacy(PsychedelicsIntegrationTestFixture fixture)
            : base((IntegrationTestFixture) fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            _userId = context.AddUser();
            _eventId = context.AddEvent(_userId);

            _command = new ChangeEventPrivacy(_userId, _eventId, EventPrivacy.MembersOnly);
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
                aggregate.Privacy.ShouldBe(Psychedelics.Events.EventPrivacy.MembersOnly);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = EventSourceExtensions.LoadEvents((IDocumentSession) context.Session, (Id) _eventId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<EventPrivacyChanged>();

                @event.UserId.ShouldBe(_userId);

                @event.EventId.ShouldBe(_eventId);
                @event.Privacy.ShouldBe(_command.Privacy);
            });
        }
    }
}