using System.IO;
using System.Text;
using Bogus;
using PsychedelicExperience.Common;
using PsychedelicExperience.Psychedelics.ExperienceView;
using PsychedelicExperience.Psychedelics.Messages.Documents.Queries;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Queries;
using PsychedelicExperience.Psychedelics.Messages.TopicInteractions.Queries;
using Xunit;
using Dose = PsychedelicExperience.Psychedelics.Messages.Experiences.Queries.Dose;
using Position = PsychedelicExperience.Psychedelics.Messages.Organisations.Queries.Position;

namespace PsychedelicExperience.Psychedelics.Tests.Unit
{
    public class DocumentPropertyTypes
    {
        [Fact]
        public void DocumentReactPropertyTypes()
        {
            var builder = new StringBuilder();

            var documenter = new PropTypesDocumenter(builder);

            documenter.Header();

            documenter.Document<FieldPrivilege>();
            documenter.Document<Position>();
            documenter.Document<OptionalDescription>();
            documenter.Document<RatingDetails>();

            documenter.Document<Dose>();
            documenter.Document<ExperienceSummary>();
            documenter.Document<ExperienceDetails>();
            documenter.Document<ExperiencesResult>();
            documenter.Document<ExperienceStatistics>();

            documenter.Document<CenterDetails>();
            documenter.Document<CenterReviewDetails>();

            documenter.Document<CommunityDetails>();
            documenter.Document<CommunityReviewDetails>();

            documenter.Document<HealthcareProviderDetails>();
            documenter.Document<HealthcareProviderReviewDetails>();

            documenter.Document<OrganisationSummary>();
            documenter.Document<OrganisationDetailsPrivileges>();
            documenter.Document<OrganisationDetails>();
            documenter.Document<OrganisationsResult>();
            documenter.Document<OrganisationMapPoint>();
            //documenter.Document<OrganisationStatistics>();

            documenter.Document<OrganisationReviewDetails>();
            documenter.Document<OrganisationReviewResult>();

            documenter.Document<OrganisationUpdateDetailsPrivileges>();
            documenter.Document<OrganisationUpdateDetails>();
            documenter.Document<OrganisationUpdatesResult>();

            documenter.Document<DocumentSummary>();
            documenter.Document<DocumentDetailsPrivileges>();
            documenter.Document<DocumentDetails>();
            documenter.Document<DocumentsResult>();

            documenter.Document<EventSummary>();
            documenter.Document<EventDetailsPrivileges>();
            documenter.Document<EventDetails>();
            documenter.Document<EventsResult>();
            documenter.Document<EventMapPoint>();

            documenter.Document<TopicComment>();
            documenter.Document<TopicInteractionDetails>();

            documenter.DefaultExport();

            File.WriteAllText("c:\\temp\\propTypesContracts.js", builder.ToString());
        }

        [Fact]
        public void DocumentContractDummys()
        {

            var documenter = new DummyDocumenter();

            documenter.Header();

            documenter.Document(CreateFieldPrivilege());
            documenter.Document(CreatePosition());

            documenter.Document(CreateDose().Generate());
            documenter.Document(CreateExperienceSummary().Generate());
            documenter.Document(CreateExperienceDetails().Generate());
            documenter.Document(CreateExperiencesResult().Generate());

            //documenter.Document(CreateLocation());

            //documenter.Document(CreateOrganisationDetailsPrivileges());
            //documenter.Document(CreateOrganisationSummary());
            //documenter.Document(CreateOrganisationDetails());
            //documenter.Document(CreateOrganisationsResult());

            //documenter.Document(CreateTopicInteractionDetails());
            //documenter.Document(CreateTopicComment());

            documenter.DefaultExport();

            File.WriteAllText("c:\\temp\\dummies.json", documenter.ToString());
        }

        private FieldPrivilege CreateFieldPrivilege()
        {
            return new FieldPrivilege(true, true);
        }

        private Position CreatePosition()
        {
            return new Faker<Position>()
                .RuleFor(fake => fake.Longitude, value => value.Random.Decimal())
                .RuleFor(fake => fake.Latitude, value => value.Random.Decimal())
                .Generate();
        }

        private Faker<Dose> CreateDose()
        {
            return new Faker<Dose>()
                .RuleFor(fake => fake.Id, value => value.Random.ShortGuid())
                .RuleFor(fake => fake.TimeOffset, value => value.Random.Int(30))
                .RuleFor(fake => fake.Amount, value => value.Random.Decimal())
                .RuleFor(fake => fake.Unit, value => value.Lorem.Word())
                .RuleFor(fake => fake.Form, value => value.Lorem.Sentence())
                .RuleFor(fake => fake.Method, value => value.Lorem.Sentence())
                .RuleFor(fake => fake.Notes, value => value.Lorem.Text());
        }

        private Faker<ExperienceSummary> CreateExperienceSummary()
        {
            return new Faker<ExperienceSummary>()
                .RuleFor(fake => fake.ExperienceId, value => new ShortGuid(value.Random.Uuid()))
                .RuleFor(fake => fake.UserId, value => new ShortGuid(value.Random.Uuid()))
                .RuleFor(fake => fake.UserName, value => value.Name.FindName())
                .RuleFor(fake => fake.Created, value => value.Date.Past())
                .RuleFor(fake => fake.Title, value => value.Lorem.Sentence())
                .RuleFor(fake => fake.DateTime, value => value.Date.Past())
                .RuleFor(fake => fake.PrivacyLevel, value => value.PickRandom<PrivacyLevel>().ToString())
                .RuleFor(fake => fake.Tags, value => value.Lorem.Words())
                .RuleFor(fake => fake.Substances, value => value.Lorem.Words())
                ;
        }

        private Faker<ExperienceDetails> CreateExperienceDetails()
        {
            return new Faker<ExperienceDetails>()
                .RuleFor(fake => fake.ExperienceId, value => value.Random.ShortGuid())
                .RuleFor(fake => fake.UserId, value => value.Random.ShortGuid())
                .RuleFor(fake => fake.UserName, value => value.Name.FindName())
                .RuleFor(fake => fake.UserFullName, value => value.Name.FindName())
                .RuleFor(fake => fake.UserAvatar, value => value.Person.Avatar)
                .RuleFor(fake => fake.Created, value => value.Date.Past())
                .RuleFor(fake => fake.Title, value => value.Lorem.Sentence())
                .RuleFor(fake => fake.DateTime, value => value.Date.Past())
                .RuleFor(fake => fake.Level, value => value.PickRandom<ScaleOf5>().ToString())
                .RuleFor(fake => fake.Set, value => value.Lorem.Sentence())
                .RuleFor(fake => fake.Setting, value => value.Lorem.Sentence())
                .RuleFor(fake => fake.PrivateNotes, value => value.Lorem.Sentence())
                .RuleFor(fake => fake.PublicDescription, value => value.Lorem.Sentence())
                .RuleFor(fake => fake.PrivacyLevel, value => value.PickRandom<PrivacyLevel>().ToString())
                .RuleFor(fake => fake.Tags, value => value.Lorem.Words())
                .RuleFor(fake => fake.Doses, value => CreateDose().Generate(4).ToArray())
                .RuleFor(fake => fake.Privileges, value => CreateExperienceDetailPrivileges().Generate());
        }

        private Faker<ExperienceDetailsPrivileges> CreateExperienceDetailPrivileges()
        {
            return new Faker<ExperienceDetailsPrivileges>()
                .RuleFor(fake => fake.Remove, value => value.Random.Bool())
                .RuleFor(fake => fake.Report, value => value.Random.Bool())
                .RuleFor(fake => fake.Editable, value => value.Random.Bool())
                .RuleFor(fake => fake.IsOwner, value => value.Random.Bool())

                .RuleFor(fake => fake.Tags, value => CreateFieldPrivilege())
                .RuleFor(fake => fake.DateTime, value => CreateFieldPrivilege())
                .RuleFor(fake => fake.Doses, value => CreateFieldPrivilege())
                .RuleFor(fake => fake.Level, value => CreateFieldPrivilege())
                .RuleFor(fake => fake.PrivacyLevel, value => CreateFieldPrivilege())
                .RuleFor(fake => fake.PrivateNotes, value => CreateFieldPrivilege())
                .RuleFor(fake => fake.PublicDescription, value => CreateFieldPrivilege())
                .RuleFor(fake => fake.Set, value => CreateFieldPrivilege())
                .RuleFor(fake => fake.Setting, value => CreateFieldPrivilege())
                .RuleFor(fake => fake.Title, value => CreateFieldPrivilege());
        }


        private Faker<ExperiencesResult> CreateExperiencesResult()
        {
            return new Faker<ExperiencesResult>()
                .RuleFor(fake => fake.Experiences, value => CreateExperienceSummary().Generate(12).ToArray())
                .RuleFor(fake => fake.Page, value => 0)
                .RuleFor(fake => fake.Last, value => 12)
                .RuleFor(fake => fake.Total, value => 12);
        }

        private Faker<OrganisationDetailsPrivileges> CreateOrganisationDetailsPrivileges()
        {
            return new Faker<OrganisationDetailsPrivileges>()
                .RuleFor(fake => fake.Manageable, value => value.Random.Bool())
                .RuleFor(fake => fake.Editable, value => value.Random.Bool())
                .RuleFor(fake => fake.IsOwner, value => value.Random.Bool())
                ;
        }
    }

    public static class RandomizerExtensionMethods
    {
        public static ShortGuid ShortGuid(this Randomizer randomizer)
        {
            return Common.ShortGuid.NewGuid();
        }
    }
}