using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Experiences;
using PsychedelicExperience.Psychedelics.Messages.Experiences.Queries;
using PsychedelicExperience.Web.Infrastructure;
using PsychedelicExperience.Web.Infrastructure.Resources;

namespace PsychedelicExperience.Web.Controllers
{
    public class ExperienceViewsController : ViewController
    {
        public ExperienceViewsController(IMediator messageDispatcher, IConfiguration configuration)
            : base(messageDispatcher, configuration)
        {
        }

        [HttpGet("~/experience/")]
        public async Task<IActionResult> Get()
        {
            var query = new GetExperiences(null);

            return await ViewWithState(query, (builder, result) => builder.WithExperiences(result));
        }

        [HttpGet("~/experience/page/{page}")]
        public async Task<IActionResult> Get(int page)
        {
            var query = new GetExperiences(null, page: page);

            return await ViewWithState(query, (builder, result) => builder.WithExperiences(result));
        }

        [HttpGet("~/experience/substance/{substance}")]
        public async Task<IActionResult> GetBySubstance(string substance)
        {
            var query = new GetExperiences(null, substances: new[] { substance });

            return await ViewWithState(query, (builder, result) => builder.WithExperiences(result));
        }

        [HttpGet("~/experience/substance/{substance}/page/{page}")]
        public async Task<IActionResult> GetBySubstance(string substance, int page)
        {
            var query = new GetExperiences(null, substances: new[] { substance }, page: page);

            return await ViewWithState(query, (builder, result) => builder.WithExperiences(result));
        }

        [HttpGet("~/experience/tag/{tag}")]
        public async Task<IActionResult> GetByTag(string tag)
        {
            var query = new GetExperiences(null, tags: new[] { tag });

            return await ViewWithState(query, (builder, result) => builder.WithExperiences(result));
        }

        [HttpGet("~/experience/tag/{tag}/page/{page}")]
        public async Task<IActionResult> GetByTag(string tag, int page)
        {
            var query = new GetExperiences(null, tags: new[] { tag }, page: page);

            return await ViewWithState(query, (builder, result) => builder.WithExperiences(result));
        }

        [HttpGet("~/experience/new")]
        public IActionResult New()
        {
            return ViewWithState();
        }

        [HttpGet("~/experience/{id}")]
        public async Task<IActionResult> GetById(ExperienceId id)
        {
            var query = new GetExperience(null, id);

            return await ViewWithState(query,
                (builder, result) => builder.WithExperience(result),
                result => result.RedirectWhen(experience => experience != null, experience => experience.Url, Request.QueryString));
        }

        [HttpGet("~/experience/{id}/{slug}")]
        public async Task<IActionResult> GetById(ExperienceId id, string slug)
        {
            var query = new GetExperience(null, id);

            return await ViewWithState(query,
                (builder, result) => builder.WithExperience(result),
                result => result.RedirectWhen(experience => experience != null && experience.Slug != slug, experience => experience.Url, Request.QueryString));
        }

        [HttpGet("~/experience/{id}/social-image")]
        [ProducesResponseType(typeof(WebRequestMethods.File), 200)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        public async Task<FileStreamResult> Get(ExperienceId id)
        {
            var query = new GetExperience( null, id);
            var result = await MessageDispatcher.Send(query);
            if (result == null)
            {
                return DefaultSocialImage();
            }

            var templateExperienceSocialImage = result.Partner == "typm"
                ? WebResources.TemplateExperienceTypmSocialImage
                : WebResources.TemplateExperienceSocialImage;
                 
            var stream = templateExperienceSocialImage.AddTemplateText("“" + result.Title + "”");
            return new FileStreamResult(stream, "image/png");
        }

        private FileStreamResult DefaultSocialImage() => Image(WebResources.DefaultExperienceSocialImage);

        private static FileStreamResult Image(byte[] data)
        {
            var stream = new MemoryStream(data);
            return new FileStreamResult(stream, "image/png");
        }
    }
}
