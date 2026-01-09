using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Mail;
using PsychedelicExperience.Common.Views;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.ReportMail
{
    public class MailSubscription
    {
        public Guid Id { get; set; }
    }

    public class MailSubscriptionProjection : EventProjection, IDisposable
    {
        private readonly ISendInBlue _sendInBlue;
        private readonly ILogger<MailSubscriptionProjection> _logger;

        public override Type ViewType => typeof(MailSubscription);

        public MailSubscriptionProjection(IConfiguration configuration, ISendInBlue sendInBlue, ILogger<MailSubscriptionProjection> logger)
        {
            if (!configuration.SendInBlueApiEnabled()) return;

            _sendInBlue = sendInBlue;
            _logger = logger;

            EventAsync<UserRegistered>(async (session, id, @event) => await CheckNewsletter(@event));
        }

        private async Task CheckNewsletter(UserRegistered @event)
        {
            try
            {
                if (@event.EMail != null)
                {
                    await _sendInBlue.AddNewsletter(@event.EMail?.Value, @event.DisplayName?.Value);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while updating newsletter subscription {@event.EMail?.Value}, {@event.DisplayName?.Value}" );
            }
        }

        public void Dispose()
        {
        }
    }
}