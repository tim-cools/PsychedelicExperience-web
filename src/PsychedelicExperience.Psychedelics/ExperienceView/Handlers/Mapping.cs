using System;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Security;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Security;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;
using PsychedelicExperience.Psychedelics.TopicInteractionView;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.ExperienceView.Handlers
{
    internal static class Mapping
    {
        internal static ExperienceSummary MapSummary(this Experience experience, TopicInteraction interaction, IUserInfoResolver userInfoResolver)
        {
            if (experience == null) throw new ArgumentNullException(nameof(experience));
            if (userInfoResolver == null) throw new ArgumentNullException(nameof(userInfoResolver));

            var userInfo = userInfoResolver.GetInfo((UserId) experience.UserId);

            return new ExperienceSummary
            {
                ExperienceId = experience.Id,

                UserId = experience.UserId,
                UserName = userInfo.DisplayName,
                Url = GetUrl(experience),

                Created = experience.Created,

                Title = experience.Title,
                Substances = experience.Doses.Select(dose => dose.Substance).ToArray(),
                Tags = experience.Tags.ToArray(),

                PrivacyLevel = experience.PrivacyLevel.ToString(),
                DateTime = experience.DateTime,

                Likes = interaction?.Likes ?? 0,
                Dislikes = interaction?.Dislikes ?? 0,
                Views = interaction?.Views ?? 0,
                CommentCount = interaction?.CommentCount ?? 0,
            };
        }

        private static string GetUrl(Experience experience)
        {
            return $"/experience/{new ShortGuid(experience.Id)}/{experience.Slug()}";
        }

        internal static ExperienceDetails MapDetails(this Experience experience, User user, IUserInfoResolver userInfoResolver, IUserDataProtector userDataProtector)
        {
            if (experience == null) throw new ArgumentNullException(nameof(experience));
            if (userInfoResolver == null) throw new ArgumentNullException(nameof(userInfoResolver));
            if (userDataProtector == null) throw new ArgumentNullException(nameof(userDataProtector));

            if (!experience.CanView(user))
            {
                return null;
            }

            var isOwner = experience.IsOwner(user);
            var canEdit = isOwner || user != null && user.IsAdministrator();
            var isRestricted = experience.IsRestricted(user);
            var privileges = GetPrivileges(isOwner, canEdit, isRestricted);
            var owner = userInfoResolver.GetInfo((UserId) experience.UserId);

            return new ExperienceDetails
            {
                ExperienceId = experience.Id,

                UserId = experience.UserId,
                UserName = owner.DisplayName,
                ExternalDescription = ExternalDescription(experience),
                Slug = experience.Slug(),
                Url = GetUrl(experience),

                Title = experience.Title,
                DateTime = OnlyWhenVisible(privileges.DateTime, experience.DateTime, null),
                Level = experience.Level,
                Partner = experience.Partners.SingleOrDefault(),

                Tags = experience.Tags.Select(tag => tag).ToArray(),
                Doses = experience.Doses.Select(MapDose).ToArray(),

                PrivacyLevel = OnlyWhenVisible(privileges.PrivacyLevel, experience.PrivacyLevel.ToString().ToLowerInvariant()),
                PrivateNotes = EncryptWhenVisible(privileges.PrivateNotes, user, experience.PrivateDescription, userDataProtector),
                PublicDescription = OnlyWhenVisible(privileges.PublicDescription, experience.PublicDescription),
                Set = OnlyWhenVisible(privileges.Set, experience.SetDescription),
                Setting = OnlyWhenVisible(privileges.Setting, experience.SettingDescription),
                Privileges = privileges
            };
        }

        private static string ExternalDescription(Experience experience)
        {
            var substances = experience.Doses.Select(dose => dose.Substance)
                .Distinct()
                .Aggregate((string) null, (result, item) => result == null ? item : $"{result}, {item}");

            if (substances != null)
            {
                substances = substances + ": ";
            }

            return $"{substances}{experience.PublicDescription}".SmartTruncate(Seo.ExternalDescriptionLength);
        }

        private static Messages.Experiences.Queries.Dose MapDose(Dose arg)
        {
            return new Messages.Experiences.Queries.Dose
            {
                Id = arg.Id,
                TimeOffset = arg.TimeOffset,

                Substance = arg.Substance,
                Form = arg.Form,

                Amount = arg.Amount,
                Unit = arg.Unit,
                Method = arg.Method,
                Notes = arg.Notes,
            };
        }

        private static string EncryptWhenVisible(FieldPrivilege fieldPrivilege, User user, EncryptedString value, IUserDataProtector userDataProtector)
        {
            if (user == null || !fieldPrivilege.Visible) return null;

            return userDataProtector.Decrypt((UserId) user.Id, value);
        }

        private static T OnlyWhenVisible<T>(FieldPrivilege fieldPrivilege, T value, T defaultValue)
        {
            return fieldPrivilege.Visible ? value : defaultValue;
        }

        private static string OnlyWhenVisible(FieldPrivilege fieldPrivilege, string value) => OnlyWhenVisible(fieldPrivilege, value, null);

        private static ExperienceDetailsPrivileges GetPrivileges(bool isOwner, bool canEdit, bool isRestrected)
        {
            return new ExperienceDetailsPrivileges
            {
                IsOwner = isOwner,
                Remove = canEdit,
                Report = true,
                Editable = canEdit,
                Title = new FieldPrivilege(true, canEdit),
                DateTime = new FieldPrivilege(!isRestrected, canEdit),
                Level = new FieldPrivilege(true, canEdit),
                PublicDescription = new FieldPrivilege(true, canEdit),
                PrivateNotes = new FieldPrivilege(isOwner, isOwner),
                Set = new FieldPrivilege(!isRestrected, canEdit),
                Setting = new FieldPrivilege(!isRestrected, canEdit),
                PrivacyLevel = new FieldPrivilege(true, canEdit),
                Tags = new FieldPrivilege(true, canEdit),
                Doses = new FieldPrivilege(true, canEdit)
            };
        }
    }
}