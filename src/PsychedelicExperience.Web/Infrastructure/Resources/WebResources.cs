using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Web.Infrastructure.Resources
{
    public static class WebResources
    {
        private static readonly Lazy<byte[]> _defaultAvatar =
            new Lazy<byte[]>(() => typeof(WebResources).ReadResourceData("avatar-default-80x80.png"));
        private static readonly Lazy<byte[]> _defaultDocumentImage =
            new Lazy<byte[]>(() => typeof(WebResources).ReadResourceData("document-default.png"));
        private static readonly Lazy<byte[]> _defaultEventImageSmall =
            new Lazy<byte[]>(() => typeof(WebResources).ReadResourceData("event-default-small.png"));
        private static readonly Lazy<byte[]> _defaultEventImage =
            new Lazy<byte[]>(() => typeof(WebResources).ReadResourceData("event-default.png"));
        private static readonly Lazy<byte[]> _defaultExperienceSocialImage =
            new Lazy<byte[]>(() => typeof(WebResources).ReadResourceData("experience-default-social.png"));
        private static readonly Lazy<byte[]> _templateExperienceSocialImage =
            new Lazy<byte[]>(() => typeof(WebResources).ReadResourceData("experience-social-template.png"));
        private static readonly Lazy<byte[]> _templateExperienceTypmSocialImage =
            new Lazy<byte[]>(() => typeof(WebResources).ReadResourceData("experience-typm-social-template.png"));

        public static byte[] DefaultAvatar => _defaultAvatar.Value;
        public static byte[] DefaultDocumentImage => _defaultDocumentImage.Value;
        public static byte[] DefaultEventImageSmall => _defaultEventImageSmall.Value;
        public static byte[] DefaultEventImage => _defaultEventImage.Value;

        public static byte[] DefaultExperienceSocialImage => _defaultExperienceSocialImage.Value;
        public static byte[] TemplateExperienceSocialImage => _templateExperienceSocialImage.Value;
        public static byte[] TemplateExperienceTypmSocialImage => _templateExperienceTypmSocialImage.Value;
    }
}