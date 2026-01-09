using System;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Microsoft.AspNetCore.Identity;
using PsychedelicExperience.Common;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Membership
{
    public static class SessionExtensions
    {
        public static async Task<User> LoadUserAsync(this IQuerySession session, UserId userId)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            return userId != null ? await session.LoadAsync<User>(userId.Value) : null;
        }

        public static async Task<User> LoadUserAsync(this IQuerySession session, ILookupNormalizer lookupNormalizer, EMail email)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            var emailValue = email != null ? lookupNormalizer.Normalize(email.Value) : null;

            return email != null ? await session.Query<User>()
                .Where(where => where.NormalizedEmail == emailValue)
                .FirstOrDefaultAsync()
                : null;
        }

        public static async Task<T> LoadAsync<T>(this IQuerySession session, Id id) where T : class
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            return id != null ? await session.LoadAsync<T>(id.Value) : null;
        }

        public static T Load<T>(this IQuerySession session, Id id) where T : class
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            return id != null ? session.Load<T>(id.Value) : null;
        }
    }
}