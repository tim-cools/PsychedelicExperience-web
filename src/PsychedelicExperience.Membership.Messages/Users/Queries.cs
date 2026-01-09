using System;
using PsychedelicExperience.Common.Messages;

namespace PsychedelicExperience.Membership.Messages.Users
{
    public class UserByIdQuery : IRequest<User>
    {
        public UserId Id { get; }
        public UserId AuthenticatedUserId { get; }

        public UserByIdQuery(UserId id, UserId authenticatedUserId)
        {
            Id = id;
            AuthenticatedUserId = authenticatedUserId;
        }
    }

    public class UserByEMailQuery : IRequest<User>
    {
        public EMail EMail { get; }
        public UserId AuthenticatedUserId { get; }

        public UserByEMailQuery(UserId authenticatedUserId, EMail eMail)
        {
            AuthenticatedUserId = authenticatedUserId;
            EMail = eMail;
        }
    }

    public class UserAvatarByIdQuery : IRequest<UserAvatar>
    {
        public Guid Id { get; }

        public UserAvatarByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}