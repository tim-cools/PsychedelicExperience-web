using System;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Events;
using PsychedelicExperience.Psychedelics.Messages.Events;
using PsychedelicExperience.Psychedelics.Messages.Events.Commands;
using PsychedelicExperience.Psychedelics.Messages.Events.Events;
using PsychedelicExperience.Psychedelics.Tests.Integration.Organisations;
using Shouldly;
using Xunit;
using EventMemberStatus = PsychedelicExperience.Psychedelics.Messages.Events.EventMemberStatus;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Events
{
    public class WhenJoinEventMember : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private EventId _eventId;
        private JoinEventMember _command;
        private UserId _userId;
        private Result _result;

        public WhenJoinEventMember(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            _userId = context.AddUser();
            _eventId = context.AddEvent(_userId);

            _command = new JoinEventMember(_eventId, _userId, EventMemberStatus.Interested);
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
                var eventMemberId = context.GetEventMemberId(_eventId, _userId);
                var aggregate = EventSourceExtensions.Load<EventMember>(context.Session, eventMemberId);

                aggregate.ShouldNotBeNull();
                aggregate.Status.ShouldBe(Psychedelics.Events.EventMemberStatus.Interested);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var eventMemberId = context.GetEventMemberId(_eventId, _userId);
                var events = context.Session.LoadEvents(eventMemberId).ToArray();
                events.Length.ShouldBe(1);

                var @event = events.LastEventShouldBeOfType<EventMemberJoined>();

                @event.UserId.ShouldBe(_userId);

                @event.EventId.ShouldBe(_eventId);
                @event.Status.ShouldBe(_command.Status);
            });
        }

        [Fact]
        public void ThenTheMemberShouldBeAdded()
        {
            SessionScope(context =>
            {
                var members = context.GetEventMembers(_userId, _eventId);
                members.Members.ShouldContain(where => where.UserId.Guid == _userId.Value && where.Status == nameof(EventMemberStatus.Interested));
            });
        }
    }

    public class WhenJoinEventMemberAlreadyJoind : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private EventId _eventId;
        private JoinEventMember _command;
        private UserId _userId;
        private Exception _exception;

        public WhenJoinEventMemberAlreadyJoind(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _userId = context.AddUser();
            _eventId = context.AddEvent(_userId);

            context.InviteEventMember(_eventId, _userId, _userId);

            _command = new JoinEventMember(_eventId, _userId, EventMemberStatus.Interested);
        }

        protected override void When(TestContext<IMediator> context)
        {
            _exception = Expect.Exception(() => context.Service.ExecuteNowWithTimeout(_command));
        }

        [Fact]
        public void ThenTheCommandShouldThrowABusinessException()
        {
            GetInnerException().ShouldBeOfType<BusinessException>();
        }

        private Exception GetInnerException()
        {
            var exception = _exception;
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            return exception;
        }
    }
}