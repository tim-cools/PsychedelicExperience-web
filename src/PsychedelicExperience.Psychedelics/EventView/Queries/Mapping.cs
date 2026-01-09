using System;
using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.Events.Queries;
using PsychedelicExperience.Psychedelics.OrganisationView;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.EventView.Queries
{
    internal static class Mapping
    {
        internal static EventSummary MapSummary(this Event @event, Organisation organisation, User user, bool isOrganisationMember, EventMember eventMember,  IUserInfoResolver userInfoResolver)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));
            if (userInfoResolver == null) throw new ArgumentNullException(nameof(userInfoResolver));

            var isOwner = organisation?.IsOwner(user) == true;
            if (!@event.CanView(user, isOrganisationMember, isOwner))
            {
                return null;
            }

            var userInfo = userInfoResolver.GetInfo((UserId)@event.UserId);
            var lastUpdatedUserId = userInfoResolver.GetInfo((UserId)@event.LastUpdatedUserId);

            return new EventSummary
            {
                EventId = @event.Id,

                UserId = @event.UserId,

                OrganisationId = organisation?.Id,
                OrganisationName = organisation?.Name,
                OrganisationUrl = organisation?.GetUrl(),

                UserName = userInfo.DisplayName,
                LastUpdatedUserId = @event.LastUpdatedUserId,
                LastUpdatedUserName = lastUpdatedUserId.DisplayName,

                Url = $"/event/{new ShortGuid(@event.Id)}/{@event.Slug()}",

                Created = @event.Created,
                Published = @event.Published,
                LastUpdated = @event.LastUpdated,
                Type = @event.EventType.ToString(),
                Privacy = @event.Privacy.ToString(),

                Name = @event.Name,
                Address = @event.Address?.Name,
                Tags = @event.Tags.Select(tag => tag).ToArray(),
                StartDateTime = @event.StartDateTime,
                EndDateTime = @event.EndDateTime,

                Members = MapMembers(@event, eventMember)
            };
        }

        private static Messages.Events.Queries.Members MapMembers(Event @event, EventMember eventMember)
        {
            return new Messages.Events.Queries.Members
            {
                Status = eventMember?.Status.ToString(),
                Attending = @event.Members?.Attending ?? 0,
                Interested = @event.Members?.Interested ?? 0,
                Invited = @event.Members?.Invited ?? 0,
                NotAttending = @event.Members?.NotAttending ?? 0,
            };
        }

        internal static EventDetails MapDetails(this Event @event, Organisation organisation, User user, bool isOrganisationMember, EventMember eventMember, IUserInfoResolver userInfoResolver)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));
            if (userInfoResolver == null) throw new ArgumentNullException(nameof(userInfoResolver));

            var isOwner = organisation?.IsOwner(user) == true;
            if (!@event.CanView(user, isOrganisationMember, isOwner))
            {
                return null;
            }

            var canEdit = @event.CanEdit(user, isOwner);
            var userInfo = userInfoResolver.GetInfo((UserId)@event.UserId);
            var lastUpdatedUserId = userInfoResolver.GetInfo((UserId)@event.LastUpdatedUserId);

            var privileges = new EventDetailsPrivileges
            {
                Editable = canEdit,
                Removable = canEdit
            };

            return new EventDetails
            {
                EventId = @event.Id,

                OrganisationId = organisation?.Id,
                OrganisationName = organisation?.Name,
                OrganisationUrl = organisation?.GetUrl(),

                Created = @event.Created,
                CreatedUserId = userInfo.UserId,
                CreatedUserName = userInfo.DisplayName,

                LastUpdated = @event.LastUpdated,
                LastUpdatedUserName = lastUpdatedUserId.DisplayName,
                LastUpdatedUserId = lastUpdatedUserId.UserId,

                StartDateTime = @event.StartDateTime,
                EndDateTime = @event.EndDateTime,
                Privacy = @event.Privacy.ToString(),
                EventType = @event.EventType.ToString(),

                Name = @event.Name,
                Description = @event.Description,
                LocationName = @event.LocationName,
                Address = MapAddress(@event),
                Tags = @event.Tags.Select(tag => tag).ToArray(),
                Url = GetUrl(@event),

                Privileges = privileges,

                Members = MapMembers(@event, eventMember)
            };
        }

        private static string GetUrl(Event @event)
        {
            return $"/event/{new ShortGuid(@event.Id)}/{@event.Slug()}";
        }

        private static Messages.Events.Queries.EventAddress MapAddress(Event @event)
        {
            return @event.Address != null
                ? new Messages.Events.Queries.EventAddress
                {
                    Name = @event.Address.Name,
                    Position = @event.Address.Position != null
                        ? new Position
                        {
                            Latitude = @event.Address.Position.Latitude,
                            Longitude = @event.Address.Position.Longitude
                        }
                        : null,
                    PlaceId = @event.Address.PlaceId
                }
                : null;
        }   

        internal static Messages.Events.Queries.EventMember[] MapMembers(this IReadOnlyList<EventMember> eventMembers, IUserInfoResolver userInfoResolver)
        {
            return eventMembers.Select(member => new Messages.Events.Queries.EventMember
            {
                Status = member.Status.ToString(),
                UserId = member.MemberId,
                UserName = userInfoResolver.GetInfo(new UserId(member.MemberId)).DisplayName
            }).ToArray();
        }
    }
}