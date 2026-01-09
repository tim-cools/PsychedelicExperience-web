using System;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.Organisations
{
    public class WhenCreatingAnOrganisationReview : ApiTest
    {
        private OrganisationReviewResult _result;
        private ShortGuid _organisationId;
        private ShortGuid _reviewId;
        private TestAccount _account;

        public WhenCreatingAnOrganisationReview(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
            _organisationId = ApiClient.Organisations.Create(_account);
            _reviewId = ApiClient.Organisations.CreateReview(_account, _organisationId, new DateTime(2000, 10, 12));
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = ApiClient.Organisations.GetReview(_account, _organisationId, _reviewId);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenTheReviewShouldBeReturned()
        {
            _result.Review.OrganisationId.ShouldBe(_organisationId);
            _result.Review.ReviewId.ShouldBe(_reviewId);
            _result.Review.Visited.ShouldBe(new DateTime(2000, 10, 12));
        }
    }

    public class WhenCreatingAnOrganisationReviewWithCenter : ApiTest
    {
        private OrganisationReviewResult _result;
        private ShortGuid _organisationId;
        private ShortGuid _reviewId;
        private TestAccount _account;

        public WhenCreatingAnOrganisationReviewWithCenter(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
            _organisationId = ApiClient.Organisations.Create(_account);
            _reviewId = ApiClient.Organisations.CreateReview(_account, _organisationId, new DateTime(2000, 10, 12), TestData.CenterReview());
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = ApiClient.Organisations.GetReview(_account, _organisationId, _reviewId);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenTheReviewShouldBeReturned()
        {
            _result.Review.OrganisationId.ShouldBe(_organisationId);
            _result.Review.ReviewId.ShouldBe(_reviewId);
            _result.Review.Visited.ShouldBe(new DateTime(2000, 10, 12));
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenTheCenterShouldBeReturned()
        {
            _result.Review.Center.ShouldNotBeNull();
            _result.Review.Center.Honest.Value.ShouldBe(4);
            _result.Review.Center.Honest.Description.ShouldBe("Honest");
        }
    }

    public class WhenCreatingAnOrganisationReviewWithExperience : ApiTest
    {
        private OrganisationReviewResult _result;
        private ShortGuid _organisationId;
        private ShortGuid _reviewId;
        private TestAccount _account;

        public WhenCreatingAnOrganisationReviewWithExperience(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
            _organisationId = ApiClient.Organisations.Create(_account);
            _reviewId = ApiClient.Organisations.CreateReviewWithExperience(_account, _organisationId, new DateTime(2000, 10, 12));
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = ApiClient.Organisations.GetReview(_account, _organisationId, _reviewId);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenTheReviewShouldBeReturned()
        {
            _result.Review.OrganisationId.ShouldBe(_organisationId);
            _result.Review.ReviewId.ShouldBe(_reviewId);
            _result.Review.Visited.ShouldBe(new DateTime(2000, 10, 12));
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenTheCenterShouldBeReturned()
        {
            _result.Review.Experience.ShouldNotBeNull();
            _result.Review.Experience.Title.ShouldBe("new experience");
            _result.Review.Experience.Description.ShouldBe("new description");
        }
    }
}