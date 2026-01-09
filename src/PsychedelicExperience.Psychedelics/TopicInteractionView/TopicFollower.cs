using System;

namespace PsychedelicExperience.Psychedelics.TopicInteractionView
{
    public class TopicFollower
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TopicId { get; set; }
        public DateTime Since { get; set; }
    }
}