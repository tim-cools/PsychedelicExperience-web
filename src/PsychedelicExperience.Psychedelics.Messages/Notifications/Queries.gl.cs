using System;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace  PsychedelicExperience.Psychedelics.Messages.Notifications
{
	public class GetNotifications : IRequest<NotificationsResult>
	{
		public UserId UserId { get; set; }
		public int Page { get; set; }

		public GetNotifications(UserId userId, int page) : base()
		{
			UserId = userId;
			Page = page;
		}
	}

	public class NotificationsResult
	{
		public Notification[] Notifications { get; set; }
		public long Page { get; set; }
		public long Total { get; set; }
		public long Last { get; set; }

		public NotificationsResult() : base()
		{
		}

		public NotificationsResult(Notification[] notifications, long page, long total, long last) : base()
		{
			Notifications = notifications;
			Page = page;
			Total = total;
			Last = last;
		}
	}

	public class Notification
	{
		public DateTime DateTime { get; set; }
		public string Body { get; set; }

		public Notification() : base()
		{
		}

		public Notification(DateTime dateTime, string body) : base()
		{
			DateTime = dateTime;
			Body = body;
		}
	}

}
