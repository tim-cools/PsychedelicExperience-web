using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Marten;
using Microsoft.AspNetCore.Identity;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Commands;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using PsychedelicExperience.Psychedelics.Messages.Tags.Queries;
using PsychedelicExperience.Psychedelics.OrganisationView.Handlers;
using PsychedelicExperience.Psychedelics.Tags.Handlers;
using User = PsychedelicExperience.Membership.Users.Domain.User;    

namespace PsychedelicExperience.Psychedelics.Organisations
{
    public class AddOrganisationValidator : AbstractValidator<AddOrganisation>
    {
        public AddOrganisationValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Name).Name();
            RuleFor(command => command.Description).Description();
            RuleFor(command => command.Tags).Tags();
            RuleFor(command => command.Address).Address();
        }
    }

    public class AddOrganisationHandler : AggregateCommandHandler<AddOrganisation, Organisation, OrganisationId>
    {
        public AddOrganisationHandler(IDocumentSession session, IValidator<AddOrganisation> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanAdd(user))
        {
        }
    }

    public class AddOrganisationPhotoValidator : AbstractValidator<AddOrganisationPhotos>
    {
        public AddOrganisationPhotoValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.Photos).PhotoFiles();
        }
    }

    public class AddOrganisationPhotosHandler : AggregateCommandHandler<AddOrganisationPhotos, Organisation, OrganisationId, Context<AddOrganisationPhotos, Organisation>, AddOrganisationPhotosResult>
    {
        public AddOrganisationPhotosHandler(IDocumentSession session, IValidator<AddOrganisationPhotos> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                context => {})
        {
        }

        protected override AddOrganisationPhotosResult ResultSuccess(Context<AddOrganisationPhotos, Organisation> context)
        {
            var photos = context.Command.Photos
                .Select(photo => photo.Id.Value)
                .ToArray();

            return new AddOrganisationPhotosResult(photos);
        }

        protected override AddOrganisationPhotosResult ResultFailed(Context<AddOrganisationPhotos, Organisation> context, BusinessException businessException)
        {
            return new AddOrganisationPhotosResult(false, businessException.Errors.ToArray());
        }
    }

    public class AddOrganisationReviewValidator : AbstractValidator<AddOrganisationReview>
    {
        public AddOrganisationReviewValidator()
        {
            RuleFor(command => command.OrganisationReviewId).OrganisationReviewId();
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Name).Name();
            RuleFor(command => command.Visited).Visited();
            RuleFor(command => command.Description).Description();
            RuleFor(command => command.Rating).Rating();
            RuleFor(command => command.Center).CenterReview();
            RuleFor(command => command.Community).CommunityReview();
            RuleFor(command => command.HealthcareProvider).HealthcareProviderReview();
        }
    }

    public class AddOrganisationReviewHandler : AggregateCommandHandler<AddOrganisationReview, OrganisationReview, OrganisationReviewId>
    {
        public AddOrganisationReviewHandler(IDocumentSession session, IValidator<AddOrganisationReview> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationReviewId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanAdd(user))
        {
        }

        protected override async Task StoreAggregate(Context<AddOrganisationReview, OrganisationReview> context)
        {
            await base.StoreAggregate(context);

            await CreateExperience(context, context.Command.Experience);
        }

        private async Task CreateExperience(Context<AddOrganisationReview, OrganisationReview> context, Experience commandExperience)
        {
            if (commandExperience == null) return;

            var experienceId = commandExperience.ExperienceId;
            var experience = await Session.LoadAggregate<Experiences.Experience>(experienceId);

            experience.Add(experienceId, context.User, null, commandExperience.Title, commandExperience.Description, null);

            Session.StoreChanges(experience);
        }
    }

    public class AddOrganisationOwnerHandler : AggregateCommandHandler<AddOrganisationOwner, Organisation, OrganisationId, AddOrganisationOwnerHandler.Context, AddOrganisationOwnerResult>
    {
        private readonly ILookupNormalizer _lookupNormalizer;

        public class Context : Context<AddOrganisationOwner, Organisation>
        {
            public User Owner { get; set; }
        }

        public AddOrganisationOwnerHandler(IDocumentSession session, IValidator<AddOrganisationOwner> commandValidator, ILookupNormalizer lookupNormalizer) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                context => context.Aggregate.EnsureCanAddOwner(context.User, context.Owner))
        {
            _lookupNormalizer = lookupNormalizer;
        }

        protected override AddOrganisationOwnerResult ResultSuccess(Context context)
        {
            return new AddOrganisationOwnerResult(context.Owner.Id);
        }

        protected override AddOrganisationOwnerResult ResultFailed(Context context, BusinessException businessException)
        {
            return new AddOrganisationOwnerResult(false, businessException.Errors.ToArray());
        }

        protected override void HandleCommand(Context context)
        {
            context.Aggregate.Handle(context.User, context.Command, context.Owner);
        }

        protected override async Task LoadData(Context context)
        {
            await base.LoadData(context);

            if (context.Command.UserEmail != null)
            {
                context.Owner = await Session.LoadUserAsync(_lookupNormalizer, context.Command.UserEmail);
            }
            else
            {
                context.Owner = await Session.LoadUserAsync(context.Command.OwnerId);
            }
        }
    }

    public class AddOrganisationOwnerValidator : AbstractValidator<AddOrganisationOwner>
    {
        public AddOrganisationOwnerValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.OwnerId).NotEmpty().When(command => command.UserEmail == null);
            RuleFor(command => command.UserEmail).NotEmpty().When(command => command.OwnerId == null);
        }
    }

    public class RemoveOrganisationOwnerHandler : AggregateCommandHandler<RemoveOrganisationOwner, Organisation, OrganisationId>
    {
        public RemoveOrganisationOwnerHandler(IDocumentSession session, IValidator<RemoveOrganisationOwner> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanAdmin(user))
        {
        }
    }

    public class RemoveOrganisationOwnerValidator : AbstractValidator<RemoveOrganisationOwner>
    {
        public RemoveOrganisationOwnerValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.OwnerId).UserId();
        }
    }

    public class AddOrganisationTagValidator : AbstractValidator<AddOrganisationTag>
    {
        public AddOrganisationTagValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.TagName).TagName();
        }
    }

    public class AddOrganisationTagHandler : AggregateCommandHandler<AddOrganisationTag, Organisation, OrganisationId>
    {
        public AddOrganisationTagHandler(IDocumentSession session, IValidator<AddOrganisationTag> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }


        protected override void OnValidateCommand(Context<AddOrganisationTag, Organisation> context)
        {
            var editor = context.User.IsAtLeast(Roles.ContentManager);
            if (!TagRepository.Exists(TagsDomain.Organisations, context.Command.TagName, editor))
            {
                throw new BusinessException(new ValidationError("TagName", "TagNotAllowed",
                    $"Can't add tag: {context.Command.TagName}"));
            }
        }
    }

    public class AddOrganisationTypeValidator : AbstractValidator<AddOrganisationType>
    {
        public AddOrganisationTypeValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Type).Type();
        }
    }

    public class AddOrganisationTypeHandler : AggregateCommandHandler<AddOrganisationType, Organisation, OrganisationId>
    {
        public AddOrganisationTypeHandler(IDocumentSession session, IValidator<AddOrganisationType> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class LinkOrganisationValidator : AbstractValidator<LinkOrganisation>
    {
        public LinkOrganisationValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Relation).Relation();
        }
    }

    public class LinkOrganisationTypeHandler : AggregateCommandHandler<LinkOrganisation, Organisation, OrganisationId>
    {
        public LinkOrganisationTypeHandler(IDocumentSession session, IValidator<LinkOrganisation> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeOrganisationAddressValidator : AbstractValidator<ChangeOrganisationAddress>
    {
        public ChangeOrganisationAddressValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Address).Address();
        }
    }

    public class ChangeOrganisationAddressHandler : AggregateCommandHandler<ChangeOrganisationAddress, Organisation, OrganisationId>
    {
        public ChangeOrganisationAddressHandler(IDocumentSession session, IValidator<ChangeOrganisationAddress> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeOrganisationDescriptionValidator : AbstractValidator<ChangeOrganisationDescription>
    {
        public ChangeOrganisationDescriptionValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Description).Description();
        }
    }

    public class ChangeOrganisationDescriptionHandler : AggregateCommandHandler<ChangeOrganisationDescription, Organisation, OrganisationId>
    {
        public ChangeOrganisationDescriptionHandler(IDocumentSession session, IValidator<ChangeOrganisationDescription> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class SetOrganisationWarningHandler : AggregateCommandHandler<SetOrganisationWarning, Organisation, OrganisationId>
    {
        public SetOrganisationWarningHandler(IDocumentSession session, IValidator<SetOrganisationWarning> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanAdmin(user))
        {
        }
    }

    public class SetOrganisationWarningValidator : AbstractValidator<SetOrganisationWarning>
    {
        public SetOrganisationWarningValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Title).InfoTitle();
            RuleFor(command => command.Content).InfoContent();
        }
    }

    public class RemoveOrganisationWarningHandler : AggregateCommandHandler<RemoveOrganisationWarning, Organisation, OrganisationId>
    {
        public RemoveOrganisationWarningHandler(IDocumentSession session, IValidator<RemoveOrganisationWarning> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanAdmin(user))
        {
        }
    }

    public class RemoveOrganisationWarningValidator : AbstractValidator<RemoveOrganisationWarning>
    {
        public RemoveOrganisationWarningValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
        }
    }

    public class SetOrganisationInfoHandler : AggregateCommandHandler<SetOrganisationInfo, Organisation, OrganisationId>
    {
        public SetOrganisationInfoHandler(IDocumentSession session, IValidator<SetOrganisationInfo> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanAdmin(user))
        {
        }
    }

    public class SetOrganisationInfoValidator : AbstractValidator<SetOrganisationInfo>
    {
        public SetOrganisationInfoValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Title).InfoTitle();
            RuleFor(command => command.Content).InfoContent();
        }
    }

    public class RemoveOrganisationInfoHandler : AggregateCommandHandler<RemoveOrganisationInfo, Organisation, OrganisationId>
    {
        public RemoveOrganisationInfoHandler(IDocumentSession session, IValidator<RemoveOrganisationInfo> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanAdmin(user))
        {
        }
    }

    public class RemoveOrganisationInfoValidator : AbstractValidator<RemoveOrganisationInfo>
    {
        public RemoveOrganisationInfoValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
        }
    }

    public class AddOrganisatioContactValidator : AbstractValidator<AddOrganisationContact>
    {
        public AddOrganisatioContactValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Type).NotNull();
        }
    }

    public class AddOrganisationContactHandler : AggregateCommandHandler<AddOrganisationContact, Organisation, OrganisationId>
    {
        public AddOrganisationContactHandler(IDocumentSession session, IValidator<AddOrganisationContact> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ContactOrganisationValidator : AbstractValidator<ContactOrganisation>
    {
        public ContactOrganisationValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.Email).EmailAddress();
            RuleFor(command => command.Subject).NotEmpty();
            RuleFor(command => command.Message).NotEmpty();
        }
    }

    public class ContactOrganisationHandler : AggregateCommandHandler<ContactOrganisation, Organisation, OrganisationId, ContactOrganisationHandler.Context, Result>
    {
        public class Context : Context<ContactOrganisation, Organisation>
        {
            public Organisation Organisation { get; set; }
        }

        public ContactOrganisationHandler(IDocumentSession session, IValidator<ContactOrganisation> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                context => {})
        {
        }

        protected override Result ResultSuccess(Context context)
        {
            return Result.Success;
        }

        protected override Result ResultFailed(Context context, BusinessException businessException)
        {
            return Result.Failed(businessException.Errors.ToArray());
        }

        protected override void HandleCommand(Context context)
        {
            context.Aggregate.Handle(context.User, context.Command, context.Organisation);
        }

        protected override async Task LoadData(Context context)
        {
            await base.LoadData(context);

            context.Organisation = await Session.LoadAsync<Organisation>(context.Command.OrganisationId);
        }
    }

    public class RemoveOrganisatioContactValidator : AbstractValidator<RemoveOrganisationContact>
    {
        public RemoveOrganisatioContactValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Type).NotNull();
            RuleFor(command => command.Value).NotNull();
        }
    }

    public class RemoveOrganisationContactHandler : AggregateCommandHandler<RemoveOrganisationContact, Organisation, OrganisationId>
    {
        public RemoveOrganisationContactHandler(IDocumentSession session, IValidator<RemoveOrganisationContact> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeOrganisationNameValidator : AbstractValidator<ChangeOrganisationName>
    {
        public ChangeOrganisationNameValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Name).Name();
        }
    }

    public class ChangeOrganisationNameHandler : AggregateCommandHandler<ChangeOrganisationName, Organisation, OrganisationId>
    {
        public ChangeOrganisationNameHandler(IDocumentSession session, IValidator<ChangeOrganisationName> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }


    public class ChangeOrganisationCenterValidator : AbstractValidator<ChangeOrganisationCenter>
    {
        public ChangeOrganisationCenterValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationReviewId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Center).Center();
        }
    }

    public class ChangeOrganisationCenterHandler : AggregateCommandHandler<ChangeOrganisationCenter, Organisation, OrganisationId>
    {
        public ChangeOrganisationCenterHandler(IDocumentSession session, IValidator<ChangeOrganisationCenter> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeOrganisationPractitionerValidator : AbstractValidator<ChangeOrganisationPractitioner>
    {
        public ChangeOrganisationPractitionerValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationReviewId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Practitioner).Practitioner();
        }
    }

    public class ChangeOrganisationPractitionerHandler : AggregateCommandHandler<ChangeOrganisationPractitioner, Organisation, OrganisationId>
    {
        public ChangeOrganisationPractitionerHandler(IDocumentSession session, IValidator<ChangeOrganisationPractitioner> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeOrganisationPersonValidator : AbstractValidator<ChangeOrganisationPerson>
    {
        public ChangeOrganisationPersonValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationReviewId();
            RuleFor(command => command.UserId).UserId();
        }
    }

    public class ChangeOrganisationPersonHandler : AggregateCommandHandler<ChangeOrganisationPerson, Organisation, OrganisationId>
    {
        public ChangeOrganisationPersonHandler(IDocumentSession session, IValidator<ChangeOrganisationPerson> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanAdmin(user))
        {
        }
    }

    public class ChangeOrganisationCommunityValidator : AbstractValidator<ChangeOrganisationCommunity>
    {
        public ChangeOrganisationCommunityValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationReviewId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Community).Community();
        }
    }

    public class ChangeOrganisationCommunityHandler : AggregateCommandHandler<ChangeOrganisationCommunity, Organisation, OrganisationId>
    {
        public ChangeOrganisationCommunityHandler(IDocumentSession session, IValidator<ChangeOrganisationCommunity> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeOrganisationHealthcareProviderValidator : AbstractValidator<ChangeOrganisationHealthcareProvider>
    {
        public ChangeOrganisationHealthcareProviderValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationReviewId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.HealthcareProvider).HealthcareProvider();
        }
    }

    public class ChangeOrganisationHealthCarProviderHandler : AggregateCommandHandler<ChangeOrganisationHealthcareProvider, Organisation, OrganisationId>
    {
        public ChangeOrganisationHealthCarProviderHandler(IDocumentSession session, IValidator<ChangeOrganisationHealthcareProvider> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeOrganisationReviewDescriptionValidator : AbstractValidator<ChangeOrganisationReviewDescription>
    {
        public ChangeOrganisationReviewDescriptionValidator()
        {
            RuleFor(command => command.OrganisationReviewId).OrganisationReviewId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Description).Description();
        }
    }

    public class ChangeOrganisationReviewDescriptionHandler : AggregateCommandHandler<ChangeOrganisationReviewDescription, OrganisationReview, OrganisationReviewId>
    {
        public ChangeOrganisationReviewDescriptionHandler(IDocumentSession session, IValidator<ChangeOrganisationReviewDescription> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationReviewId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeOrganisationReviewNameValidator : AbstractValidator<ChangeOrganisationReviewName>
    {
        public ChangeOrganisationReviewNameValidator()
        {
            RuleFor(command => command.OrganisationReviewId).OrganisationReviewId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Name).Name();
        }
    }

    public class ChangeOrganisationReviewNameHandler : AggregateCommandHandler<ChangeOrganisationReviewName, OrganisationReview, OrganisationReviewId>
    {
        public ChangeOrganisationReviewNameHandler(IDocumentSession session, IValidator<ChangeOrganisationReviewName> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationReviewId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class RateOrganisationReviewRatedValidator : AbstractValidator<RateOrganisationReview>
    {
        public RateOrganisationReviewRatedValidator()
        {
            RuleFor(command => command.OrganisationReviewId).OrganisationReviewId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Rating).Rating();
        }
    }

    public class RateOrganisationReviewRatedHandler : AggregateCommandHandler<RateOrganisationReview, OrganisationReview, OrganisationReviewId>
    {
        public RateOrganisationReviewRatedHandler(IDocumentSession session, IValidator<RateOrganisationReview> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationReviewId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeOrganisationReviewCenterValidator : AbstractValidator<ChangeOrganisationReviewCenter>
    {
        public ChangeOrganisationReviewCenterValidator()
        {
            RuleFor(command => command.OrganisationReviewId).OrganisationReviewId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Review).CenterReview();
        }
    }

    public class ChangeOrganisationReviewCenterHandler : AggregateCommandHandler<ChangeOrganisationReviewCenter, OrganisationReview, OrganisationReviewId>
    {
        public ChangeOrganisationReviewCenterHandler(IDocumentSession session, IValidator<ChangeOrganisationReviewCenter> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationReviewId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeOrganisationReviewCommunityValidator : AbstractValidator<ChangeOrganisationReviewCommunity>
    {
        public ChangeOrganisationReviewCommunityValidator()
        {
            RuleFor(command => command.OrganisationReviewId).OrganisationReviewId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Review).CommunityReview();
        }
    }

    public class ChangeOrganisationReviewCommunityHandler : AggregateCommandHandler<ChangeOrganisationReviewCommunity, OrganisationReview, OrganisationReviewId>
    {
        public ChangeOrganisationReviewCommunityHandler(IDocumentSession session, IValidator<ChangeOrganisationReviewCommunity> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationReviewId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeOrganisationReviewHealthcareProviderValidator : AbstractValidator<ChangeOrganisationReviewHealthcareProvider>
    {
        public ChangeOrganisationReviewHealthcareProviderValidator()
        {
            RuleFor(command => command.OrganisationReviewId).OrganisationReviewId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Review).HealthcareProviderReview();
        }
    }

    public class ChangeOrganisationReviewHealthCarProviderHandler : AggregateCommandHandler<ChangeOrganisationReviewHealthcareProvider, OrganisationReview, OrganisationReviewId>
    {
        public ChangeOrganisationReviewHealthCarProviderHandler(IDocumentSession session, IValidator<ChangeOrganisationReviewHealthcareProvider> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationReviewId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class RemoveOrganisationValidator : AbstractValidator<RemoveOrganisation>
    {
        public RemoveOrganisationValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
        }
    }

    public class RemoveOrganisationHandler : AggregateCommandHandler<RemoveOrganisation, Organisation, OrganisationId>
    {
        public RemoveOrganisationHandler(IDocumentSession session, IValidator<RemoveOrganisation> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanRemove(user))
        {
        }

        protected override async Task StoreAggregate(Context<RemoveOrganisation, Organisation> context)
        {
            await base.StoreAggregate(context);

            Session.Delete(context.Aggregate);
        }
    }

    public class RemoveOrganisationPhotoValidator : AbstractValidator<RemoveOrganisationPhoto>
    {
        public RemoveOrganisationPhotoValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.PhotoId).PhotoId();
            RuleFor(command => command.UserId).UserId();
        }
    }

    public class RemoveOrganisationPhotoHandler : AggregateCommandHandler<RemoveOrganisationPhoto, Organisation, OrganisationId>
    {
        public RemoveOrganisationPhotoHandler(IDocumentSession session, IValidator<RemoveOrganisationPhoto> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class RemoveOrganisationReviewValidator : AbstractValidator<RemoveOrganisationReview>
    {
        public RemoveOrganisationReviewValidator()
        {
            RuleFor(command => command.OrganisationReviewId).OrganisationReviewId();
            RuleFor(command => command.UserId).UserId();
        }
    }

    public class RemoveOrganisationReviewHandler : AggregateCommandHandler<RemoveOrganisationReview, OrganisationReview, OrganisationReviewId>
    {
        public RemoveOrganisationReviewHandler(IDocumentSession session, IValidator<RemoveOrganisationReview> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationReviewId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanRemove(user))
        {
        }

        protected override async Task StoreAggregate(Context<RemoveOrganisationReview, OrganisationReview> context)
        {
            await base.StoreAggregate(context);

            Session.Delete(context.Aggregate);
        }
    }

    public class ReportOrganisationReviewValidator : AbstractValidator<ReportOrganisationReview>
    {
        public ReportOrganisationReviewValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.OrganisationReviewId).OrganisationReviewId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Reason).Reason();
        }
    }

    public class ReportOrganisationReviewHandler : AggregateCommandHandler<ReportOrganisationReview, OrganisationReview, OrganisationReviewId>
    {
        public ReportOrganisationReviewHandler(IDocumentSession session, IValidator<ReportOrganisationReview> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationReviewId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanReport(user))
        {
        }
    }

    public class RemoveOrganisationTagValidator : AbstractValidator<RemoveOrganisationTag>
    {
        public RemoveOrganisationTagValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.TagName).TagName();
        }
    }

    public class RemoveOrganisationTagHandler : AggregateCommandHandler<RemoveOrganisationTag, Organisation, OrganisationId>
    {
        public RemoveOrganisationTagHandler(IDocumentSession session, IValidator<RemoveOrganisationTag> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class RemoveOrganisationTypeValidator : AbstractValidator<RemoveOrganisationType>
    {
        public RemoveOrganisationTypeValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Type).Type();
        }
    }

    public class RemoveOrganisationTypeHandler : AggregateCommandHandler<RemoveOrganisationType, Organisation, OrganisationId>
    {
        public RemoveOrganisationTypeHandler(IDocumentSession session, IValidator<RemoveOrganisationType> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class UnlinkOrganisationValidator : AbstractValidator<UnlinkOrganisation>
    {
        public UnlinkOrganisationValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
        }
    }

    public class UnlinkOrganisationHandler : AggregateCommandHandler<UnlinkOrganisation, Organisation, OrganisationId>
    {
        public UnlinkOrganisationHandler(IDocumentSession session, IValidator<UnlinkOrganisation> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ReportOrganisationValidator : AbstractValidator<ReportOrganisation>
    {
        public ReportOrganisationValidator()
        {
            RuleFor(command => command.OrganisationId).OrganisationId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Reason).Reason();
        }
    }

    public class ReportOrganisationHandler : AggregateCommandHandler<ReportOrganisation, Organisation, OrganisationId>
    {
        public ReportOrganisationHandler(IDocumentSession session, IValidator<ReportOrganisation> commandValidator) :
            base(session, commandValidator,
                command => command.OrganisationId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanReport(user))
        {
        }
    }

    public class RemoveOrganisationsHandler : IAsyncRequestHandler<RemoveOrganisations, RemoveOrganisationsResult>
    {
        private readonly IDocumentSession _session;
        private readonly GetOrganisationsQueryBuilder _queryBuilder;

        public RemoveOrganisationsHandler(IDocumentSession session, GetOrganisationsQueryBuilder queryBuilder)
        {
            _session = session;
            _queryBuilder = queryBuilder;
        }

        public async Task<RemoveOrganisationsResult> Handle(RemoveOrganisations command)
        {
            var user = await _session.LoadUserAsync(command.UserId);

            if (!user.IsAdministrator()) throw new InvalidOperationException("User is not administrator!");

            var all = await _queryBuilder
                .WithPrivacy(user)
                .FilterTypes("Therapist")
                .Execute() as JsonOrganisationsResult;

            var organisations = await _queryBuilder
                .WithPrivacy(user)
                .FilterTypes("Therapist")
                .FilterByHasOwner(false)
                .Execute() as JsonOrganisationsResult;

            var organisationsOwner = await _queryBuilder
                .WithPrivacy(user)
                .FilterTypes("Therapist")
                .FilterByHasOwner(true)
                .Execute() as JsonOrganisationsResult;

            if (command.Confirm)
            {
                foreach (var organisation in organisations.Organisations)
                {
                    var aggregate =
                        _session.Load<PsychedelicExperience.Psychedelics.Organisations.Organisation>(organisation
                            .OrganisationId.Guid);
                    var removeOrganisation = new RemoveOrganisation(new OrganisationId(aggregate.Id), command.UserId);
                    aggregate.Handle(user, removeOrganisation);

                    _session.StoreChanges(aggregate);
                }

                await _session.SaveChangesAsync();
            }

            return new RemoveOrganisationsResult
            {
                Count = all.Total,
                CountWithOwner = organisationsOwner.Total,
                CountWithoutOwner = organisations.Total,
            };
        }
    }
}