using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Documents;
using PsychedelicExperience.Psychedelics.Messages.Documents.Queries;
using PsychedelicExperience.Common;
using PsychedelicExperience.Web.Infrastructure;
using PsychedelicExperience.Web.Infrastructure.Resources;

namespace PsychedelicExperience.Web.Controllers
{
    public class DocumentViewsController : ViewController
    {
        public DocumentViewsController(IMediator messageDispatcher, IConfiguration configuration)
            : base(messageDispatcher, configuration)
        {
        }

        [HttpGet("~/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var query = new GetDocument(null, slug);

            return await ViewWithState(query, (builder, result) => builder.WithDocument(result));
        }

        [HttpGet("~/blog/{slug}")]
        public async Task<IActionResult> GetBlogBySlug(string slug) => await GetBySlug($"blog/{slug}");

        [HttpGet("~/support/{slug}")]
        public async Task<IActionResult> GetSupportBySlug(string slug) => await GetBySlug($"blog/{slug}");


        [HttpGet("~/document/")]
        public async Task<IActionResult> Get() => await GetDocuments();

        [HttpGet("~/document/page/{page}")]
        public async Task<IActionResult> Get(int page) => await GetDocuments(page);

        [HttpGet("~/document/tag/{tag}")]
        public async Task<IActionResult> GetByTag(string tag) => await GetDocuments(0, tag);

        [HttpGet("~/document/tag/{tag}/page/{page}")]
        public async Task<IActionResult> GetByTag(string tag, int page) => await GetDocuments(page, tag);

        [HttpGet("~/blog/")]
        public async Task<IActionResult> GetBlog() => await GetDocuments(0, "blog");

        [HttpGet("~/blog/page/{page}")]
        public async Task<IActionResult> GetBlog(int page) => await GetDocuments(page, "blog");

        [HttpGet("~/blog/tag/{tag}")]
        public async Task<IActionResult> GetBlogByTag(string tag) => await GetDocuments(0, "blog", tag);

        [HttpGet("~/blog/tag/{tag}/page/{page}")]
        public async Task<IActionResult> GetBlogByTag(string tag, int page) => await GetDocuments(page, "blog", tag);

        [HttpGet("~/support/")]
        public async Task<IActionResult> GetSupport() => await GetDocuments(0, "support");

        [HttpGet("~/support/page/{page}")]
        public async Task<IActionResult> GetSupport(int page) => await GetDocuments(page, "support");

        [HttpGet("~/support/tag/{tag}")]
        public async Task<IActionResult> GetSupportByTag(string tag) => await GetDocuments(0, "support", tag);

        [HttpGet("~/support/tag/{tag}/page/{page}")]
        public async Task<IActionResult> GetSupportByTag(string tag, int page) => await GetDocuments(page, "support", tag);

        private async Task<IActionResult> GetDocuments(int page = 0, params string[] tags)
        {
            var query = new GetDocuments(null, tags, page: page);

            return await ViewWithState(query, (builder, result) => builder.WithDocuments(result));
        }

        [HttpGet("~/document/new")]
        public IActionResult New()
        {
            return ViewWithState();
        }

        [HttpGet("~/document/{id}")]
        public async Task<IActionResult> GetById(DocumentId id)
        {
            var query = new GetDocument(null, id);

            return await ViewWithState(query,
                (builder, result) => builder.WithDocument(result),
                result => result.NotFoundWhenNull());
        }

        [HttpGet("~/document/{id}/edit")]
        public async Task<IActionResult> Edit(DocumentId id)
        {
            var query = new GetDocument(null, id);

            return await ViewWithState(query, (builder, result) => builder.WithDocument(result));
        }

        /// <summary>
        /// Get a Image from a document. Contenttype should be jpg/png.
        /// </summary>
        [HttpGet("~/document/{id}/image")]
        [ProducesResponseType(typeof(File), 200)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        public async Task<IActionResult> Image(DocumentId id, int height = 400, int width = 750)
        {
            var query = new GetDocumentImage(id);
            var result = await MessageDispatcher.Send(query);
            if (result == null)
            {
                return DefaultImage();
            }

            var folder = Configuration.PhotosFolder();
            var fileName = Path.GetFileName(result.FileName);
            var fullPath = Path.Combine(folder, fileName);

            var resized = ImageSizes.GetResized(folder, fullPath, height, width);
            return FileImage(resized ?? fullPath);
        }

        private static IActionResult FileImage(string sourceFile)
        {
            var stream = System.IO.File.OpenRead(sourceFile);

            return new FileStreamResult(stream, "image/" + Path.GetExtension(sourceFile).TrimStart('.'));
        }

        private FileStreamResult DefaultImage()
        {
            return Image(WebResources.DefaultDocumentImage);
        }

        private static FileStreamResult Image(byte[] data)
        {
            var stream = new MemoryStream(data);
            return new FileStreamResult(stream, "image/png");
        }
    }
}
