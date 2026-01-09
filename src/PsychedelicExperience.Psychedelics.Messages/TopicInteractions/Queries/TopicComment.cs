using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.TopicInteractions.Queries
{
    public class TopicComment
    {
        public DateTime Timestamp { get; set; }

        public ShortGuid UserId { get; set; }
        public string UserName { get; set; }

        public string Text { get; set; }
    }
}