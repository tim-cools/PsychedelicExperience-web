using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.UserInfo;

namespace PsychedelicExperience.Psychedelics.NotificationView
{
    public class MarkupRendererOptions
    {
        public string UrlDomainPrefix { get; }
        public string UtmSource { get; }
        public string UtmMedium { get; }

        public bool IncludeUtm => !string.IsNullOrEmpty(UtmMedium) || !string.IsNullOrEmpty(UtmSource);

        public MarkupRendererOptions(string urlDomainPrefix = null, string utmSource = null, string utmMedium = null)
        {
            UrlDomainPrefix = urlDomainPrefix?.TrimEnd('/');
            UtmSource = utmSource;
            UtmMedium = utmMedium;
        }
    }

    public class MarkupRenderer
    {
        private readonly MarkupRendererOptions _options;

        public MarkupRenderer(MarkupRendererOptions options = null)
        {
            _options = options ?? new MarkupRendererOptions();
        }

        public string Render(Markup markup)
        {
            var stringBuilder = new StringBuilder();
            foreach (MarkupPart part in markup)
            {
                part.Render(stringBuilder, _options);
            }
            return stringBuilder.ToString();
        }
    }

    public class Markup : IEnumerable<MarkupPart>
    {
        private readonly List<MarkupPart> _markupParts = new List<MarkupPart>();

        public Markup User(Guid? changeUser, Guid notificationUser, IUserInfoResolver userInfoResolver, string youText = "You")
        {
            if (!changeUser.HasValue)
            {
                return Text("An anonymous user");
            }

            if (changeUser == notificationUser)
            {
                return Text(youText);
            }

            var user = userInfoResolver.GetInfo(new UserId(changeUser.Value));
            return Link(user.DisplayName, $"/user/{user.UserId}");
        }

        public Markup Text(string text)
        {
            _markupParts.Add(new MarkupText(text));
            return this;
        }

        public Markup Link(string text, string url)
        {
            _markupParts.Add(new MarkupLink(text, url));
            return this;
        }

        IEnumerator<MarkupPart> IEnumerable<MarkupPart>.GetEnumerator()
        {
            return _markupParts.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _markupParts.GetEnumerator();
        }
    }

    public abstract class MarkupPart
    {
        public abstract void Render(StringBuilder stringBuilder, MarkupRendererOptions options);
    }

    public class MarkupText : MarkupPart
    {
        public string Text { get; }

        public MarkupText(string text)
        {
            Text = text;
        }

        public override void Render(StringBuilder stringBuilder, MarkupRendererOptions options)
        {
            stringBuilder.Append($"<span>{Text}</span>");
        }
    }

    public class MarkupLink : MarkupPart
    {
        public string Text { get; }
        public string Url { get; }

        public MarkupLink(string text, string url)
        {
            Text = text;
            Url = url;
        }

        public override void Render(StringBuilder stringBuilder, MarkupRendererOptions options)
        {
            var utmSeparator = Url.Contains("?") ? "&" : "?";

            var utm = options.IncludeUtm
                ? $"{utmSeparator}utm_source={options.UtmMedium}&utm_medium={options.UtmSource}" 
                : string.Empty;

            stringBuilder.Append($"<a href='{options.UrlDomainPrefix}{Url}{utm}' target='_blank'>{Text}</a>");
        }
    }
}