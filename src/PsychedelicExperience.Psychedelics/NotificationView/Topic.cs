using System;

namespace PsychedelicExperience.Psychedelics.NotificationView
{
    public class Topic
    {
        public TopicType TopicType { get; set; }
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }

        public string TypeText()
        {
            switch (TopicType)
            {
                case TopicType.Experience:
                case TopicType.Organisation:
                case TopicType.Event:
                case TopicType.Document:
                    return TopicType.ToString().ToLowerInvariant();

                case TopicType.OrganisationReview:
                    return "review";

                case TopicType.OrganisationUpdate:
                    return "update";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string Url()
        {
            switch (TopicType)
            {
                case TopicType.Experience:
                case TopicType.Organisation:
                case TopicType.Event:
                case TopicType.Document:
                    return $"/{TopicType.ToString().ToLowerInvariant()}/{Id}";

                case TopicType.OrganisationReview:
                    return $"/organisation/{ParentId}/review/{Id}";

                case TopicType.OrganisationUpdate:
                    return $"/organisation/{ParentId}/update/{Id}";

                default:
                    throw new ArgumentOutOfRangeException();
            }            
        }
        
    }
}