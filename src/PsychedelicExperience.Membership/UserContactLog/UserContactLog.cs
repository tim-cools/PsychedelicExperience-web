using System;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;

namespace PsychedelicExperience.Membership.UserContactLog
{
    public class UserContactLog
    {
        public Guid Id { get; protected set; }
        public DateTime DateTime { get; set; }

        public UserId UserId { get; set; }
        public EMail EMail { get; set; }

        public string Source { get; set; }
        public Guid SourceId { get; set; }
        public string Purpose { get; set; }
        public string Data { get; set; }
    }
}