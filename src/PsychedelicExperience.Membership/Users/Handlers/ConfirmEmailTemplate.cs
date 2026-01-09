using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MimeKit;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Mail;

namespace PsychedelicExperience.Membership.Users.Handlers
{
    internal class ConfirmEmailTemplate : Template
    {
        private readonly string _userName;
        private readonly string _url;
        
        public override string TemplateName => "20170415-confirm-email";

        public override string FromAddressPrefix => null;

        public ConfirmEmailTemplate(string userName, string url)
        {
            _userName = userName;
            _url = url;
        }

        public override string GetSubject()
        {
            return _userName + ", please confirm your account request";
        }

        public override Dictionary<string, string> GetMergeFields()
        {
            return new Dictionary<string, string>
          {
              { "userName", _userName },
              { "url", _url }
          };
        }
    }

    public static class ConfirmEmailTemplateExtensions
    {
        public static async Task SendConfirmEmail(this IMailSender emailSender, IConfiguration configuration, Guid userId, string displayName, string email, string token)
        {
            var address = new MailboxAddress(displayName, email);

            var url = configuration.WebUrl($"/account/confirm?userId={WebUtility.UrlDecode(new ShortGuid(userId))}&token={WebUtility.UrlEncode(token)}");

            await emailSender.SendMail(address, new ConfirmEmailTemplate(displayName, url));
        }
    }
}