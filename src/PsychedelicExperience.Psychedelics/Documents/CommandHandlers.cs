using FluentValidation;
using Marten;
using PsychedelicExperience.Psychedelics.Messages.Documents;
using PsychedelicExperience.Psychedelics.Messages.Documents.Commands;

namespace PsychedelicExperience.Psychedelics.Documents
{
    public class AddDocumentValidator : AbstractValidator<AddDocument>
    {
        public AddDocumentValidator()
        {
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.DocumentId).DocumentId();
            RuleFor(command => command.DocumentType).DocumentType();
        }
    }

    public class AddDocumentHandler : AggregateCommandHandler<AddDocument, Document, DocumentId>
    {
        public AddDocumentHandler(IDocumentSession session, IValidator<AddDocument> commandValidator) :
            base(session, commandValidator,
                command => command.DocumentId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class RemoveDocumentValidator : AbstractValidator<RemoveDocument>
    {
        public RemoveDocumentValidator()
        {
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.DocumentId).DocumentId();
        }
    }

    public class RemoveDocumentHandler : AggregateCommandHandler<RemoveDocument, Document, DocumentId>
    {
        public RemoveDocumentHandler(IDocumentSession session, IValidator<RemoveDocument> commandValidator) :
            base(session, commandValidator,
                command => command.DocumentId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeDocumentNameValidator : AbstractValidator<ChangeDocumentName>
    {
        public ChangeDocumentNameValidator()
        {
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.DocumentId).DocumentId();
            RuleFor(command => command.Name).Name();
        }
    }

    public class ChangeDocumentNameHandler : AggregateCommandHandler<ChangeDocumentName, Document, DocumentId>
    {
        public ChangeDocumentNameHandler(IDocumentSession session, IValidator<ChangeDocumentName> commandValidator) :
            base(session, commandValidator,
                command => command.DocumentId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeDocumentDescriptionValidator : AbstractValidator<ChangeDocumentDescription>
    {
        public ChangeDocumentDescriptionValidator()
        {
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.DocumentId).DocumentId();
            RuleFor(command => command.Description).Description();
        }
    }

    public class ChangeDocumentDescriptionHandler :
        AggregateCommandHandler<ChangeDocumentDescription, Document, DocumentId>
    {
        public ChangeDocumentDescriptionHandler(IDocumentSession session,
            IValidator<ChangeDocumentDescription> commandValidator) :
            base(session, commandValidator,
                command => command.DocumentId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeDocumentSlugValidator : AbstractValidator<ChangeDocumentSlug>
    {
        public ChangeDocumentSlugValidator()
        {
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.DocumentId).DocumentId();
            RuleFor(command => command.Slug).Slug();
        }
    }

    public class ChangeDocumentSlugHandler : AggregateCommandHandler<ChangeDocumentSlug, Document, DocumentId>
    {
        public ChangeDocumentSlugHandler(IDocumentSession session, IValidator<ChangeDocumentSlug> commandValidator) :
            base(session, commandValidator,
                command => command.DocumentId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeDocumentImageValidator : AbstractValidator<ChangeDocumentImage>
    {
        public ChangeDocumentImageValidator()
        {
            RuleFor(command => command.DocumentId).DocumentId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.Image).Image();
        }
    }

    public class ChangeDocumentImageHandler : AggregateCommandHandler<ChangeDocumentImage, Document, DocumentId>
    {
        public ChangeDocumentImageHandler(IDocumentSession session, IValidator<ChangeDocumentImage> commandValidator) :
            base(session, commandValidator,
                command => command.DocumentId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ClearDocumentImageValidator : AbstractValidator<ClearDocumentImage>
    {
        public ClearDocumentImageValidator()
        {
            RuleFor(command => command.DocumentId).DocumentId();
            RuleFor(command => command.UserId).UserId();
        }
    }

    public class ClearDocumentImageHandler : AggregateCommandHandler<ClearDocumentImage, Document, DocumentId>
    {
        public ClearDocumentImageHandler(IDocumentSession session, IValidator<ClearDocumentImage> commandValidator) :
            base(session, commandValidator,
                command => command.DocumentId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class ChangeDocumentContentValidator : AbstractValidator<ChangeDocumentContent>
    {
        public ChangeDocumentContentValidator()
        {
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.DocumentId).DocumentId();
            RuleFor(command => command.Content).Content();
        }
    }

    public class ChangeDocumentContentHandler : AggregateCommandHandler<ChangeDocumentContent, Document, DocumentId>
    {
        public ChangeDocumentContentHandler(IDocumentSession session, IValidator<ChangeDocumentContent> commandValidator)
            :
            base(session, commandValidator,
                command => command.DocumentId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class AddDocumentTagValidator : AbstractValidator<AddDocumentTag>
    {
        public AddDocumentTagValidator()
        {
            RuleFor(command => command.DocumentId).DocumentId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.TagName).TagName();
        }
    }

    public class AddDocumentTagHandler : AggregateCommandHandler<AddDocumentTag, Document, DocumentId>
    {
        public AddDocumentTagHandler(IDocumentSession session, IValidator<AddDocumentTag> commandValidator) :
            base(session, commandValidator,
                command => command.DocumentId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class RemoveDocumentTagValidator : AbstractValidator<RemoveDocumentTag>
    {
        public RemoveDocumentTagValidator()
        {
            RuleFor(command => command.DocumentId).DocumentId();
            RuleFor(command => command.UserId).UserId();
            RuleFor(command => command.TagName).TagName();
        }
    }

    public class RemoveDocumentTagHandler : AggregateCommandHandler<RemoveDocumentTag, Document, DocumentId>
    {
        public RemoveDocumentTagHandler(IDocumentSession session, IValidator<RemoveDocumentTag> commandValidator) :
            base(session, commandValidator,
                command => command.DocumentId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class PublishDocumentValidator : AbstractValidator<PublishDocument>
    {
        public PublishDocumentValidator()
        {
            RuleFor(command => command.DocumentId).DocumentId();
            RuleFor(command => command.UserId).UserId();
        }
    }

    public class PublishDocumentHandler : AggregateCommandHandler<PublishDocument, Document, DocumentId>
    {
        public PublishDocumentHandler(IDocumentSession session, IValidator<PublishDocument> commandValidator) :
            base(session, commandValidator,
                command => command.DocumentId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

    public class UnpublishDocumentValidator : AbstractValidator<UnpublishDocument>
    {
        public UnpublishDocumentValidator()
        {
            RuleFor(command => command.DocumentId).DocumentId();
            RuleFor(command => command.UserId).UserId();
        }
    }

    public class UnpublishDocumentHandler : AggregateCommandHandler<UnpublishDocument, Document, DocumentId>
    {
        public UnpublishDocumentHandler(IDocumentSession session, IValidator<UnpublishDocument> commandValidator) :
            base(session, commandValidator,
                command => command.DocumentId,
                command => command.UserId,
                (aggregate, user) => aggregate.EnsureCanEdit(user))
        {
        }
    }

}