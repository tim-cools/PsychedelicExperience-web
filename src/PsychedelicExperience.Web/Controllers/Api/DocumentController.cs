using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Membership.Messages;
using PsychedelicExperience.Psychedelics.Messages.Documents;
using PsychedelicExperience.Psychedelics.Messages.Documents.Commands;
using PsychedelicExperience.Psychedelics.Messages.Documents.Queries;
using PsychedelicExperience.Psychedelics.Messages.Tags.Queries;
using PsychedelicExperience.Web.Infrastructure;
using PsychedelicExperience.Web.Infrastructure.Security;
using PsychedelicExperience.Web.ViewModels.Api;

namespace PsychedelicExperience.Web.Controllers.Api
{
    [UnauthenticateWhenSessionExpired]
    public class DocumentController : Controller
    {
        private readonly IMediator _messageDispatcher;
        private readonly IConfiguration _configuration;

        public DocumentController(IMediator messageDispatcher, IConfiguration configuration)
        {
            _messageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet("~/api/document/")]
        [ProducesResponseType(typeof(DocumentsResult), 200)]
        public async Task<IActionResult> GetList(string query, string[] tags, int page = 0)
        {
            var userId = User.GetUserId();

            var documentsQuery = new GetDocuments(userId, tags, query, page);
            var documents = await _messageDispatcher.Send(documentsQuery);

            return Json(documents);
        }

        [HttpGet("~/api/document/{id}/")]
        [ProducesResponseType(typeof(DocumentDetails), 200)]
        public async Task<IActionResult> Get(DocumentId id)
        {
            var userId = User.GetUserId();

            var query = new GetDocument(userId, id);
            var experience = await _messageDispatcher.Send(query);

            return experience != null ? Json(experience) : id.AcceptedWhenRecent();
        }

        [HttpGet("~/api/document/slug/{slug}")]
        [ProducesResponseType(typeof(DocumentDetails), 200)]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var userId = User.GetUserId();

            var query = new GetDocument(userId, slug);
            var experience = await _messageDispatcher.Send(query);

            return experience != null ? Json(experience) : (IActionResult)NotFound();
        }

        [HttpGet("~/api/document/slug/blog/{slug}")]
        [ProducesResponseType(typeof(DocumentDetails), 200)]
        public async Task<IActionResult> GetBlogBySlug(string slug) => await GetBySlug($"blog/{slug}");

        [HttpGet("~/api/document/slug/support/{slug}")]
        [ProducesResponseType(typeof(DocumentDetails), 200)]
        public async Task<IActionResult> GetSupportBySlug(string slug) => await GetBySlug($"support/{slug}");

        [Authorize]
        [HttpPost("~/api/document/")]
        public async Task<IActionResult> Post()
        {
            var id = DocumentId.New();
            var userId = User.GetAuthorizedUserId();

            var command = new AddDocument(userId, id, DocumentType.Page);

            var result = await _messageDispatcher.Send(command);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return CreatedAtAction("Get", new { id = new ShortGuid(id.Value) }, null);
        }

        [Authorize]
        [HttpDelete("~/api/document/{id}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Remove(DocumentId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new RemoveDocument(userId, id);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/document/{id}/update-name")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateTitle(DocumentId id, Name name)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeDocumentName(userId, id, name);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/document/{id}/update-slug")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateSlug(DocumentId id, Name slug)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeDocumentSlug(userId, id, slug);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/document/{id}/update-description")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateDescription(DocumentId id, Description description)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeDocumentDescription(userId, id, description);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/document/{id}/update-content")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> UpdateContent(DocumentId id, Description content)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ChangeDocumentContent(userId, id, content);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/document/{id}/publish")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Publish(DocumentId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new PublishDocument(id, userId);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPut("~/api/document/{id}/unpublish")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> Unpublish(DocumentId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new UnpublishDocument(id, userId);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpPost("~/api/document/{id}/tags/{name}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> AddTag(DocumentId id, string name)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new AddDocumentTag(id, userId, name);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [Authorize]
        [HttpDelete("~/api/document/{id}/tags/{name}")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> RemoveTag(DocumentId id, string name)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new RemoveDocumentTag(id, userId, name);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        /// <summary>
        /// Add photos to an document.
        /// </summary>
        [HttpPost("~/api/document/{id}/image/")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> AddPhoto(DocumentId id, ICollection<IFormFile> files)
        {
            var userId = User.GetUserId();

            if (files.Count != 1)
            {
                return BadRequest("Invalid number of files. Should be 1.");
            }

            var formFiles = await files.Save(_configuration);

            var image = formFiles
                .Select(file => new Image(new ImageId(file.Id), userId, file.FileName, file.OriginalFileName))
                .First();

            var query = new ChangeDocumentImage(id, userId, image);
            var result = await _messageDispatcher.Send(query);

            return this.ResultData(result);
        }

        [Authorize]
        [HttpDelete("~/api/document/{id}/image")]
        [ProducesResponseType(typeof(EmptyResponse), 200)]
        [ProducesResponseType(typeof(Result), 400)]
        public async Task<IActionResult> RemovePhoto(DocumentId id)
        {
            var userId = User.GetAuthorizedUserId();

            var query = new ClearDocumentImage(id, userId);
            var result = await _messageDispatcher.Send(query);

            return this.Result(result);
        }

        [HttpGet("~/api/document/tags/")]
        [ProducesResponseType(typeof(Tag[]), 200)]
        public async Task<IActionResult> Tags(string query)
        {
            var userId = User.GetUserId();

            var request = new TagsQuery(userId, query, TagsDomain.Documents);
            var tags = await _messageDispatcher.Send(request);

            return Json(tags);
        }
    }
}
