using System;

namespace  PsychedelicExperience.Psychedelics.NotificationView
{
	public class NotificationUserStatus
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public int? LastId { get; set; }
		public bool Enabled { get; set; }
		public TimeSpan Interval { get; set; }
		public DateTime? Expire { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public DateTime DateTime { get; set; }

		public NotificationUserStatus() : base()
		{
		}

		public NotificationUserStatus(Guid userId, int? lastId, bool enabled, TimeSpan interval, DateTime? expire, string name, string email, DateTime dateTime) : base()
		{
			UserId = userId;
			LastId = lastId;
			Enabled = enabled;
			Interval = interval;
			Expire = expire;
			Name = name;
			Email = email;
			DateTime = dateTime;
		}
	}
}
