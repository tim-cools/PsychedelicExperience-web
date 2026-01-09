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
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Events
{
    public class WhenRemoveEventMember : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private EventId _eventId;
        private RemoveEventMember _command;
        private UserId _userId;
        private Result _result;
        private Guid _eventMemberId;

        public WhenRemoveEventMember(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            _userId = context.AddUser();
            _eventId = context.AddEvent(_userId);
            _eventMemberId = context.InviteEventMember(_eventId, _userId, _userId);

            _command = new RemoveEventMember(_eventId, _userId, _userId);
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
                var aggregate = EventSourceExtensions.Load<EventMember>(context.Session, _eventMemberId);

                aggregate.ShouldNotBeNull();
                aggregate.Status.ShouldBe(Psychedelics.Events.EventMemberStatus.Removed);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_eventMemberId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<EventMemberRemoved>();

                @event.UserId.ShouldBe(_userId);
                @event.EventId.ShouldBe(_eventId);
            });
        }

        [Fact]
        public void ThenTheMemberShouldBeRemoved()
        {
            SessionScope(context =>
            {
                var members = context.GetEventMembers(_userId, _eventId);
                members.Members.ShouldNotContain(where => where.UserId.Guid == _userId.Value);
            });
        }
    }
}