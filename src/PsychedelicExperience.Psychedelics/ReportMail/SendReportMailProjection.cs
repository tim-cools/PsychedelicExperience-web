using System;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Mail;
using PsychedelicExperience.Common.Views;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Events.Events;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Events;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Events;
using PsychedelicExperience.Psychedelics.Messages.UserInteractions.Events;

namespace PsychedelicExperience.Psychedelics.ReportMail
{
    public class MailSent
    {
        public Guid Id { get; set; }
    }

    public class SendReportMailProjection : EventProjection
    {
        private readonly string _prefix;
        private readonly IMailSender _mailSender;

        public override Type ViewType => typeof(MailSent);

        public SendReportMailProjection(IConfiguration configuration, IMailSender mailSender)
        {
            if (!configuration.SendMailEnabled()) return;

            _mailSender = mailSender;

            _prefix = $"[PEx-{configuration.Environment()}] ";

            SendMail<UserRegistered>(
                @event => $"User Registered: {@event.DisplayName}");

            SendMail<ExperienceAdded>(
                @event => $"Experience added: {@event.Title}");

            SendMail<ExperienceReported>(
                @event => $"Experience Reported: {@event.Reason}");

            SendMail<OrganisationAdded>(
                @event => $"Organisation Added: {@event.Name}");

            SendMail<OrganisationContacted>(
                @event => $"Organisation contacted: {@event.OrganisationId}");

            SendMail<OrganisationReported>(
                @event => $"OrganisationReported: {@event.Reason}");
            
            SendMail<OrganisationReviewAdded>(
                @event => $"OrganisationReviewAdded: {@event.Name}");

            SendMail<OrganisationReviewReported>(
                @event => $"Organisation Review Reported: {@event.Reason}");

            SendMail<OrganisationOwnerConfirmed>(
                @event => $"Organisation owner confirmed: {@event.UserId}");

            SendMail<OrganisationUpdateAdded>(
                @event => $"Organisation update added: {@event.UserId}");

            SendMail<EventAdded>(
                @event => $"Event added: {@event.Name}");

            SendMail<Commented>(
                @event => $"Commented: {@event.UserInteractionId}");
        }

        private void SendMail<T>(Func<T, string> subjectFormatter)
            where T : Event
        {
            EventAsync<T>(async (session, id, @event) =>
            {
                var subject = _prefix + subjectFormatter(@event);
                var eventJson = JsonConvert.SerializeObject(@event, Formatting.Indented);
                var body = $"Event: {@eventJson}";

                await _mailSender.SendMail(subject, body);
            });
        }
    }
}