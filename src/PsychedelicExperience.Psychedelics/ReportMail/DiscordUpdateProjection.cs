using System;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Views;
using PsychedelicExperience.Membership.Messages.UserProfiles;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Events.Events;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Events;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Events;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Events;
using PsychedelicExperience.Psychedelics.Messages.UserInteractions.Events;

namespace PsychedelicExperience.Psychedelics.ReportMail
{
    public class DiscordUpdate
    {
        public Guid Id { get; set; }
    }

    public class DiscordUpdateProjection : EventProjection, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly string _prefix;
        private readonly string _token;
        private readonly ulong _channel;

        public override Type ViewType => typeof(DiscordUpdate);

        public DiscordUpdateProjection(IConfiguration configuration)
        {
            _configuration = configuration;
            if (!configuration.SendMailEnabled()) return;

            _token = configuration.DiscordToken();
            _channel = configuration.DiscordChannel();
            _prefix = $"[PEx-{configuration.Environment()}]  ";

            SendMail<UserRegistered>(
                @event => $"User Registered: {@event.DisplayName} ({_configuration.WebUrl("user", @event.Id)})");

            SendMail<ExperienceAdded>(
                @event => $"Experience added: {@event.Title} ({_configuration.WebUrl("experience", @event.ExperienceId)})");

            SendMail<ExperienceReported>(
                @event => $"Experience Reported: {@event.Reason} ({_configuration.WebUrl("experience", @event.ExperienceId)})");

            SendMail<OrganisationAdded>(
                @event => $"Organisation Added: {@event.Name} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");

            SendMail<OrganisationPersonChanged>(
                @event => $"Organisation Person changed: {@event.Person} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");

            SendMail<OrganisationCenterChanged>(
                @event => $"Organisation center changed: {@event.UserId} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");

            SendMail<OrganisationPractitionerChanged>(
                @event => $"Organisation practitioner changed: {@event.UserId} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");

            SendMail<OrganisationContacted>(
                @event => $"Organisation contacted: {@event.OrganisationName} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");

            SendMail<OrganisationReported>(
                @event => $"OrganisationReported: {@event.OrganisationName}: {@event.Reason} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");
            
            SendMail<OrganisationReviewAdded>(
                @event => $"OrganisationReviewAdded: {@event.Name} ({_configuration.WebUrl("organisation", @event.OrganisationId, "review", @event.OrganisationReviewId)})");

            SendMail<OrganisationReviewReported>(
                @event => $"Organisation Review Reported: {@event.Reason} ({_configuration.WebUrl("organisation", @event.OrganisationId, "review", @event.OrganisationReviewId)})");

            SendMail<OrganisationOwnerConfirmed>(
                @event => $"Organisation owner confirmed: {@event.UserId} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");

            SendMail<OrganisationUpdateAdded>(
                @event => $"Organisation update added: {@event.UserId} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");

            SendMail<EventAdded>(
                @event => $"Event added: {@event.Name} ({_configuration.WebUrl("event", @event.EventId)})");

            SendMail<Commented>(
                @event => $"Commented: {@event.UserId} ({_configuration.WebUrl("organisation", @event.TopicId)})");

            SendMail<OrganisationNameChanged>(
                @event => $"Organisation Name Changed: {@event.Name} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");
            SendMail<OrganisationContactAddded>(
                @event => $"Organisation Contact Added: {@event.OrganisationName} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");
            SendMail<OrganisationContactRemoved>(
                @event => $"Organisation Contact Removed: {@event.OrganisationName} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");
            SendMail<OrganisationRemoved>(
                @event => $"Organisation Removed: {@event.OrganisationName} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");
            SendMail<OrganisationOwnerAdded>(
                @event => $"Organisation Owner Added: {@event.OrganisationName} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");
            SendMail<OrganisationOwnerRemoved>(
                @event => $"Organisation Owner Removed: {@event.OrganisationName} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");;

            SendMail<OrganisationWarningSet>(
                @event => $"OrganisationWarningSet: {@event.OrganisationName} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");
            SendMail<OrganisationWarningRemoved>(
                @event => $"OrganisationWarningRemoved: {@event.OrganisationName} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");;

            SendMail<OrganisationInfoSet>(
                @event => $"OrganisationWarningSet: {@event.OrganisationName} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");
            SendMail<OrganisationInfoRemoved>(
                @event => $"OrganisationWarningRemoved: {@event.OrganisationName} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");

            SendMail<OrganisationLinked>(
                @event => $"Organisation Linked: {@event.OrganisationName} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");
            SendMail<OrganisationUnlinked>(
                @event => $"Organisation Unlinked: {@event.OrganisationName} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");;

            SendMail<OrganisationPhotosAdded>(
                @event => $"Organisation Photo Added: {@event.OrganisationName} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");
            SendMail<OrganisationPhotoRemoved>(
                @event => $"Organisation Photo Removed: {@event.OrganisationName} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");;

            SendMail<OrganisationTypeAdded>(
                @event => $"Organisation Type Added: {@event.OrganisationName} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");
            SendMail<OrganisationTypeRemoved>(
                @event => $"Organisation Type Removed: {@event.OrganisationName} ({_configuration.WebUrl("organisation", @event.OrganisationId)})");;

            SendMail<UserProfileDisplayNameChanged>(
                @event => $"User Profile Display Name Changed: {@event.DisplayName} ({_configuration.WebUrl("user", @event.UserId)})");
            SendMail<UserProfileEMailChanged>(
                @event => $"User Profile EMail Changed: {@event.DisplayName} ({_configuration.WebUrl("user", @event.UserId)})");
        }

        private void SendMail<T>(Func<T, string> subjectFormatter)
            where T : PsychedelicExperience.Common.Aggregates.Event
        {
            EventAsync<T>(async (session, id, @event) =>
            {
                var timestamp = @event.EventTimestamp;
                var subject = subjectFormatter(@event);

                await SendMessage(timestamp, subject);
            });
        }

        private async Task SendMessage(DateTime timestamp, string subject)
        {
            var message = _configuration.IsProduction()
                ? subject
                : $"{_prefix}[{timestamp}] {subject}";

            using var _discordRestClient = new DiscordRestClient();

            await _discordRestClient.LoginAsync(TokenType.Bot, _token);

            if (!(await _discordRestClient.GetChannelAsync(_channel) is IRestMessageChannel channel))
            {
                throw new InvalidOperationException("Discord channel is not IRestMessageChannel");
            }

            await channel.SendMessageAsync(message);
        }

        public void Dispose()
        {
        }
    }
}