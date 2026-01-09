using System;
using System.Linq;
using System.Threading;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Membership.Messages.UserProfiles;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api.UserProfile
{
    public class WhenGettingTheCurrentAccountById : ApiTest
    {
        private TestAccount _account;
        private UserProfileDetails _result;

        public WhenGettingTheCurrentAccountById(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
            ApiClient.UserProfile.EnsureCreated(_account);
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = ApiClient.UserProfile.Get(_account, _account);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            _result.EMail.ShouldBe(_account.EMail);
            _result.DisplayName.ShouldBe(_account.DisplayName);
        }
    }

    public class WhenGettingTheCurrentAccount : ApiTest
    {
        private TestAccount _account;
        private UserProfileDetails _result;

        public WhenGettingTheCurrentAccount(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
            ApiClient.UserProfile.EnsureCreated(_account);
        }

        protected override void When(TestContext<IMediator> context)
        {
            _result = ApiClient.UserProfile.Get(_account, _account);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            _result.EMail.ShouldBe(_account.EMail);
            _result.DisplayName.ShouldBe(_account.DisplayName);
        }
    }

    public class WhenChangingAccountEMail : ApiTest
    {
        private TestAccount _account;
        private readonly string _email = Guid.NewGuid() +"@you.be";

        public WhenChangingAccountEMail(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
            ApiClient.UserProfile.EnsureCreated(_account);
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.UserProfile.ChangeEmail(_account, _email);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            Execute.WithTimeOut(
                () => ApiClient.UserProfile.Get(_account),
                profile => profile.EMail == _email,
                organisation => "EMail not updated");
        }
    }

    public class WhenChangingTagLine : ApiTest
    {
        private TestAccount _account;

        public WhenChangingTagLine(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
            ApiClient.UserProfile.EnsureCreated(_account);
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.UserProfile.ChangeTagLine(_account, "newtagline");
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            Execute.WithTimeOut(
                () => ApiClient.UserProfile.Get(_account),
                profile => profile.TagLine == "newtagline",
                organisation => "FullName not updated");
        }
    }


    public class WhenChangingAccountDescription : ApiTest
    {
        private TestAccount _account;

        public WhenChangingAccountDescription(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
            ApiClient.UserProfile.EnsureCreated(_account);
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.UserProfile.ChangeDescription(_account, "newDescription");
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            Execute.WithTimeOut(
                () => ApiClient.UserProfile.Get(_account),
                profile => profile.Description == "newDescription",
                organisation => "Avatar not updated");
        }
    }

    public class WhenChangingAccountDisplayName : ApiTest
    {
        private TestAccount _account;

        public WhenChangingAccountDisplayName(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
            ApiClient.UserProfile.EnsureCreated(_account);
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.UserProfile.ChangeDisplayName(_account, "newDisplayName");
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            Execute.WithTimeOut(
                () => ApiClient.UserProfile.Get(_account),
                profile => profile.DisplayName == "newDisplayName",
                organisation => "DisplayName not updated");
        }
    }

    public class WhenChangingAccountFullName : ApiTest
    {
        private TestAccount _account;

        public WhenChangingAccountFullName(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
            ApiClient.UserProfile.EnsureCreated(_account);
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.UserProfile.ChangeFullName(_account, "newFullName");
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            Execute.WithTimeOut(
                () => ApiClient.UserProfile.Get(_account),
                profile => profile.FullName == "newFullName",
                organisation => "FullName not updated");
        }
    }

    public class WhenChangingAccountAvatar : ApiTest
    {
        private TestAccount _account;
        private int _originalLength;

        public WhenChangingAccountAvatar(WebIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void Given(TestContext<IMediator> context)
        {
            _account = ApiClient.Account.CreateTestAccount();
            ApiClient.UserProfile.EnsureCreated(_account);

            _originalLength = ApiClient.Avatar.Get(_account).Length;
        }

        protected override void When(TestContext<IMediator> context)
        {
            ApiClient.UserProfile.ChangeAvatar(_account);
        }

        [Fact]
        [Trait("category", "acceptance")]
        public void ThenCurrentUserDetailsShouldBeReturned()
        {
            Execute.WithTimeOut(
               () => ApiClient.Avatar.Get(_account),
               IsChanged,
               organisation => "Avatar not updated");            
        }

        private bool IsChanged(byte[] avatar)
        {
            return avatar.Length != _originalLength;
        }
    }
}