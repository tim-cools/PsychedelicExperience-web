using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MimeKit;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Mail;
using PsychedelicExperience.Psychedelics.OrganisationView;

namespace PsychedelicExperience.Psychedelics.OrganisationInvites
{
    public static class EmailTemplateExtensions
    {
        public static async Task SendInviteOrganisationEmail(this IMailSender emailSender, IConfiguration configuration, Organisation organisation, Contact contact, string inviteToken)
        {
            var address = new MailboxAddress(organisation.Name, contact.Value);

            var registrationUrl = configuration.WebUrl($"/register?t={WebUtility.UrlEncode(inviteToken)}");
            var inviteUrl = configuration.WebUrl($"/user/invite?t={WebUtility.UrlEncode(inviteToken)}");

            var organisationUrl = configuration.WebUrl(organisation.GetUrl());

            var template = new InviteOrganisationTemplate(
                organisation.Name,
                registrationUrl,
                inviteUrl, 
                organisationUrl);

            await emailSender.SendMail(address, template);
        }
    }
}