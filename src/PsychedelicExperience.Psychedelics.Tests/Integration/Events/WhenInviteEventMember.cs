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
    public class WhenInviteEventMember : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private EventId _eventId;
        private InviteEventMember _command;
        private UserId _userId;
        private Result _result;

        public WhenInviteEventMember(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            _userId = context.AddUser();
            _eventId = context.AddEvent(_userId);

            _command = new InviteEventMember(_eventId, _userId, _userId);
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
                aggregate.Status.ShouldBe(Psychedelics.Events.EventMemberStatus.Invited);
                aggregate.AddedUserId.ShouldBe(_userId);
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

                var @event = events.LastEventShouldBeOfType<EventMemberInvited>();

                @event.UserId.ShouldBe(_userId);

                @event.EventId.ShouldBe(_eventId);
                @event.MemberId.ShouldBe(_command.MemberId);
            });
        }

        [Fact]
        public void ThenTheMemberShouldBeAdded()
        {
            SessionScope(context =>
            {
                var members = context.GetEventMembers(_userId, _eventId);
                members.Members.ShouldContain(where => where.UserId.Guid == _userId.Value && where.Status == nameof(Psychedelics.Events.EventMemberStatus.Invited));
            });
        }
    }
}