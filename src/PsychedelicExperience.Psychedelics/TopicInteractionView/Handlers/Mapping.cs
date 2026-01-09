using System;
using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Security;
using PsychedelicExperience.Membership.UserInfo;
using PsychedelicExperience.Psychedelics.Messages.TopicInteractions.Queries;
using PsychedelicExperience.Psychedelics.UserInteractions;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.TopicInteractionView.Handlers
{
    internal static class Mapping
    {
        internal static TopicInteractionDetails MapDetails(this TopicInteraction interaction, User user, TopicPemission permissions, UserInteraction userInteraction, IUserInfoResolver userInfoResolver, IUserDataProtector userDataProtector)
        {
            if (userInfoResolver == null) throw new ArgumentNullException(nameof(userInfoResolver));
            if (userDataProtector == null) throw new ArgumentNullException(nameof(userDataProtector));

            if (permissions?.View != true) return null;

            return new TopicInteractionDetails
            {
                CanInteract = user != null,

                HasLiked = userInteraction != null && userInteraction.Opinion == Opinion.Like,
                HasDisliked = userInteraction != null && userInteraction.Opinion ==  Opinion.Dislike,
                Following = userInteraction != null && userInteraction.Followed,

                Followers = interaction?.Followers ?? 0,
                Likes = interaction?.Likes ?? 0,
                Dislikes = interaction?.Dislikes ?? 0,
                LastUpdated = interaction?.LastUpdated,
                CommentCount = interaction?.CommentCount ?? 0,
                Views = interaction?.Views ?? 0,
                Comments = Comments(interaction, permissions, userInfoResolver, userDataProtector)
            };
        }

        private static Messages.TopicInteractions.Queries.TopicComment[] Comments(TopicInteraction interaction, TopicPemission permissions, IUserInfoResolver userInfoResolver, IUserDataProtector userDataProtector)
        {
            return permissions.ViewComments
                ? interaction?.Comments?.Select(comment => Map(comment, userInfoResolver, userDataProtector)).ToArray()
                : null;
        }

        private static Messages.TopicInteractions.Queries.TopicComment Map(TopicComment comment, IUserInfoResolver userInfoResolver, IUserDataProtector userDataProtector)
        {
            var user = userInfoResolver.GetInfo(comment.UserId);

            return new Messages.TopicInteractions.Queries.TopicComment
            {
                Text = userDataProtector.Decrypt(comment.UserId, comment.Text),
                UserId = comment.UserId.Value,
                UserName = user.DisplayName,
                Timestamp = comment.Timestamp
            };
        }

        internal static TopicFollowersDetails MapFollowers(this TopicInteraction interaction, User user, TopicPemission permissions, IList<TopicFollower> followers, IUserInfoResolver userInfoResolver)
        {
            if (userInfoResolver == null) throw new ArgumentNullException(nameof(userInfoResolver));

            if (permissions?.Manage != true) return null;

            return new TopicFollowersDetails
            {
                Followers = Followers(followers, userInfoResolver)
            };
        }

        private static TopicFollowerDetails[] Followers(IEnumerable<TopicFollower> followers, IUserInfoResolver userInfoResolver)
        {
            return followers.Select(follower => Follower(follower, userInfoResolver)).ToArray();
        }

        private static TopicFollowerDetails Follower(TopicFollower follower, IUserInfoResolver userInfoResolver)
        {
            var user = userInfoResolver.GetInfo((UserId) follower.UserId);

            return new TopicFollowerDetails
            {
                UserId = follower.UserId,
                UserName = user.DisplayName,
                Since = follower.Since
            };
        }
    }
}