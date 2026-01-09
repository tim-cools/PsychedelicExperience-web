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
    internal class ResetPasswordTemplate : Template
    {
        private readonly string _userName;
        private readonly string _url;

        public override string TemplateName => "20180306-reset-password";
        public override string FromAddressPrefix => null;

        public ResetPasswordTemplate(string userName, string url)
        {
            _userName = userName;
            _url = url;
        }

        public override string GetSubject() => _userName + ", reset your password";

        public override Dictionary<string, string> GetMergeFields()
        {
            return new Dictionary<string, string>
            {
                { "userName", _userName },
                { "url", _url }
            };
        }
    }

    public static class ResetPasswordTemplateExtensions
    {
        public static async Task SendResetPassword(this IMailSender emailSender, IConfiguration configuration, Guid userId, string displayName, string email, string token)
        {
            var address = new MailboxAddress(displayName, email);

            var url = configuration.WebUrl($"/user/reset-password?userId={WebUtility.UrlDecode(new ShortGuid(userId))}&token={WebUtility.UrlEncode(token)}");

            await emailSender.SendMail(address, new ResetPasswordTemplate(displayName, url));
        }
    }
}