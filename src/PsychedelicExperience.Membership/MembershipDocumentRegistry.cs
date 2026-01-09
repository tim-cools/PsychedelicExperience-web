using Marten;
using Marten.Events;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Clients.Domain;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.RefreshTokens.Domain;
using PsychedelicExperience.Membership.UserProfiles;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Membership
{
    public class MembershipDocumentRegistry : MartenRegistry, IInitializeEvents
    {
        public MembershipDocumentRegistry()
        {
            For<User>();
            For<RefreshToken>();
            For<Client>();

            For<UserProfileView.UserProfile>().DocumentAlias("views_userprofile");
        }

        public void Initialize(EventGraph events)
        {
            events.AddEventType(typeof(UserToRoleAdded));
            events.AddEventType(typeof(UserFromRoleRemoved));
            events.AddEventType(typeof(UserEmailConfirmed));
            events.AddEventType(typeof(UserJoinedExperiencesBeta));

            events.ConfigureEventSourced<UserProfile>();
        }
    }
}