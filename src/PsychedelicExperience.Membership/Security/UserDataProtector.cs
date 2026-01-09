using System;
using System.Security.Cryptography;
using Marten;
using Microsoft.Extensions.Logging;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Security;
using PsychedelicExperience.Membership.Messages.Users;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Membership.Security
{
    public class UserDataProtector : IUserDataProtector
    {
        private readonly IDocumentSession _documentSession;
        private readonly ILogger<UserDataProtector> _logger;

        public UserDataProtector(IDocumentSession documentSession, ILogger<UserDataProtector> logger)
        {
            _documentSession = documentSession ?? throw new ArgumentNullException(nameof(documentSession));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public EncryptedString Encrypt(UserId userId, string value)
        {
            var user = GetUser(userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found: " + userId);
            }
            return user.EncryptionKey.Encrypt(value);
        }

        public string Decrypt(UserId userId, EncryptedString value)
        {
            var user = GetUser(userId);
            try
            {
                return user?.EncryptionKey.Decrypt(value);
            }
            catch (CryptographicException exception)
            {
                _logger.LogWarningMethod(nameof(Decrypt), exception);
                return "encryption error";
            }
        }

        private User GetUser(UserId userId)
        {
            var user = userId != null
                ? _documentSession.Load<User>(userId.Value)
                : null;

            if (user == null) return null;

            if (user.EnsureKey())
            {
                _documentSession.Store(user);
            }
            return user;
        }
    }
}