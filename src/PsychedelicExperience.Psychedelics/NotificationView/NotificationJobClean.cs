using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Mail;
using PsychedelicExperience.Common.Services;
using PsychedelicExperience.Membership.UserInfo;

namespace PsychedelicExperience.Psychedelics.NotificationView
{
    public class NotificationJob : IStartable, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationJob> _logger;
        private readonly IMailSender _mailSender;
        private readonly IDocumentStore _documentStore;
        private readonly IDaemonController _daemonController;
        private readonly MarkupRenderer _markupRenderer;

        private Timer _timer;
        private readonly IUserInfoResolver _userInfoResolver;

        public NotificationJob(IDocumentStore documentStore, IDaemonController daemonController, IMailSender mailSender, ILogger<NotificationJob> logger, IConfiguration configuration, IUserInfoResolver userInfoResolver)
        {
            _userInfoResolver = userInfoResolver ?? throw new ArgumentNullException(nameof(userInfoResolver));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailSender = mailSender ?? throw new ArgumentNullException(nameof(mailSender));
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            _daemonController = daemonController ?? throw new ArgumentNullException(nameof(daemonController));
            _markupRenderer = new MarkupRenderer(new MarkupRendererOptions(configuration.WebHostName(), "notifications", "email"));
        }

        public void Start()
        {
            if (_timer != null) throw new InvalidOperationException("Service already started.");

            //todo check why email ends up in spam
            //_timer = new Timer(Interval, null, TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(1));
        }

        public void Stop()
        {
            if (_timer == null) throw new InvalidOperationException("Service not started.");

            _timer.Dispose();
        }

        private void Interval(object state)
        {
            if (!_configuration.SendMailEnabled()) return;

            if (!ProjectionCompleted()) return;

            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            try
            {
                Process();
            }
            catch (Exception exception)
            {
                _logger.LogErrorMethod(nameof(Process), exception);
            }

            _timer.Change(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(1));
        }

        private void Process()
        {
            using var session = _documentStore.OpenSession();

            var userInfoResolver = new UserInfoResolver(session);

            var userStatuses = GetUsersToNotify(session);

            userStatuses.ForEach(status => NotifyUser(status, userInfoResolver, session));
        }

        private static List<NotificationUserStatus> GetUsersToNotify(IDocumentSession session)
        {
            return session.Query<NotificationUserStatus>()
                .Where(status => status.Enabled && (status.Expire == null || status.Expire <= DateTime.Now))
                .ToList();
        }

        private bool ProjectionCompleted()
        {
            var projectionStatus = _daemonController.GetProgress<NotificationTopicStatus>();
            return projectionStatus >= .99;
        }

        private void NotifyUser(NotificationUserStatus userStatus, IUserInfoResolver userInfoResolver, IDocumentSession session)
        {
            if (userStatus.Email.EndsWith("@dummy.com")) return;
            
            var notifications = GetNotifications(userStatus, session);
            if (notifications.Count == 0) return;

            var markups = Map(notifications);

            SendEmail(userStatus, markups, userInfoResolver);
            UpdateStatus(userStatus, session, notifications);
        }

        private void SendEmail(NotificationUserStatus userStatus, IList<Markup> notifications, IUserInfoResolver userInfoResolver)
        {
            var template = CreateEmail(userStatus, notifications, userStatus.LastId == null);
            var mailboxAddress = new MailboxAddress(userStatus.Name, userStatus.Email);

            _mailSender.SendMail(mailboxAddress, template);
        }

        private static void UpdateStatus(NotificationUserStatus userStatus, IDocumentSession session, IList<Notification> notifications)
        {
            userStatus.LastId = notifications.Last().Id;
            userStatus.DateTime = DateTime.Now;
            userStatus.Expire = DateTime.Now + userStatus.Interval;

            session.Store(userStatus);
            session.SaveChanges();
        }

        private static List<Notification> GetNotifications(NotificationUserStatus userStatus, IDocumentSession session)
        {
            var queryable = userStatus.LastId != null
                ? session.Query<Notification>().Where(where => where.UserId == userStatus.UserId && where.Id > userStatus.LastId.Value)
                : session.Query<Notification>().Where(where => where.UserId == userStatus.UserId);

            return queryable.OrderBy(notification => notification.Id).ToList();
        }

        private IList<Markup> Map(IList<Notification> notifications)
        {
            var ordered = notifications.OrderByDescending(notification => notification.DateTime).ToList();

            return Notifications(ordered)
                .Union(GroupNotifications(ordered))
                .ToList();
        }

        private IEnumerable<Markup> Notifications(IList<Notification> notifications)
        {
            return notifications
                .Where(notification => !(notification is GroupNotification))
                .Select(notification => notification.Format(_userInfoResolver));
        }

        private static IEnumerable<Markup> GroupNotifications(IEnumerable<Notification> notifications)
        {
            return notifications
                .OfType<GroupNotification>()
                .GroupBy(notification => notification.GetType())
                .Select(Group)
                .Where(group => group != null);
        }

        private static Markup Group(IEnumerable<GroupNotification> notifications)
        {
            var last = notifications.OrderBy(notification => notification.DateTime).Last();
            return last.GroupFormat(notifications);
        }

        private Template CreateEmail(NotificationUserStatus userStatus, IList<Markup> notifications, bool first)
        {
            return new NotificationsEmailTemplate(first, userStatus.UserId, userStatus.Name, notifications, _configuration.WebHostName(), _markupRenderer);
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                Stop();
            }
        }
    }
}