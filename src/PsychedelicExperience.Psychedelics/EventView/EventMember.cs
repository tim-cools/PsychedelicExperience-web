using System;
using PsychedelicExperience.Membership.Users.Domain;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.EventView
{
    public class EventMember
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid MemberId { get; set; }
        public Guid AddedUserId { get; set; }

        public EventMemberStatus Status { get; set; }

        public bool CanEdit(User user, bool isOwner)
        {
            return user != null && (user.Id == MemberId || user.IsAtLeast(Roles.ContentManager));
        }
    }

    public enum EventMemberStatus
    {
        Invited,
        Attending,
        NotAttending,
        Interested,
        Ignored,
        Removed
    }
}