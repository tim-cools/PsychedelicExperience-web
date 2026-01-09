using System;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Psychedelics.Messages.Documents;
using PsychedelicExperience.Psychedelics.Messages.Documents.Commands;
using Xunit.Abstractions;

namespace PsychedelicExperience.Psychedelics.Tests.Integration
{
    public static class DocumentTestDataExtensions
    {
        public static DocumentId AddDocument(this TestContext<IMediator> context, UserId userId, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var id = DocumentId.New();
            var command = new AddDocument(userId, id, DocumentType.Page);
            context.ShouldSucceed(command, testOutputHelper);

            return id;
        }

        public static DocumentId AddDocument(this TestContext<IMediator> context, UserId userId, string name, ITestOutputHelper testOutputHelper = null)
        {
            var id = context.AddDocument(userId, testOutputHelper);
            context.SetDocumentName(id, userId, name);
            return id;
        }

        public static DocumentBuilder BuildDocument(this TestContext<IMediator> context, UserId userId, string name, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var builder = new DocumentBuilder(context, userId, testOutputHelper);
            return builder.BuildDocument(name);
        }

        public static DocumentId AddDocumentTag(this TestContext<IMediator> context, DocumentId documentId, UserId userId, string tag, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var id = DocumentId.New();
            var command = new AddDocumentTag(documentId, userId, tag);
            context.ShouldSucceed(command, testOutputHelper);

            return id;
        }

        public static DocumentId SetDocumentName(this TestContext<IMediator> context, DocumentId documentId, UserId userId, string name, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var id = DocumentId.New();

            var command = new ChangeDocumentName(userId, documentId, new Name(name));

            context.ShouldSucceed(command, testOutputHelper);

            return id;
        }

        public static DocumentId SetDocumentImage(this TestContext<IMediator> context, DocumentId documentId, UserId userId, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var id = DocumentId.New();

            var image = new Image(ImageId.New(), userId, "file", "org file");
            var command = new ChangeDocumentImage(documentId, userId, image);

            context.ShouldSucceed(command, testOutputHelper);

            return id;
        }

        public static DocumentId PublishDocument(this TestContext<IMediator> context, DocumentId documentId, UserId userId, ITestOutputHelper testOutputHelper = null)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var id = DocumentId.New();
            var command = new PublishDocument(documentId, userId);
            context.ShouldSucceed(command, testOutputHelper);

            return id;
        }
    }
}
