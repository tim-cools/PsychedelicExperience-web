using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Notifications;

namespace PsychedelicExperience.Psychedelics.NotificationView.Handlers
{
    public class GetNotificationsHandler : QueryHandler<GetNotifications, NotificationsResult>
    {
        private const int PageSize = 10;

        private readonly IUserInfoResolver _userInfoResolver;
        private readonly MarkupRenderer _markupRenderer;

        public GetNotificationsHandler(IQuerySession session, IUserInfoResolver userInfoResolver) : base(session)
        {
            _userInfoResolver = userInfoResolver ?? throw new ArgumentNullException(nameof(userInfoResolver));
            _markupRenderer = new  MarkupRenderer();
        }

        protected override async Task<NotificationsResult> Execute(GetNotifications query)
        {
            var userId = (Guid)query.UserId;
            var skip = query.Page * PageSize;

            var notifications = await Session.Query<Notification>()
                .Stats(out var stats)
                .Where(notification => notification.UserId == userId)
                .OrderByDescending(update => update.DateTime)
                .Skip(skip)
                .Take(PageSize)
                .ToListAsync();

            return new NotificationsResult
            {
                Page = query.Page,
                Total = stats.TotalResults,
                Notifications = Map(notifications),
                Last = skip + notifications.Count
            };
        }

        private Messages.Notifications.Notification[] Map(IReadOnlyList<Notification> notifications)
        {
            return FormatNotifications(notifications)
                .Union(GroupNotifications(notifications))
                .OrderByDescending(notification => notification.DateTime)
                .ToArray();
        }

        private IEnumerable<Messages.Notifications.Notification> FormatNotifications(IReadOnlyList<Notification> notifications)
        {
            return notifications
                .Where(notification => !(notification is GroupNotification))
                .Select(notification => new Messages.Notifications.Notification
                {
                    Body = _markupRenderer.Render(notification.Format(_userInfoResolver)),
                    DateTime = notification.DateTime
                });
        }

        private IEnumerable<Messages.Notifications.Notification> GroupNotifications(IReadOnlyList<Notification> notifications)
        {
            return notifications
                .OfType<GroupNotification>()
                .GroupBy(notification => new
                {
                    type = notification.GetType(),
                    week = notification.DateTime.GetWeekOfYear(),
                    year = notification.DateTime.Year
                })
                .Select(Group)
                .Where(group => group != null)
                .ToArray();
        }

        private  Messages.Notifications.Notification Group(IEnumerable<GroupNotification> notifications)
        {
            var last = notifications.OrderBy(notification => notification.DateTime).Last();
            var groupFormat = last.GroupFormat(notifications);
            return groupFormat != null 
                ? new Messages.Notifications.Notification
            {
                Body = _markupRenderer.Render(groupFormat),
                DateTime = last.DateTime
            } : null;
        }
    }
}