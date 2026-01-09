using System;
using Marten;
using System.Linq;
using Baseline;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.UserProfiles;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.UserProfiles;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Membership.Infrastructure
{
    public static class M20170103_MigrateOldUsersMigration
    {
        public class IdOnly
        {
            public Guid Id { get; set; }
        }

        private static readonly DateTime registrationDate = new DateTime(2016, 10, 01); //begin of web-site

        private const string sqlDisplayNameNull = @"select u.data
from psychedelics.mt_doc_user u
left join psychedelics.mt_doc_views_userprofile up on u.id = up.id
left join psychedelics_events.mt_events e on e.type = 'user_registered' and CAST(e.data->> 'Id' as uuid) = up.id
where u.data->> 'DisplayName' is null";

        private const string sqlNoProfile = @"select u.data
from psychedelics.mt_doc_user u
left join psychedelics.mt_doc_views_userprofile up on u.id = up.id
left join psychedelics_events.mt_events e on e.type = 'user_registered' and CAST(e.data->> 'Id' as uuid) = up.id
where up.id is null or e.id is null";

        public static IDocumentSession M20170103_MigrateOldUsers(this IDocumentSession session)
        {
            return session
                .UpdateDisplayNames()
                .CreateProfile();
        }

        private static IDocumentSession UpdateDisplayNames(this IDocumentSession session)
        {
            var usersWithoutProfile = session.Query<IdOnly>(sqlDisplayNameNull);
            foreach (var userId in usersWithoutProfile)
            {
                var user = session.Load<User>(userId.Id);
                user.DisplayName = user.UserName;

                session.Store(user);
            }
            return session;
        }

        private static IDocumentSession CreateProfile(this IDocumentSession session)
        {
            var users = session.Query<IdOnly>(sqlNoProfile);
            foreach (var userId in users)
            {
                var user = session.Load<User>(userId.Id);
                
                var @event = new UserRegistered(
                    new UserId(user.Id),
                    null,
                    LoginType.UserName,
                    new Name(user.DisplayName),
                    null,
                    new EMail(user.Email),
                    registrationDate
                    );

                session.Events.Append(user.Id, @event);

                session.AddUserProfile(user);
            }
            return session;
        }

        private static void AddUserProfile(this IDocumentSession session, User user)
        {
            var userProfile = new UserProfile();
            var profileCommand = new CreateUserProfile(
                null,
                new UserProfileId(user.Id),
                new Name(user.DisplayName),
                null,
                new EMail(user.Email));

            userProfile.Handle(null, profileCommand);
            userProfile.GetChanges()
                .OfType<Event>()
                .Each(@event => @event.EventTimestamp = registrationDate);

            session.StoreChanges(userProfile);
        }
    }
}