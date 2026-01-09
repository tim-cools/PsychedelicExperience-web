using System;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace  PsychedelicExperience.Membership.Messages.UserProfiles
{
	public class CreateUserProfile : IRequest<Result>
	{
		public UserId RequesterId { get; set; }
		public UserProfileId UserProfileId { get; set; }
		public Name DisplayName { get; set; }
		public Name FullName { get; set; }
		public EMail EMail { get; set; }

		public CreateUserProfile(UserId requesterId, UserProfileId userProfileId, Name displayName, Name fullName, EMail eMail) : base()
		{
			RequesterId = requesterId;
			UserProfileId = userProfileId;
			DisplayName = displayName;
			FullName = fullName;
			EMail = eMail;
		}
	}

	public class ChangeUserProfileEMail : IRequest<Result>
	{
		public UserId RequesterId { get; set; }
		public UserProfileId UserProfileId { get; set; }
		public EMail EMail { get; set; }

		public ChangeUserProfileEMail(UserId requesterId, UserProfileId userProfileId, EMail eMail) : base()
		{
			RequesterId = requesterId;
			UserProfileId = userProfileId;
			EMail = eMail;
		}
	}

	public class ChangeUserProfileAddress : IRequest<Result>
	{
		public UserId RequesterId { get; set; }
		public UserProfileId UserProfileId { get; set; }
		public Address Address { get; set; }

		public ChangeUserProfileAddress(UserId requesterId, UserProfileId userProfileId, Address address) : base()
		{
			RequesterId = requesterId;
			UserProfileId = userProfileId;
			Address = address;
		}
	}

	public class ChangeUserProfileDisplayName : IRequest<Result>
	{
		public UserId RequesterId { get; set; }
		public UserProfileId UserProfileId { get; set; }
		public Name Name { get; set; }

		public ChangeUserProfileDisplayName(UserId requesterId, UserProfileId userProfileId, Name name) : base()
		{
			RequesterId = requesterId;
			UserProfileId = userProfileId;
			Name = name;
		}
	}

	public class ChangeUserProfileFullName : IRequest<Result>
	{
		public UserId RequesterId { get; set; }
		public UserProfileId UserProfileId { get; set; }
		public Name Name { get; set; }

		public ChangeUserProfileFullName(UserId requesterId, UserProfileId userProfileId, Name name) : base()
		{
			RequesterId = requesterId;
			UserProfileId = userProfileId;
			Name = name;
		}
	}

	public class ChangeUserProfileAvatar : IRequest<Result>
	{
		public UserId RequesterId { get; set; }
		public UserProfileId UserProfileId { get; set; }
		public FormFile File { get; set; }

		public ChangeUserProfileAvatar(UserId requesterId, UserProfileId userProfileId, FormFile file) : base()
		{
			RequesterId = requesterId;
			UserProfileId = userProfileId;
			File = file;
		}
	}

	public class ChangeUserProfileTagline : IRequest<Result>
	{
		public UserId RequesterId { get; set; }
		public UserProfileId UserProfileId { get; set; }
		public string TagLine { get; set; }

		public ChangeUserProfileTagline(UserId requesterId, UserProfileId userProfileId, string tagLine) : base()
		{
			RequesterId = requesterId;
			UserProfileId = userProfileId;
			TagLine = tagLine;
		}
	}

	public class ChangeUserProfileDescription : IRequest<Result>
	{
		public UserId RequesterId { get; set; }
		public UserProfileId UserProfileId { get; set; }
		public Description Description { get; set; }

		public ChangeUserProfileDescription(UserId requesterId, UserProfileId userProfileId, Description description) : base()
		{
			RequesterId = requesterId;
			UserProfileId = userProfileId;
			Description = description;
		}
	}

	public class ChangeUserProfilePrivacy : IRequest<Result>
	{
		public UserId RequesterId { get; set; }
		public UserProfileId UserProfileId { get; set; }
		public PrivacyLevel Level { get; set; }

		public ChangeUserProfilePrivacy(UserId requesterId, UserProfileId userProfileId, PrivacyLevel level) : base()
		{
			RequesterId = requesterId;
			UserProfileId = userProfileId;
			Level = level;
		}
	}

	public class DisableUserProfileNotificationEmail : IRequest<Result>
	{
		public UserId RequesterId { get; set; }
		public UserProfileId UserProfileId { get; set; }

		public DisableUserProfileNotificationEmail(UserId requesterId, UserProfileId userProfileId) : base()
		{
			RequesterId = requesterId;
			UserProfileId = userProfileId;
		}
	}

	public class EnableUserProfileNotificationEmail : IRequest<Result>
	{
		public UserId RequesterId { get; set; }
		public UserProfileId UserProfileId { get; set; }

		public EnableUserProfileNotificationEmail(UserId requesterId, UserProfileId userProfileId) : base()
		{
			RequesterId = requesterId;
			UserProfileId = userProfileId;
		}
	}

	public class ChangeUserProfileNotificationEmailInterval : IRequest<Result>
	{
		public UserId RequesterId { get; set; }
		public UserProfileId UserProfileId { get; set; }
		public TimeSpan Interval { get; set; }

		public ChangeUserProfileNotificationEmailInterval(UserId requesterId, UserProfileId userProfileId, TimeSpan interval) : base()
		{
			RequesterId = requesterId;
			UserProfileId = userProfileId;
			Interval = interval;
		}
	}

}
