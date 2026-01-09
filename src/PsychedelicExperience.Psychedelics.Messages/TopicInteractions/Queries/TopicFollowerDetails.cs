using System;

namespace PsychedelicExperience.Psychedelics.Messages.TopicInteractions.Queries
{
    public class TopicFollowerDetails
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Since { get; set; }
    }
}