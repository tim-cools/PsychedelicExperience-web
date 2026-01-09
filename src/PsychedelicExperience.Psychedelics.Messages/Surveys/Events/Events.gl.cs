using System;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Experiences;

namespace  PsychedelicExperience.Psychedelics.Messages.Surveys.Events
{
	public class SurveyAdded : Event
	{
		public SurveyId SurveyId { get; set; }
		public UserId UserId { get; set; }
		public ExperienceId ExperienceId { get; set; }
		public DateTime Started { get; set; }
		public string SurveyName { get; set; }
		public SurveyData Data { get; set; }
	}

}
