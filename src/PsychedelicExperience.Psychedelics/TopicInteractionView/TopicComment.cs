using System;
using PsychedelicExperience.Common.Security;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Security;

namespace PsychedelicExperience.Psychedelics.TopicInteractionView
{
    public class TopicComment
    {
        public DateTime Timestamp { get; set; }
        public UserId UserId { get; set; }
        public EncryptedString Text { get; set; }
    }
}