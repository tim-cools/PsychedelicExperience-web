using System;
using System.Collections.Generic;
using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Security;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Events;
using PsychedelicExperience.Psychedelics.Messages.Surveys;
using PsychedelicExperience.Psychedelics.Messages.Surveys.Events;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.Experiences
{
    public class Survey : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public Guid ExperienceId { get; private set; }

        public DateTime? DateTimeStarted { get; private set; }
        public DateTime? DateTimeStored { get; private set; }

        public string Name { get; private set; }

        public SurveyData Data { get; private set; }

        public void Add(SurveyId surveyId, ExperienceId experienceId, User user, string surveyName, SurveyData data)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            Publish(new SurveyAdded
            {
                SurveyId = surveyId,
                UserId = new UserId(user.Id),
                ExperienceId = experienceId,
                SurveyName = surveyName,
                Data = data
            });
        }

        public void Apply(SurveyAdded @event)
        {
            Id = (Guid) @event.SurveyId;
            ExperienceId = (Guid) @event.ExperienceId;
            UserId = @event.UserId.Value;
            Name = @event.SurveyName;
            Data = @event.Data;
            DateTimeStarted = @event.Started;
            DateTimeStored = @event.EventTimestamp;
        }

        private bool IsOwner(User user)
        {
            return user != null && user.Id == UserId;
        }

        public bool CanView(User user)
        {
            return user.IsAtLeast(Roles.ContentManager) || IsOwner(user);
        }

        private void EnsureCanEdit(User user)
        {
            if (!user.IsAtLeast(Roles.ContentManager) && !IsOwner(user))
            {
                throw new BusinessException($"{user.Id} could not edit experience {Id}!");
            }
        }
    }
}