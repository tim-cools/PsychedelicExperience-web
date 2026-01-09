using System.Linq;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Aggregates;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Common.Tests;
using PsychedelicExperience.Common.Tests.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Membership.Messages.Users;
using PsychedelicExperience.Membership.Tests.Integration;
using PsychedelicExperience.Membership.Users.Domain;
using PsychedelicExperience.Psychedelics.Documents;
using PsychedelicExperience.Psychedelics.Messages.Documents;
using PsychedelicExperience.Psychedelics.Messages.Documents.Commands;
using PsychedelicExperience.Psychedelics.Messages.Documents.Events;
using Shouldly;
using Xunit;
using DocumentType = PsychedelicExperience.Psychedelics.Messages.Documents.DocumentType;

namespace PsychedelicExperience.Psychedelics.Tests.Integration.Documents
{
    public class WhenAddDocument : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly DocumentId _documentId = DocumentId.New();
        private Result _result;

        public WhenAddDocument(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId, Roles.ContentManager);

            var query = new AddDocument(_userId, _documentId, DocumentType.Page);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheAggregateShouldBeUpdated()
        {
            SessionScope(context =>
            {
                var aggregate = context.Session.Load<Document>(_documentId);

                aggregate.ShouldNotBeNull();
                aggregate.UserId.ShouldBe(_userId);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_documentId).ToArray();
                events.Length.ShouldBe(1);

                var @event = events.LastEventShouldBeOfType<DocumentAdded>();

                @event.UserId.ShouldBe(_userId);
                @event.DocumentId.ShouldBe(_documentId);
            });
        }
    }

    public class WhenAddDocumentTag : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private DocumentId _documentId;
        private Result _result;

        public WhenAddDocumentTag(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId, Roles.ContentManager);
            _documentId = context.AddDocument(_userId);

            var query = new AddDocumentTag(_documentId, _userId, "Tag1");
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheAggregateShouldBeUpdated()
        {
            SessionScope(context =>
            {
                var aggregate = context.Session.Load<Document>(_documentId);

                aggregate.ShouldNotBeNull();
                aggregate.UserId.ShouldBe(_userId);
                aggregate.Tags.ShouldContain(tag => tag.Name == "Tag1");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_documentId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<DocumentTagAdded>();

                @event.UserId.ShouldBe(_userId);
                @event.DocumentId.ShouldBe(_documentId);
                @event.TagName.ShouldBe("Tag1");
            });
        }
    }

    public class WhenChangeDocumentContent : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private DocumentId _documentId;
        private Result _result;

        public WhenChangeDocumentContent(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId, Roles.ContentManager);
            _documentId = context.AddDocument(_userId);

            var query = new ChangeDocumentContent(_userId, _documentId, new Description("new content"));
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheAggregateShouldBeUpdated()
        {
            SessionScope(context =>
            {
                var aggregate = context.Session.Load<Document>(_documentId);

                aggregate.ShouldNotBeNull();
                aggregate.UserId.ShouldBe(_userId);
                aggregate.Content.Value.ShouldBe("new content");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_documentId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<DocumentContentChanged>();

                @event.UserId.ShouldBe(_userId);
                @event.DocumentId.ShouldBe(_documentId);
                @event.Content.Value.ShouldBe("new content");
            });
        }
    }

    public class WhenChangeDocumentDescription : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private DocumentId _documentId;
        private Result _result;

        public WhenChangeDocumentDescription(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId, Roles.ContentManager);
            _documentId = context.AddDocument(_userId);

            var query = new ChangeDocumentDescription(_userId, _documentId, new Description("new description for at least 50 characters long with suffix"));
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheAggregateShouldBeUpdated()
        {
            SessionScope(context =>
            {
                var aggregate = context.Session.Load<Document>(_documentId);

                aggregate.ShouldNotBeNull();
                aggregate.UserId.ShouldBe(_userId);
                aggregate.Description?.Value.ShouldBe("new description for at least 50 characters long with suffix");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_documentId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<DocumentDescriptionChanged>();

                @event.UserId.ShouldBe(_userId);
                @event.DocumentId.ShouldBe(_documentId);
                @event.Description.Value.ShouldBe("new description for at least 50 characters long with suffix");
            });
        }
    }

    public class WhenChangeDocumentImage : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private readonly ImageId _imageId = ImageId.New();
        private DocumentId _documentId;
        private Result _result;

        public WhenChangeDocumentImage(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId, Roles.ContentManager);
            _documentId = context.AddDocument(_userId);

            var query = new ChangeDocumentImage(_documentId, _userId, new Image(_imageId, _userId, "new file", "org file"));
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheAggregateShouldBeUpdated()
        {
            SessionScope(context =>
            {
                var aggregate = context.Session.Load<Document>(_documentId);

                aggregate.ShouldNotBeNull();
                aggregate.UserId.ShouldBe(_userId);
                aggregate.Image.ShouldNotBeNull();
                aggregate.Image.Id.ShouldBe(_imageId);
                aggregate.Image.UserId.ShouldBe(_userId);
                aggregate.Image.FileName.ShouldBe("new file");
                aggregate.Image.OriginalFileName.ShouldBe("org file");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_documentId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<DocumentImageChanged>();

                @event.UserId.ShouldBe(_userId);
                @event.DocumentId.ShouldBe(_documentId);
                @event.Image.ShouldNotBeNull();
                @event.Image.Id.ShouldBe(_imageId);
                @event.Image.UserId.ShouldBe(_userId);
                @event.Image.FileName.ShouldBe("new file");
                @event.Image.OriginalFileName.ShouldBe("org file");
            });
        }
    }

    public class WhenChangeDocumentName : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private DocumentId _documentId;
        private Result _result;

        public WhenChangeDocumentName(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId, Roles.ContentManager);
            _documentId = context.AddDocument(_userId);

            var query = new ChangeDocumentName(_userId, _documentId, new Name("new name"));
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheAggregateShouldBeUpdated()
        {
            SessionScope(context =>
            {
                var aggregate = context.Session.Load<Document>(_documentId);

                aggregate.ShouldNotBeNull();
                aggregate.UserId.ShouldBe(_userId);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_documentId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<DocumentNameChanged>();

                @event.UserId.ShouldBe(_userId);
                @event.DocumentId.ShouldBe(_documentId);
                @event.Name.Value.ShouldBe("new name");
            });
        }
    }

    public class WhenChangeDocumentSlug : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        
        private DocumentId _documentId;
        private Result _result;

        public WhenChangeDocumentSlug(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId, Roles.ContentManager);
            _documentId = context.AddDocument(_userId);

            var query = new ChangeDocumentSlug(_userId, _documentId, new Name("new slug"));
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheAggregateShouldBeUpdated()
        {
            SessionScope(context =>
            {
                var aggregate = context.Session.Load<Document>(_documentId);

                aggregate.ShouldNotBeNull();
                aggregate.UserId.ShouldBe(_userId);
                aggregate.Slug.Value.ShouldBe("new slug");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_documentId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<DocumentSlugChanged>();

                @event.UserId.ShouldBe(_userId);
                @event.DocumentId.ShouldBe(_documentId);
                @event.Slug.Value.ShouldBe("new slug");
            });
        }
    }

    public class WhenClearDocumentImage : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        
        private DocumentId _documentId;
        private Result _result;

        public WhenClearDocumentImage(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId, Roles.ContentManager);
            _documentId = context.AddDocument(_userId);
            context.SetDocumentImage(_documentId, _userId);

            var query = new ClearDocumentImage(_documentId, _userId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheAggregateShouldBeUpdated()
        {
            SessionScope(context =>
            {
                var aggregate = context.Session.Load<Document>(_documentId);

                aggregate.ShouldNotBeNull();
                aggregate.UserId.ShouldBe(_userId);
                aggregate.Image.ShouldBeNull();
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_documentId).ToArray();
                events.Length.ShouldBe(3);

                var @event = events.LastEventShouldBeOfType<DocumentImageCleared>();

                @event.UserId.ShouldBe(_userId);
                @event.DocumentId.ShouldBe(_documentId);
            });
        }
    }

    public class WhenPublishDocument : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        private DocumentId _documentId;
        private Result _result;

        public WhenPublishDocument(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId, Roles.ContentManager);
            _documentId = context.AddDocument(_userId);

            var query = new PublishDocument(_documentId, _userId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheAggregateShouldBeUpdated()
        {
            SessionScope(context =>
            {
                var aggregate = context.Session.Load<Document>(_documentId);

                aggregate.ShouldNotBeNull();
                aggregate.UserId.ShouldBe(_userId);
                aggregate.DocumentStatus.ShouldBe(DocumentStatus.Published);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_documentId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<DocumentPublished>();
                @event.UserId.ShouldBe(_userId);
                @event.DocumentId.ShouldBe(_documentId);
            });
        }
    }

    public class WhenRemoveDocument : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        
        private DocumentId _documentId;
        private Result _result;

        public WhenRemoveDocument(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId, Roles.ContentManager);
            _documentId = context.AddDocument(_userId);

            var query = new RemoveDocument(_userId, _documentId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheAggregateShouldBeUpdated()
        {
            SessionScope(context =>
            {
                var aggregate = context.Session.Load<Document>(_documentId);

                aggregate.ShouldNotBeNull();
                aggregate.UserId.ShouldBe(_userId);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_documentId).ToArray();
                events.Length.ShouldBe(2);

                var @event = events.LastEventShouldBeOfType<DocumentRemoved>();

                @event.UserId.ShouldBe(_userId);
                @event.DocumentId.ShouldBe(_documentId);
            });
        }
    }

    public class WhenRemoveDocumentTag : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        
        private DocumentId _documentId;
        private Result _result;

        public WhenRemoveDocumentTag(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId, Roles.ContentManager);
            _documentId = context.AddDocument(_userId);
            context.AddDocumentTag(_documentId, _userId, "tag1");

            var query = new RemoveDocumentTag(_documentId, _userId, "tag1");
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheAggregateShouldBeUpdated()
        {
            SessionScope(context =>
            {
                var aggregate = context.Session.Load<Document>(_documentId);

                aggregate.ShouldNotBeNull();
                aggregate.UserId.ShouldBe(_userId);
                aggregate.Tags.ShouldNotContain(tag => tag.Name == "tag1");
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_documentId).ToArray();
                events.Length.ShouldBe(3);

                var @event = events.LastEventShouldBeOfType<DocumentTagRemoved>();

                @event.UserId.ShouldBe(_userId);
                @event.DocumentId.ShouldBe(_documentId);
                @event.TagName.ShouldBe("tag1");
            });
        }
    }

    public class WhenUnpublishDocument : ServiceTestBase<IMediator>, IClassFixture<PsychedelicsIntegrationTestFixture>
    {
        private readonly UserId _userId = UserId.New();
        
        private DocumentId _documentId;
        private Result _result;

        public WhenUnpublishDocument(PsychedelicsIntegrationTestFixture fixture)
            : base(fixture)
        {
        }

        protected override void When(TestContext<IMediator> context)
        {
            context.AddUser(_userId, Roles.ContentManager);
            _documentId = context.AddDocument(_userId);
            context.PublishDocument(_documentId, _userId);

            var query = new UnpublishDocument(_documentId, _userId);
            _result = context.Service.ExecuteNowWithTimeout(query);
        }

        [Fact]
        public void ThenTheCommandShouldSucceed()
        {
            _result.ShouldBeSuccess();
        }

        [Fact]
        public void ThenTheAggregateShouldBeUpdated()
        {
            SessionScope(context =>
            {

                var aggregate = context.Session.Load<Document>(_documentId);

                aggregate.ShouldNotBeNull();
                aggregate.UserId.ShouldBe(_userId);
            });
        }

        [Fact]
        public void ThenTheEventShouldBeStored()
        {
            SessionScope(context =>
            {
                var events = context.Session.LoadEvents(_documentId).ToArray();
                events.Length.ShouldBe(3);

                var @event = events.LastEventShouldBeOfType<DocumentUnpublished>();

                @event.UserId.ShouldBe(_userId);
                @event.DocumentId.ShouldBe(_documentId);
            });
        }
    }
}