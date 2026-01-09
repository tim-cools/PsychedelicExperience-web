using System;

namespace PsychedelicExperience.Psychedelics.Messages.TopicInteractions.Queries
{
    public class TopicInteractionDetails
    {
        public int Followers { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Views { get; set; }
        public int CommentCount { get; set; }
        public TopicComment[] Comments { get; set; }

        public DateTime? LastUpdated { get; set; }
        public bool CanInteract { get; set; }
        public bool HasLiked { get; set; }
        public bool HasDisliked { get; set; }
        public bool Following { get; set; }
    }
}