using System;
using Microsoft.Extensions.Logging.Console.Internal;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Psychedelics.Messages.Substances.Queries;
using PsychedelicExperience.Psychedelics.TopicInteractionView;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Substances
{
    public class WhenFollowingAnOrganisation : ApiTest
    {
        private TestAccount _account;
        private SubstanceResult _result;
        private Guid _organisationId;
        private TopicInteraction _originalTopic;

        public WhenFollowingAnOrganisation(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);
            _organisationId = ApiClient.Organisations.Create(_account);
            _originalTopic = ApiClient.TopicInteraction.Get(_account, _organisationId);
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.TopicInteraction.Follow(_account, _organisationId);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenTheOriginalTopicShouldBe0()
        {
            _originalTopic.Followers.ShouldBe(0);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenTheFollowerShouldBeIncreased()
        {
            Execute.WithTimeOut(() => ApiClient.TopicInteraction.Get(_account, _organisationId),
                organisation => organisation.Followers == 1,
                organisation => "not changed");
        }
    }

    public class WhenUnfollowingAnOrganisation : ApiTest
    {
        private TestAccount _account;
        private SubstanceResult _result;
        private Guid _organisationId;
        private TopicInteraction _originalTopic;

        public WhenUnfollowingAnOrganisation(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.GetAdministrator(context);
            _organisationId = ApiClient.Organisations.Create(_account);

            ApiClient.TopicInteraction.Follow(_account, _organisationId);

            _originalTopic = Execute.WithTimeOut(() => ApiClient.TopicInteraction.Get(_account, _organisationId),
                organisation => organisation.Followers == 1,
                organisation => "not changed");
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.TopicInteraction.Follow(_account, _organisationId);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenTheOriginalTopicShouldBe1()
        {
            _originalTopic.Followers.ShouldBe(1);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenTheFollowerShouldBeIncreased()
        {
            Execute.WithTimeOut(() => ApiClient.TopicInteraction.Get(_account, _organisationId),
                organisation => organisation.Followers == 0,
                organisation => "not changed: " + organisation.Followers);
        }
    }
}