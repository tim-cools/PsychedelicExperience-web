using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Commands;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using PsychedelicExperience.Psychedelics.Organisations;
using Shouldly;
using Xunit;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Organisations
{
    public class WhenAddOrganisationPhoto : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly PhotoId _photoId = PhotoId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenAddOrganisationPhoto(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            _organisationId = context.AddOrganisation(_userId);

            var photo = new Photo(_photoId, _userId, "123.png", "orgi");
            var query = new AddOrganisationPhotos(_organisationId, _userId, new[] { photo});
            _result = context.Service.ExecuteNowWithTimeout(query);
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
                var aggregate = context.Session.Load<Organisation>(_organisationId);

                aggregate.ShouldNotBeNull();
                aggregate.Photos.Any(photo => Equals(photo.Id, _photoId)).ShouldBeTrue();
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<OrganisationPhotosAdded>();

                @event.UserId.ShouldBe(_userId);
                @event.OrganisationId.ShouldBe(_organisationId);
                @event.Photos.Length.ShouldBe(1);
                @event.Photos[0].Id.ShouldBe(_photoId);
                @event.Photos[0].UserId.ShouldBe(_userId);
                @event.Photos[0].FileName.ShouldBe("123.png");
                @event.Photos[0].OriginalFileName.ShouldBe("orgi");
            });
        }
    }

    public class WhenAddOrganisationNotAuthorizedPhoto : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly PhotoId _photoId = PhotoId.New();
        private OrganisationId _organisationId;
        private Result _result;

        public WhenAddOrganisationNotAuthorizedPhoto(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId);
            _organisationId = context.AddOrganisation(_userId);

            var photo = new Photo(_photoId, null, "123.png", "orgi");
            var query = new AddOrganisationPhotos(_organisationId, null, new[] { photo });
            _result = context.Service.ExecuteNowWithTimeout(query);
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
                var aggregate = context.Session.Load<Organisation>(_organisationId);

                aggregate.ShouldNotBeNull();
                aggregate.Photos.Any(photo => Equals(photo.Id, _photoId)).ShouldBeTrue();
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_organisationId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<OrganisationPhotosAdded>();

                @event.UserId.ShouldBe(null);
                @event.OrganisationId.ShouldBe(_organisationId);
                @event.Photos.Length.ShouldBe(1);
                @event.Photos[0].Id.ShouldBe(_photoId);
                @event.Photos[0].UserId.ShouldBe(null);
                @event.Photos[0].FileName.ShouldBe("123.png");
                @event.Photos[0].OriginalFileName.ShouldBe("orgi");
            });
        }
    }
}