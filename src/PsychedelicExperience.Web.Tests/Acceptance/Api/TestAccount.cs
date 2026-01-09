using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Web.Tests.Acceptance.Api
{
    public class TestAccount
    {
        public string EMail { get; }
        public string DisplayName { get; }
        public string AccessToken { get; }
        public ShortGuid Id { get; }

        public TestAccount(string email, string displayName, string accessToken, Guid? id = null)
        {
            EMail = email;
            DisplayName = displayName;
            AccessToken = accessToken;
            Id = id;
        }
    }
}