using System.Linq;
using System.Reflection;
using Baseline;
using Marten;
using Marten.Events;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Psychedelics.Events;
using PsychedelicExperience.Psychedelics.Experiences;
using PsychedelicExperience.Psychedelics.UserInteractions;
using Dose = PsychedelicExperience.Psychedelics.Experiences.Dose;
using Experience = PsychedelicExperience.Psychedelics.Experiences.Experience;
using PsychedelicExperience.Psychedelics.Organisations;
using Event = PsychedelicExperience.Psychedelics.Events.Event;
using Organisation = PsychedelicExperience.Psychedelics.Organisations.Organisation;

namespace PsychedelicExperience.Psychedelics
{
    public class PsychedelicsDocumentRegistry : MartenRegistry, IInitializeEvents
    {
        public PsychedelicsDocumentRegistry()
        {
            For<Experience>();
            For<Dose>();

            For<Survey>();

            For<Organisation>();
            For<OrganisationReview>();

            For<Event>();
            For<EventMember>();

            For<Documents.Document>();
            
            For<OrganisationUpdates.OrganisationUpdate>();

            For<ExperienceView.Experience>().DocumentAlias("views_experience");

            For<TopicInteractionView.TopicInteraction>().DocumentAlias("views_topic_interaction");
            
            For<OrganisationView.Organisation>().DocumentAlias("views_organisation");
            For<OrganisationView.Review>().DocumentAlias("views_organisation_review");

            For<OrganisationUpdateView.OrganisationUpdate>().DocumentAlias("views_organisation_update");

            For<DocumentView.Document>().DocumentAlias("views_document");

            For<EventView.Event>().DocumentAlias("views_event");
            For<EventView.EventMember>().DocumentAlias("views_event_member");

            For<ReportMail.ExternalMailSent_v3>().DocumentAlias("projection_externalemail");
            For<ReportMail.ExternalMailUserContact>().DocumentAlias("projection_externalemail_usercontact");
            For<ReportMail.ExternalMailOrganisationContact>().DocumentAlias("projection_externalemail_organisationcontact");

            For<NotificationView.Notification>().AddSubClassHierarchy().DocumentAlias("notification");
            For<NotificationView.NotificationUserStatus>().DocumentAlias("notification_userstatus");
            For<NotificationView.NotificationTopicStatus>().DocumentAlias("notification_topsstatus");
        }

        public void Initialize(EventGraph events)
        {
            typeof(PsychedelicsDocumentRegistry).Assembly.GetTypes()
                .Where(type => type.BaseType == typeof(AggregateRoot))
                .Each(type =>
                {
                    typeof(EventSourceExtensions)
                        .GetMethod(nameof(EventSourceExtensions.ConfigureEventSourced))
                        .MakeGenericMethod(type)
                        .Invoke(null, new object[]{events});
                });
        }
    }
}