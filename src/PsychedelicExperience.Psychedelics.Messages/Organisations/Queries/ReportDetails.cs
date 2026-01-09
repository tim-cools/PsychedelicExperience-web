using System;
using PsychedelicExperience.Common;

namespace PsychedelicExperience.Psychedelics.Messages.Organisations.Queries
{
    public class ReportDetails
    {
        public string Reason { get; set; }
        public ShortGuid UserId { get; set; }
        public string UserName { get; set; }
        public DateTime DateTime { get; set; }
    }
}