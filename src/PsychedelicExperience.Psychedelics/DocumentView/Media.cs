using System;
using System.Collections.Generic;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.DocumentView
{
    public class Media
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public DocumentStatus Status { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Published { get; set; }
        public DateTime? LastUpdated { get; set; }

        public IList<string> Tags { get; } = new List<string>();
        public IList<Link> Link { get; } = new List<Link>();
        
        public MediaType MediaType { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public string ExternalDescription { get; set; }
        public string Image { get; set; }

        public bool IsOwner(User user)
        {
            return user != null && user.Id == UserId;
        }

        public bool CanView(User user)
        {
            return Status == DocumentStatus.Published
                || IsOwner(user);
        }
    }

    public class Link
    {
        public string Url { get; set; }
        public Type Type { get; set; }
    }

    public enum MediaType
    {
        Article,
        Substance,
        Person,
        Book,
        Documentary,
        Concepts,
        Effect,
        WebSite,
        Research
    }
}