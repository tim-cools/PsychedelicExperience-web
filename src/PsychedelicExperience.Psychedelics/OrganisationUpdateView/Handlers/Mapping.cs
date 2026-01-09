using System;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Queries;
using PsychedelicExperience.Psychedelics.OrganisationView;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.OrganisationUpdateView.Handlers
{
    internal static class Mapping
    {
        internal static OrganisationUpdateSummary MapSummary(this OrganisationUpdate update, User user, Organisation organisation, IUserInfoResolver userInfoResolver)
        {
            if (organisation == null) throw new ArgumentNullException(nameof(organisation));
            if (userInfoResolver == null) throw new ArgumentNullException(nameof(userInfoResolver));

            var createdUser = userInfoResolver.GetInfo(update.CreatedBy);
            var updatedUser = userInfoResolver.GetInfo(update.LastUpdatedBy);

            return new OrganisationUpdateSummary
            {
                OrganisationUpdateId  = update.Id,
                Url  = update.GetUrl(),

                OrganisationId  = (Guid) organisation.Id,
                OrganisationName  = organisation.Name,
                OrganisationUrl  = organisation.GetUrl(),

                PrivacyLevel  = update.Privacy.ToString(),

                Created = update.Created,
                CreatedUserName = createdUser.DisplayName,
                CreatedUserId = createdUser.UserId,

                LastUpdated = update.LastUpdated,
                LastUpdatedUserName = updatedUser.DisplayName,
                LastUpdatedUserId = updatedUser.UserId,

                Subject  = update.Subject,
                Content  = update.Content
            };
        }

        internal static OrganisationUpdateDetails MapDetails(this OrganisationUpdate update, User user, Organisation organisation, IUserInfoResolver userInfoResolver, bool isMember, bool isAdministrator)
        {
            if (update == null) throw new ArgumentNullException(nameof(update));
            if (organisation == null) throw new ArgumentNullException(nameof(organisation));

            if (!update.CanView(isMember, isAdministrator))
            {
                return null;
            }

            var isOwner = organisation.IsOwner(user);
            var privileges = new OrganisationUpdateDetailsPrivileges
            {
                Editable = isOwner || user != null && user.IsAdministrator(),
            };

            var createdUser = userInfoResolver.GetInfo(update.CreatedBy);
            var updatedUser = userInfoResolver.GetInfo(update.LastUpdatedBy);

            return new OrganisationUpdateDetails
            {
                OrganisationUpdateId = update.Id,
                Url = update.GetUrl(),

                OrganisationId = (Guid)organisation.Id,
                OrganisationName = organisation.Name,
                OrganisationUrl = organisation.GetUrl(),

                PrivacyLevel = update.Privacy.ToString(),

                Created = update.Created,
                CreatedUserName = createdUser.DisplayName,
                CreatedUserId = createdUser.UserId,

                LastUpdated = update.LastUpdated,
                LastUpdatedUserName = updatedUser.DisplayName,
                LastUpdatedUserId = updatedUser.UserId,

                Subject = update.Subject,
                Content = update.Content,
                Privileges = privileges
            };
        }
    }
}