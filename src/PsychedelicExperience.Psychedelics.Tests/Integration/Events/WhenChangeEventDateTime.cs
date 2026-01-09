using System;
using System.Linq;
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
using EventSourceExtensions = PsychedelicExperience.Common.Aggregates.EventSourceExtensions;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Events
{
    public class WhenChangeEventDateTime : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private EventId _eventId;
        private ChangeEventStartDateTime _command;
        private UserId _userId;
        private Result _result;

        public WhenChangeEventDateTime(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            _userId = context.AddUser();
            _eventId = context.AddEvent(_userId);

            _command = new ChangeEventStartDateTime(_userId, _eventId, DateTime.Now.AddDays(5));
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
                var aggregate = EventSourceExtensions.Load<Event>(context.Session, _eventId);

                aggregate.ShouldNotBeNull();
                aggregate.UserId.ShouldBe(_userId);
                aggregate.StartDateTime.ShouldBe(_command.DateTime);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = EventSourceExtensions.LoadEvents(context.Session, _eventId)
                    .ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<EventStartDateTimeChanged>();

                @event.UserId.ShouldBe(_userId);

                @event.EventId.ShouldBe(_eventId);
                @event.DateTime.ShouldBe(_command.DateTime);
            });
        }
    }

    public class WhenChangeEndEventDateTime : ServiceTestBase<IMediator>,
        IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private EventId _eventId;
        private ChangeEventEndDateTime _command;
        private UserId _userId;
        private Result _result;

        public WhenChangeEndEventDateTime(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            _userId = context.AddUser();
            _eventId = context.AddEvent(_userId);

            _command = new ChangeEventEndDateTime(_userId, _eventId, DateTime.Now.AddDays(5));
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
                var aggregate = EventSourceExtensions.Load<Event>(context.Session, _eventId);

                aggregate.ShouldNotBeNull();
                aggregate.UserId.ShouldBe(_userId);
                aggregate.EndDateTime.ShouldBe(_command.DateTime);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = EventSourceExtensions.LoadEvents(context.Session, _eventId)
                    .ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<EventEndDateTimeChanged>();

                @event.UserId.ShouldBe(_userId);

                @event.EventId.ShouldBe(_eventId);
                @event.DateTime.ShouldBe(_command.DateTime);
            });
        }


        public class WhenChangeEndEventDateTimeToNull : ServiceTestBase<IMediator>,
            IClassFixture<PsychedelicsIntegrationTestFixture>
        {
            private EventId _eventId;
            private ChangeEventEndDateTime _command;
            private UserId _userId;
            private Result _result;

            public WhenChangeEndEventDateTimeToNull(PsychedelicsIntegrationTestFixture fixture)
                : base(fixture)
            {
            }

            protected override void When(TestContext<IMediator> context)
            {
                _userId = context.AddUser();
                _eventId = context.AddEvent(_userId);

                _command = new ChangeEventEndDateTime(_userId, _eventId, DateTime.Now.AddDays(5));
                context.ShouldSucceed(_command);

                _command = new ChangeEventEndDateTime(_userId, _eventId, null);
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
                    var aggregate = EventSourceExtensions.Load<Event>(context.Session, _eventId);

                    aggregate.ShouldNotBeNull();
                    aggregate.UserId.ShouldBe(_userId);
                    aggregate.EndDateTime.ShouldBeNull();
                });
            }

            [Fact]
            public void ThenTheEventShouldBeStored()
            {
                SessionScope(context =>
                {
                    var events = EventSourceExtensions.LoadEvents(context.Session, _eventId)
                        .ToArray();
                    events.Length.ShouldBe(3);

                    var @event = events.LastEventShouldBeOfType<EventEndDateTimeChanged>();

                    @event.UserId.ShouldBe(_userId);

                    @event.EventId.ShouldBe(_eventId);
                    @event.DateTime.ShouldBe(_command.DateTime);
                });
            }
        }
    }
}