using System;
using System.Collections.Generic;
using PsychedelicExperience.Membership.Users.Domain;

namespace PsychedelicExperience.Psychedelics.TopicInteractionView
{
    public class TopicInteraction
    {
        public Guid Id { get; set; }

        public InteractionType InteractionType { get; set; }

        public int Followers { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Views { get; set; }
        public int CommentCount { get; set; }

        public bool Public { get; set; }

        public List<Guid> Owners { get; set; } = new List<Guid>();
        public List<TopicComment> Comments { get; set; } = new List<TopicComment>();

        public DateTime? LastUpdated { get; set; }

        public bool IsOwner(User user)
        {
            return user != null && Owners.Contains(user.Id);
        }

        internal TopicPemission GetPermissions(User user)
        {
            switch (InteractionType)
            {
                case InteractionType.None:
                    return null;

                case InteractionType.Experience:

                    return ExperiencePermissions(user);

                case InteractionType.Organisation:
                case InteractionType.OrganisationReview:
                    return OrganisationPemission(user);

                case InteractionType.OrganisationUpdate:
                    return OrganisationUpdatePemission(user);

                case InteractionType.Document:
                    return DocumentPermissions(user);

                case InteractionType.Event:
                    return EventPermissions(user);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private TopicPemission OrganisationPemission(User user)
        {
            var manage = user != null && (user.IsAdministrator() || IsOwner(user));

            return new TopicPemission(true, true, manage);
        }

        //todo check is MembersOnly and Owner (onwers not filled in case of an Update)
        private TopicPemission OrganisationUpdatePemission(User user)
        {
            var manage = user != null && (user.IsAdministrator() || IsOwner(user));
            var allowed = Public || manage;

            return new TopicPemission(true, true, manage);
        }

        private TopicPemission ExperiencePermissions(User user)
        {
            var manage = user != null && (user.IsAdministrator() || IsOwner(user));
            var allowed = Public || manage;

            return new TopicPemission(allowed, allowed, manage);
        }

        private TopicPemission DocumentPermissions(User user)
        {
            var manage = user != null && user.IsAtLeast(Roles.ContentManager);
            var allowed = Public || manage;

            return new TopicPemission(allowed, allowed, manage);
        }

        //todo check is MembersOnly
        private TopicPemission EventPermissions(User user)
        {
            var manage = user != null && user.IsAtLeast(Roles.ContentManager);
            var allowed = Public || manage;

            return new TopicPemission(true, true, manage);
        }
    }

    public enum InteractionType
    {
        None,
        Experience,
        Organisation,
        OrganisationReview,
        OrganisationUpdate,
        Document,
        Event
    }

    internal class TopicPemission
    {
        public bool Manage { get; }
        public bool ViewComments { get; }
        public bool View { get; }

        public TopicPemission(bool viewComments, bool view, bool manage)
        {
            ViewComments = viewComments;
            View = view;
            Manage = manage;
        }
    }
}