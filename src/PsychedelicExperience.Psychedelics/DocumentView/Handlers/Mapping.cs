using System;
using System.Linq;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Security;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Messages.Documents.Queries;
using PsychedelicExperience.Psychedelics.TopicInteractionView;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.DocumentView.Handlers
{
    internal static class Mapping
    {
        internal static DocumentSummary MapSummary(this Document document, User user, TopicInteraction interaction, IUserInfoResolver userInfoResolver)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (userInfoResolver == null) throw new ArgumentNullException(nameof(userInfoResolver));

            var userInfo = userInfoResolver.GetInfo((UserId) document.UserId);
            var lastUpdatedUserId = userInfoResolver.GetInfo((UserId) document.LastUpdatedUserId);

            return new DocumentSummary
            {
                DocumentId = document.Id,

                UserId = document.UserId,
                UserName = userInfo.DisplayName,
                LastUpdatedUserId = document.LastUpdatedUserId,
                LastUpdatedUserName = lastUpdatedUserId.DisplayName,

                CanEdit = document.CanEdit(user),
                Url = $"/{document.Slug}",

                Created = document.Created,
                Published = document.Published,
                LastUpdated = document.LastUpdated,
                Type = document.DocumentType.ToString(),
                Status = document.Status.ToString(),

                Name = document.Name,
                Tags = document.Tags.Select(tag => tag).ToArray(),

                Likes = interaction?.Likes ?? 0,
                Dislikes = interaction?.Dislikes ?? 0,
                Views = interaction?.Views ?? 0,
                CommentCount = interaction?.CommentCount ?? 0,
            };
        }

        internal static DocumentDetails MapDetails(this Document document, User user, IUserInfoResolver userInfoResolver, IUserDataProtector userDataProtector)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (userInfoResolver == null) throw new ArgumentNullException(nameof(userInfoResolver));
            if (userDataProtector == null) throw new ArgumentNullException(nameof(userDataProtector));

            if (!document.CanView(user))
            {
                return null;
            }

            var canEdit = document.CanEdit(user);
            var userInfo = userInfoResolver.GetInfo((UserId)document.UserId);
            var lastUpdatedUserId = userInfoResolver.GetInfo((UserId)document.LastUpdatedUserId);

            var privileges = new DocumentDetailsPrivileges
            {
                Editable = canEdit,
                Removable = canEdit
            };

            return new DocumentDetails
            {
                DocumentId = document.Id,

                Status = document.Status.ToString(),

                Created = document.Created,
                CreatedUserId = userInfo.UserId,
                CreatedUserName = userInfo.DisplayName,

                LastUpdated = document.LastUpdated,
                LastUpdatedUserName = lastUpdatedUserId.DisplayName,
                LastUpdatedUserId = lastUpdatedUserId.UserId,

                Slug = document.Slug,
                Url = $"/{document.Slug}",

                Name = document.Name,
                Content = document.Content,
                ExternalDescription = document.ExternalDescription,

                Tags = document.Tags.Select(tag => tag).ToArray(),

                Privileges = privileges
            };
        }
    }
}