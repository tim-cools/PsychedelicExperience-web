using System;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;

namespace  PsychedelicExperience.Membership.Messages.UserProfiles
{
	public class UserProfileCreated : Event
	{
		public UserProfileId UserProfileId { get; set; }
		public UserId CreatorId { get; set; }
		public Name DisplayName { get; set; }
		public Name FullName { get; set; }
		public EMail EMail { get; set; }
	}

	public class UserProfileEMailChanged : Event
	{
		public UserProfileId UserProfileId { get; set; }
		public UserId UserId { get; set; }
		public EMail EMail { get; set; }
		public Name DisplayName { get; set; }
	}

	public class UserProfileDisplayNameChanged : Event
	{
		public UserProfileId UserProfileId { get; set; }
		public UserId UserId { get; set; }
		public Name DisplayName { get; set; }
	}

	public class UserProfileFullNameChanged : Event
	{
		public UserProfileId UserProfileId { get; set; }
		public UserId UserId { get; set; }
		public Name FullName { get; set; }
	}

	public class UserProfileAvatarChanged : Event
	{
		public UserProfileId UserProfileId { get; set; }
		public UserId UserId { get; set; }
		public AvatarId AvatarId { get; set; }
		public string FileName { get; set; }
		public string OriginalFileName { get; set; }
	}

	public class UserProfileTaglineChanged : Event
	{
		public UserProfileId UserProfileId { get; set; }
		public UserId UserId { get; set; }
		public string TagLine { get; set; }
	}

	public class UserProfileDescriptionChanged : Event
	{
		public UserProfileId UserProfileId { get; set; }
		public UserId UserId { get; set; }
		public Description Description { get; set; }
	}

	public class UserProfilePrivacyChanged : Event
	{
		public UserProfileId UserProfileId { get; set; }
		public UserId UserId { get; set; }
		public PrivacyLevel Level { get; set; }
	}

	public class UserProfileNotificationEmailDisabled : Event
	{
		public UserProfileId UserProfileId { get; set; }
		public UserId UserId { get; set; }
	}

	public class UserProfileNotificationEmailEnabled : Event
	{
		public UserProfileId UserProfileId { get; set; }
		public UserId UserId { get; set; }
	}

	public class UserProfileNotificationEmailIntervalChanged : Event
	{
		public UserProfileId UserProfileId { get; set; }
		public UserId UserId { get; set; }
		public TimeSpan Interval { get; set; }
	}

}
