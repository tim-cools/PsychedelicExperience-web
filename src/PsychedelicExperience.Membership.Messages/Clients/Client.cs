using System;

namespace PsychedelicExperience.Membership.Messages.Clients
{
    public class Client
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AllowedOrigin { get; set; }
        public string Key { get; set; }
        public string RedirectUri { get; set; }
        public bool Active { get; set; }
    }
}