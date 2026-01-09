using System;
using System.Collections.Generic;

namespace PsychedelicExperience.Psychedelics.NotificationView
{
    public class NotificationTopicStatus
    {
        public Guid Id { get; set; }
        public Topic Topic { get; set; }
        public string Name { get; set; }
        public DateTime LastUpdated { get; set; }
        public IList<Guid> Owners { get; } = new List<Guid>();
        public IList<Guid> Followers { get; } = new List<Guid>();
    }
}