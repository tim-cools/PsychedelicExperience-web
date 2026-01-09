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
using User = PsychedelicExperience.Membership.Users.Domain.User;

namespace PsychedelicExperience.Psychedelics.Experiences
{
    public class Experience : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public DateTime? DateTime { get; private set; }

        public Title Title { get; private set; }
        public List<Name> Partners { get; } = new List<Name>();

        public ExperieneSet Set { get; } = new ExperieneSet();
        public ExperieneSetting Setting { get; } = new ExperieneSetting();
        public PublicExperience Public { get; } = new PublicExperience();
        public PrivateExperience Private { get; } = new PrivateExperience();
        public ExperienceLevel? Level { get; private set; }
        public ExperiencePrivacy Privacy { get; } = new ExperiencePrivacy();

        public IList<Tag> Tags { get; } = new List<Tag>();

        public void Add(ExperienceId experienceId, User user, DateTime? date, Title title, Description description, Name partner)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            Publish(new ExperienceAdded
            {
                UserId = new UserId(user.Id),
                DateTime = date,
                ExperienceId = experienceId,
                Title = title,
                Description = description,
                Partner = partner
            });
        }

        public void Apply(ExperienceAdded @event)
        {
            Id = (Guid) @event.ExperienceId;
            UserId = @event.UserId.Value;
            Title = @event.Title;
            Public.Description = @event.Description;
            if (@event.Partner != null)
            {
                Partners.Add(@event.Partner);
            }
            DateTime = @event.DateTime;
        }

        public void Remove(User user)
        {
            EnsureCanEdit(user);

            Publish(new ExperienceRemoved
            {
                ExperienceId = (ExperienceId) Id,
                UserId = (UserId) user.Id
            });
        }

        public void Apply(ExperienceRemoved @event)
        {
        }

        public void Report(User user, string reason)
        {
            Publish(new ExperienceReported
            {
                ExperienceId = (ExperienceId) Id,
                UserId = (UserId)user.Id,
                Reason = reason
            });
        }

        public void Apply(ExperienceReported @event)
        {
        }

        public void UpdateTitle(User user, Title title)
        {
            EnsureCanEdit(user);

            Publish(new ExperienceTitleChanged
            {
                UserId = (UserId) user.Id,
                ExperienceId = (ExperienceId) Id,
                Title = title,
            });
        }

        public void Apply(ExperienceTitleChanged @event)
        {
            Title = @event.Title;
        }

        public void UpdateDateTime(User user, DateTime? dateTime)
        {
            EnsureCanEdit(user);

            Publish(new ExperienceDateTimeChanged
            {
                UserId = (UserId) user.Id,
                ExperienceId = (ExperienceId) Id,
                DateTime = dateTime,
            });
        }

        public void Apply(ExperienceDateTimeChanged @event)
        {
            DateTime = @event.DateTime;
        }

        public void AddTag(User user, Name tag)
        {
            EnsureCanEdit(user);

            if (Tags.Any(criteria => Equals(criteria.Name, tag))) return;

            Publish(new ExperienceTagAdded
            {
                UserId = (UserId) user.Id,
                ExperienceId = (ExperienceId) Id,
                TagName = tag,
            });
        }

        public void Apply(ExperienceTagAdded @event)
        {
            var tag = new Tag(@event.TagName);
            Tags.Add(tag);
        }

        public void RemoveTag(User user, Name tag)
        {
            EnsureCanEdit(user);

            if (Tags.All(criteria => !Equals(criteria.Name, tag))) return;

            Publish(new ExperienceTagRemoved
            {
                ExperienceId = (ExperienceId) Id,
                UserId = (UserId) user.Id,
                TagName = tag,
            });
        }

        public void Apply(ExperienceTagRemoved @event)
        {
            var tag = Tags.Single(where => Equals(where.Name, @event.TagName));

            Tags.Remove(tag);
        }

        public void SetExperienceLevel(User user, ExperienceLevel level)
        {
            EnsureCanEdit(user);

            Publish(new ExperienceLevelChanged
            {
                ExperienceId = (ExperienceId) Id,
                UserId = (UserId) user.Id,
                PreviousLevel = Level.HasValue ? (Messages.Experiences.ExperienceLevel)Level : (Messages.Experiences.ExperienceLevel?)null,
                NewLevel = (Messages.Experiences.ExperienceLevel)level,
            });
        }

        public void Apply(ExperienceLevelChanged @event)
        {
            Level = @event.NewLevel.HasValue ? (ExperienceLevel)@event.NewLevel : (ExperienceLevel?)null;
        }

        public void SetPrivacyLevel(User user, PrivacyLevel level)
        {
            EnsureCanEdit(user);

            Publish(new ExperiencePrivacyLevelChanged
            {
                ExperienceId = (ExperienceId) Id,
                UserId = (UserId) user.Id,
                PreviousLevel = (Messages.Experiences.PrivacyLevel)Privacy.Level,
                NewLevel = (Messages.Experiences.PrivacyLevel)level,
            });
        }

        public void Apply(ExperiencePrivacyLevelChanged @event)
        {
            Privacy.Level = (PrivacyLevel)@event.NewLevel;
        }

        public void SetSetting(User user, Description description)
        {
            EnsureCanEdit(user);

            if (description == null)
            {
                throw new BusinessException("Setting description cannot be null");
            }

            Publish(new ExperienceSettingChanged
            {
                ExperienceId = (ExperienceId) Id,
                UserId = (UserId) user.Id,
                SettingDescription = description,
            });
        }

        public void Apply(ExperienceSettingChanged @event)
        {
            Setting.Description = @event.SettingDescription;
        }

        public void SetSet(User user, Description description)
        {
            EnsureCanEdit(user);

            if (description == null)
            {
                throw new BusinessException("Set description cannot be null");
            }

            Publish(new ExperienceSetChanged
            {
                ExperienceId = (ExperienceId) Id,
                UserId = (UserId) user.Id,
                SetDescription = description,
            });
        }

        public void Apply(ExperienceSetChanged @event)
        {
            Set.Description = @event.SetDescription;
        }

        public void SetPublicDescription(User user, Description description)
        {
            EnsureCanEdit(user);

            Publish(new ExperiencePublicDescriptionChanged
            {
                ExperienceId = (ExperienceId) Id,
                UserId = (UserId) user.Id,
                Description = description,
            });
        }

        public void Apply(ExperiencePublicDescriptionChanged @event)
        {
            Public.Description = @event.Description;
        }

        public void SetPrivateNotes(User user, EncryptedString description)
        {
            EnsureCanEdit(user);

            Publish(new ExperiencePrivateNotesChanged
            {
                ExperienceId = (ExperienceId) Id,
                UserId = (UserId) user.Id,
                Description = description,
            });
        }

        public void Apply(ExperiencePrivateNotesChanged @event)
        {
            Private.Description = @event.Description;
        }

        public bool IsOwner(User user)
        {
            return user != null && user.Id == UserId;
        }

        public bool CanView(User user)
        {
            return Privacy.Level != PrivacyLevel.Private
                || IsOwner(user);
        }

        public bool IsRestricted(User user)
        {
            return Privacy.Level == PrivacyLevel.Restricted
                && !IsOwner(user);
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