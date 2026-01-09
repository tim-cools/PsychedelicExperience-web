using System;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Events;
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.Experiences
{
    public class Dose : AggregateRoot
    {
        public Guid ExperienceId { get; private set; }
        public Guid UserId { get; private set; }

        public int? TimeOffsetSeconds { get; private set; }

        public string Substance { get; private set; }
        public string Form { get; private set; }

        public decimal? Amount { get; private set; }
        public string Unit { get; private set; }
        public string Method { get; private set; }

        public string Notes { get; private set; }

        public void Add(DoseId doseId, Experience experience, User user)
        {
            EnsureCanAdd(experience, user);

            Publish(new DoseAdded
            {
                UserId = (UserId) user.Id,
                ExperienceId = (ExperienceId) experience.Id,
                DoseId = doseId
            });
        }

        public void Apply(DoseAdded @event)
        {
            Id = (Guid) @event.DoseId;
            UserId = (Guid) @event.UserId;
            ExperienceId = (Guid) @event.ExperienceId;
        }

        public void Remove(User user)
        {
            EnsureCanEdit(user);

            Publish(new DoseRemoved
            {
                DoseId = (DoseId) Id,
                ExperienceId = (ExperienceId) ExperienceId,
                UserId = (UserId) user.Id,
            });
        }

        public void Apply(DoseRemoved @event)
        {
        }

        public void UpdateAmount(User user, decimal? amount)
        {
            EnsureCanEdit(user);

            Publish(new DoseAmountUpdated
            {
                DoseId = (DoseId) Id,
                ExperienceId = (ExperienceId) ExperienceId,
                UserId = (UserId)user.Id,
                Amount = amount,
            });
        }

        public void Apply(DoseAmountUpdated @event)
        {
            Amount = @event.Amount;
        }

        public void UpdateMethod(User user, string method)
        {
            EnsureCanEdit(user);

            Publish(new DoseMethodUpdated
            {
                UserId = (UserId) user.Id,
                DoseId = (DoseId) Id,
                ExperienceId = (ExperienceId) ExperienceId,
                Method = method,
            });
        }

        public void Apply(DoseMethodUpdated @event)
        {
            Method = @event.Method;
        }

        public void UpdateUnit(User user, string unit)
        {
            EnsureCanEdit(user);

            Publish(new DoseUnitUpdated
            {
                UserId = (UserId)user.Id,
                DoseId = (DoseId) Id,
                ExperienceId = (ExperienceId) ExperienceId,
                Unit = unit
            });
        }

        public void Apply(DoseUnitUpdated @event)
        {
            Unit = @event.Unit;
        }

        public void UpdateSubstance(User user, string substance)
        {
            EnsureCanEdit(user);

            Publish(new DoseSubstanceUpdated
            {
                UserId = (UserId)user.Id,
                DoseId = (DoseId) Id,
                ExperienceId = (ExperienceId) ExperienceId,
                PreviousSubstance = Substance,
                Substance = substance,
            });
        }

        public void Apply(DoseSubstanceUpdated @event)
        {
            Substance = @event.Substance;
        }

        public void UpdateForm(User user, string form)
        {
            EnsureCanEdit(user);

            Publish(new DoseFormUpdated
            {
                UserId = (UserId) user.Id,
                DoseId = (DoseId) Id,
                ExperienceId = (ExperienceId) ExperienceId,
                Form = form
            });
        }

        public void Apply(DoseFormUpdated @event)
        {
            Form = @event.Form;
        }

        public void UpdateNotes(User user, string notes)
        {
            EnsureCanEdit(user);

            Publish(new DoseNotesUpdated()
            {
                UserId = (UserId)user.Id,
                DoseId = (DoseId) Id,
                ExperienceId = (ExperienceId) ExperienceId,
                Notes = notes
            });
        }

        public void Apply(DoseNotesUpdated @event)
        {
            Notes = @event.Notes;
        }

        public void UpdateTimeOffset(User user, int? timeOffsetSeconds)
        {
            EnsureCanEdit(user);

            Publish(new DoseTimeOffsetUpdated
            {
                UserId = (UserId) user.Id,
                DoseId = (DoseId) Id,
                ExperienceId = (ExperienceId) ExperienceId,
                TimeOffsetSeconds = timeOffsetSeconds,
            });
        }

        public void Apply(DoseTimeOffsetUpdated @event)
        {
            TimeOffsetSeconds = @event.TimeOffsetSeconds;
        }

        public bool IsOwner(User user)
        {
            return user != null && user.Id == UserId;
        }

        private void EnsureCanAdd(Experience experience, User user)
        {
            var isAdmin = user.IsAdministrator();
            var isOwner = experience.IsOwner(user);

            if (!isAdmin && !isOwner)
            {
                throw new BusinessException($"{user.Id} could not add dose to {experience.Id}!");
            }
        }

        private void EnsureCanEdit(User user)
        {
            if (!user.IsAdministrator() && !IsOwner(user))
            {
                throw new BusinessException($"{user.Id} could not edit dose {Id}!");
            }
        }
    }
}