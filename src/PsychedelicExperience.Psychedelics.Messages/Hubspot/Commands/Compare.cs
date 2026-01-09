using System;
using System.IO;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Psychedelics.Messages.Hubspot.Commands
{
    public class Compare : IRequest<CompareResult>
    {
        public UserId CurrentUserId { get; }

        public Compare(UserId currentUserId)
        {
            CurrentUserId = currentUserId;
        }
    }

    public class CompareResult : Result
    {
        private readonly StringWriter _log = new StringWriter();

        public string Result => _log.ToString();

        public CompareResult()
        {
        }

        public CompareResult(bool success, string errorcode) : base(success, new ValidationError(null, errorcode, null))
        {
        }

        public CompareResult(bool success) : base(success)
        {
        }

        public void Log(string reason, Guid? id, string name,  string comment = null)
        {
            _log.WriteLine($"Warning: {reason}: {id} ({name}) - {comment}");
        }
    }
}