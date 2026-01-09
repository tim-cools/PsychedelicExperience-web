using System;
using Marten;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Membership.UserContactLog
{
    public interface IUserContactLogger
    {
        void Log(UserId userId, EMail email, string purpose, string data);
        void Log(UserId userId, EMail email, string source, Guid sourceId, string purpose, string data);
    }

    public class UserContactLogger : IUserContactLogger
    {
        private readonly IDocumentSession _documentSession;

        public UserContactLogger(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public void Log(UserId userId, EMail email, string purpose, string data)
        {
            _documentSession.Store(new UserContactLog
            {
                DateTime = DateTime.Now,
                UserId = userId,
                EMail = email.Normalize(),
                Purpose = purpose,
                Data = data
            });
        }

        public void Log(UserId userId, EMail email, string source, Guid sourceId, string purpose, string data)
        {
            _documentSession.Store(new UserContactLog
            {
                DateTime = DateTime.Now,
                UserId = userId,
                EMail = email.Normalize(),
                Source = source,
                SourceId = sourceId,
                Purpose = purpose,
                Data = data
            });
        }
    }
}