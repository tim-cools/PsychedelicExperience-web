using System.Collections.Generic;
using PsychedelicExperience.Common.Mail;

namespace PsychedelicExperience.Psychedelics.OrganisationInvites
{
    internal class InviteOrganisationTemplate : Template
    {
        private readonly string _name;
        private readonly string _inviteUrl;
        private readonly string _registrationUrl;
        private readonly string _organisationUrl;

        public override string TemplateName => "20170406-invite-organisation";
        public override string FromAddressPrefix => null;

        public InviteOrganisationTemplate(string name, string registrationUrl, string inviteUrl, string organisationUrl)
        {
            _name = name;
            _inviteUrl = inviteUrl;
            _registrationUrl = registrationUrl;
            _organisationUrl = organisationUrl;
        }

        public override string GetSubject()
        {
            return _name + " is added to our global directory";
        }

        public override Dictionary<string, string> GetMergeFields()
        {
            return new Dictionary<string, string>
            {
                { "name", _name },
                { "inviteUrl", _inviteUrl },
                { "registrationUrl", _registrationUrl },
                { "organisationUrl", _organisationUrl}   
            };
        }
    }
}