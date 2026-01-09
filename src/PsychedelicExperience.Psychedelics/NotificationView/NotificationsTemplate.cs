using System;
using System.Collections.Generic;
using System.Text;
using Baseline;
using PsychedelicExperience.Common.Mail;

namespace PsychedelicExperience.Psychedelics.NotificationView
{
    internal class NotificationsEmailTemplate : Template
    {
        private readonly MarkupRenderer _markupRenderer;

        private readonly string _message;
        private readonly string _userName;
        private readonly string _url;
        private readonly IList<Markup> _notifications;

        public override string TemplateName => "20171113-notifications-email";
        public override string FromAddressPrefix => null;

        public NotificationsEmailTemplate(bool firstEmail, Guid userId, string userName, IList<Markup> notifications, string webHostName, MarkupRenderer markupRenderer)
        {
            _userName = userName;
            _url = $"{webHostName.TrimEnd('/')}/user/{userId}?utm_source=notifications&utm_medium=email&utm_campaign=global";
            _notifications = notifications;
            _markupRenderer = markupRenderer;
            _message = firstEmail
                ? "We like to keep you updated on what happens on PsychedelicExperience. " +
                  "Therefore we will sent you new notifications by e-mail maximum once a day. " +
                  "You can disable the e-mails or change the settings on your user profile."
                : "You have new notifications on PsychedelicExperience.";
        }

        public override string GetSubject()
        {
            return $"Your PsychedelicExperience notifications update ({DateTime.Now.Date:MMMM dd})";
        }

        public override Dictionary<string, string> GetMergeFields()
        {
            var body = FormatNotifications();

            return new Dictionary<string, string>
            {
                { "userName", _userName },
                { "url", _url },
                { "body", body },
                { "message", _message }
            };
        }

        private string FormatNotifications()
        {
            var builder = new StringBuilder();
            _notifications.Each(notification => builder.AppendLine(FormatNotification(notification)));
            return builder.ToString();
        }

        private string FormatNotification(Markup notification)
        {
            return $"<div>{_markupRenderer.Render(notification)}</div>";
        }
    }
}