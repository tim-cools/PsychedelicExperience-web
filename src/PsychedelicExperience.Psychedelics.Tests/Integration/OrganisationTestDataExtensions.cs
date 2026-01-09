using System;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Commands;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Commands;
using Xunit.Abstractions;
using Contact = PsychedelicExperience.Psychedelics.Messages.Organisations.Contact;

namespace PsychedelicExperience.Psychedelics.Tests.Integration
{
    public static class OrganisationTestDataExtensions
    {
        public static OrganisationId AddOrganisation(this TestContext<IMediator> context, UserId userId, string name = "Inner peace international", Address address = null, string[] types = null, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (address == null)
            {
                address = new Address("Somewere", "BE", new Location(1.123m, 2.456m), "123", null);
            }

            var contacts = new[]
            {
                new Contact { Type = ContactTypes.WebSite, Value = "www.fomo.com" },
                new Contact { Type = ContactTypes.EMail, Value = "fo@fbi-cia-nso" },
                new Contact { Type = ContactTypes.Phone, Value = "04867897894" }
            };
            var id = OrganisationId.New();
            var command = new AddOrganisation(id, userId, false, types ?? new [] { "Retreat" }, new Name(name), "description", contacts, new string[] { }, address, null, null, null, null);
            context.ShouldSucceed(command, testOutputHelper);

            return id;
        }

        public static OrganisationUpdateId AddOrganisationUpdate(this TestContext<IMediator> context, UserId userId, OrganisationId organisationId, string subject = "subject", string content = "content must be longer", OrganisationUpdatePrivacy privacy = OrganisationUpdatePrivacy.Public, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var id = OrganisationUpdateId.New();
            var command = new AddOrganisationUpdate(userId, organisationId, id, subject, content, privacy); 
            context.ShouldSucceed(command, testOutputHelper);

            return id;
        }

        public static OrganisationBuilder BuildOrganisation(this TestContext<IMediator> context, UserId userId, string name, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var builder = new OrganisationBuilder(context, userId, testOutputHelper);
            return builder.BuildOrganisation(name);
        }

        public static TestContext<IMediator> AddOrganisationTag(this TestContext<IMediator> context, UserId userId, OrganisationId id, string tag, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var command = new AddOrganisationTag(id, userId, tag);
            context.ShouldSucceed(command, testOutputHelper);

            return context;
        }

        public static TestContext<IMediator> AddOrganisationOwnerAsAdmin(this TestContext<IMediator> context, OrganisationId id, UserId ownerId, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var adminId = context.AddAdministrator();
            var command = new AddOrganisationOwner(id, adminId, null, ownerId);
            context.ShouldSucceed(command, testOutputHelper);

            return context;
        }

        public static TestContext<IMediator> AddOrganisationOwner(this TestContext<IMediator> context, UserId userId, OrganisationId id, UserId ownerId, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var command = new AddOrganisationOwner(id, userId, null, ownerId);
            context.ShouldSucceed(command, testOutputHelper);

            return context;
        }

        public static TestContext<IMediator> AddOrganisationPhoto(this TestContext<IMediator> context, UserId userId, OrganisationId id, PhotoId photoId, string fileName, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var photo = new Photo(photoId, userId, fileName, fileName);
            var command = new AddOrganisationPhotos(id, userId, new[] { photo });
            context.ShouldSucceed(command, testOutputHelper);

            return context;
        }

        public static TestContext<IMediator> AddOrganisationType(this TestContext<IMediator> context, UserId userId, OrganisationId id, string type, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var command = new AddOrganisationType(id, userId, type);
            context.ShouldSucceed(command, testOutputHelper);

            return context;
        }

        public static TestContext<IMediator> ChangeOrganisationPerson(this TestContext<IMediator> context, UserId userId, OrganisationId id, bool person, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var command = new ChangeOrganisationPerson(id, userId, person);
            context.ShouldSucceed(command, testOutputHelper);

            return context;
        }

        public static TestContext<IMediator> AddOrganisationInfo(this TestContext<IMediator> context, OrganisationId id, UserId userId, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var command = new SetOrganisationWarning(id, userId, "title", "content");
            context.ShouldSucceed(command, testOutputHelper);

            return context;
        }

        public static TestContext<IMediator> AddOrganisationWarning(this TestContext<IMediator> context, OrganisationId id, UserId userId, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var command = new SetOrganisationInfo(id, userId, "title", "content");
            context.ShouldSucceed(command, testOutputHelper);

            return context;
        }

        public static OrganisationReviewId AddOrganisationReview(this TestContext<IMediator> context, UserId userId, string name = "Bull shit", string description = "more bull shit", ScaleOf5 rating = ScaleOf5.Three, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var organisationId = context.AddOrganisation(userId);
            var id = OrganisationReviewId.New();

            var command = new AddOrganisationReview(id, new DateTime(2000, 10, 12), organisationId, userId, name, description, rating, null, null, null, null, null);
            context.ShouldSucceed(command, testOutputHelper);

            return id;
        }
    }
}
