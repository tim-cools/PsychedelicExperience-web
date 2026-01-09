using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Events
{
    public class WhenCreatingAnEvent : ApiTest
    {
        private EventDetails _result;
        private ShortGuid _organisationId;
        private ShortGuid _eventId;
        private TestAccount _account;

        public WhenCreatingAnEvent(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);
            _organisationId = ApiClient.Organisations.Create(_account);
            _eventId = ApiClient.Organisations.CreateEvent(_account, "Our fancy event", _organisationId);
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = ApiClient.Events.Get(_account, _eventId);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            _result.Name.ShouldBe("Our fancy event");
        }
    }
}