using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PsychedelicExperience.Common.Messages;
using PsychedelicExperience.Psychedelics.Messages.Organisations;
using PsychedelicExperience.Psychedelics.Messages.Organisations.Queries;
using PsychedelicExperience.Common;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates;
using PsychedelicExperience.Psychedelics.Messages.OrganisationUpdates.Queries;
using PsychedelicExperience.Web.Infrastructure;

namespace PsychedelicExperience.Web.Controllers
{
    public class OrganisationViewsController : ViewController
    {
        public OrganisationViewsController(IMediator messageDispatcher, IConfiguration configuration) : base(messageDispatcher, configuration)
        {
        }

        [HttpGet("~/organisation/")]
        
        [HttpGet("~/retreat/")]
        [HttpGet("~/clinic/")]
        [HttpGet("~/research/")]
        [HttpGet("~/community/")]
        [HttpGet("~/shop/")]
        [HttpGet("~/education/")]
        [HttpGet("~/consumer/")]
        [HttpGet("~/medical/")]
        [HttpGet("~/consumer-products/")]
        [HttpGet("~/medical-products/")]
        [HttpGet("~/business-Services/")]
        [HttpGet("~/media/")]
        
        [HttpGet("~/shaman/")]
        [HttpGet("~/guide/")]
        [HttpGet("~/coach/")]
        [HttpGet("~/therapist/")]
        [HttpGet("~/integration/")]
        [HttpGet("~/trip-sitter/")]
        [HttpGet("~/facilitator/")]
        [HttpGet("~/advocate/")]
        [HttpGet("~/researcher/")]
        [HttpGet("~/mycologist/")]
        [HttpGet("~/anthropologist/")]
        public async Task<IActionResult> Get(bool onlyWithoutTags = false, string[] tags = null, string[] types = null, bool? hasOwner = null)
        {
            var query = new GetOrganisations(null, tags: tags, onlyWithoutTags: onlyWithoutTags, types: GetOrganisationType(types), hasOwner: hasOwner);

            return await ViewWithState(query, (builder, result) => builder.WithOrganisations(result));
        }

        [HttpGet("~/organisation/page/{page}")]
             
        [HttpGet("~/retreat/page/{page}")]
        [HttpGet("~/clinic/page/{page}")]
        [HttpGet("~/research/page/{page}")]
        [HttpGet("~/community/page/{page}")]
        [HttpGet("~/shop/page/{page}")]
        [HttpGet("~/education/page/{page}")]
        [HttpGet("~/consumer/page/{page}")]
        [HttpGet("~/medical/page/{page}")]
        [HttpGet("~/consumer-products/page/{page}")]
        [HttpGet("~/medical-products/page/{page}")]
        [HttpGet("~/business-Services/page/{page}")]
        [HttpGet("~/media/page/{page}")]
        
        [HttpGet("~/shaman/page/{page}")]
        [HttpGet("~/guide/page/{page}")]
        [HttpGet("~/coach/page/{page}")]
        [HttpGet("~/therapist/page/{page}")]
        [HttpGet("~/integration/page/{page}")]
        [HttpGet("~/trip-sitter/page/{page}")]
        [HttpGet("~/facilitator/page/{page}")]
        [HttpGet("~/advocate/page/{page}")]
        [HttpGet("~/researcher/page/{page}")]
        [HttpGet("~/mycologist/page/{page}")]
        [HttpGet("~/anthropologist/page/{page}")]
        public async Task<IActionResult> Get(int page, bool onlyWithoutTags = false, string[] tags = null, string[] types = null)
        {
            var query = new GetOrganisations(null, tags: tags, page: page, onlyWithoutTags: onlyWithoutTags, types: GetOrganisationType(types));

            return await ViewWithState(query, (builder, result) => builder.WithOrganisations(result));
        }

        [HttpGet("~/organisation/tag/{tag}")]
       
        [HttpGet("~/retreat/tag/{tag}")]
        [HttpGet("~/clinic/tag/{tag}")]
        [HttpGet("~/research/tag/{tag}")]
        [HttpGet("~/community/tag/{tag}")]
        [HttpGet("~/shop/tag/{tag}")]
        [HttpGet("~/education/tag/{tag}")]
        [HttpGet("~/consumer/tag/{tag}")]
        [HttpGet("~/medical/tag/{tag}")]
        [HttpGet("~/consumer-products/tag/{tag}")]
        [HttpGet("~/medical-products/tag/{tag}")]
        [HttpGet("~/business-Services/tag/{tag}")]
        [HttpGet("~/media/tag/{tag}")]
        
        [HttpGet("~/shaman/tag/{tag}")]
        [HttpGet("~/guide/tag/{tag}")]
        [HttpGet("~/coach/tag/{tag}")]
        [HttpGet("~/therapist/tag/{tag}")]
        [HttpGet("~/integration/tag/{tag}")]
        [HttpGet("~/trip-sitter/tag/{tag}")]
        [HttpGet("~/facilitator/tag/{tag}")]
        [HttpGet("~/advocate/tag/{tag}")]
        [HttpGet("~/researcher/tag/{tag}")]
        [HttpGet("~/mycologist/tag/{tag}")]
        [HttpGet("~/anthropologist/tag/{tag}")]
        public async Task<IActionResult> GetByTag(string tag, bool onlyWithoutTags = false, string[] tags = null, string[] types = null)
        {
            var tagsValues = MergeTags(tag, tags);
            var query = new GetOrganisations(null, tags: tagsValues, onlyWithoutTags: onlyWithoutTags, types: GetOrganisationType(types));

            return await ViewWithState(query, (builder, result) => builder.WithOrganisations(result));
        }


        [HttpGet("~/organisation/tag/{tag}/page/{page}")]
        
        [HttpGet("~/retreat/tag/{tag}/page/{page}")]
        [HttpGet("~/clinic/tag/{tag}/page/{page}")]
        [HttpGet("~/research/tag/{tag}/page/{page}")]
        [HttpGet("~/community/tag/{tag}/page/{page}")]
        [HttpGet("~/shop/tag/{tag}/page/{page}")]
        [HttpGet("~/education/tag/{tag}/page/{page}")]
        [HttpGet("~/consumer/tag/{tag}/page/{page}")]
        [HttpGet("~/medical/tag/{tag}/page/{page}")]
        [HttpGet("~/consumer-products/tag/{tag}/page/{page}")]
        [HttpGet("~/medical-products/tag/{tag}/page/{page}")]
        [HttpGet("~/business-Services/tag/{tag}/page/{page}")]
        [HttpGet("~/media/tag/{tag}/page/{page}")]
        
        [HttpGet("~/shaman/tag/{tag}/page/{page}")]
        [HttpGet("~/guide/tag/{tag}/page/{page}")]
        [HttpGet("~/coach/tag/{tag}/page/{page}")]
        [HttpGet("~/therapist/tag/{tag}/page/{page}")]
        [HttpGet("~/integration/tag/{tag}/page/{page}")]
        [HttpGet("~/trip-sitter/tag/{tag}/page/{page}")]
        [HttpGet("~/facilitator/tag/{tag}/page/{page}")]
        [HttpGet("~/advocate/tag/{tag}/page/{page}")]
        [HttpGet("~/researcher/tag/{tag}/page/{page}")]
        [HttpGet("~/mycologist/tag/{tag}/page/{page}")]
        [HttpGet("~/anthropologist/tag/{tag}/page/{page}")]
        public async Task<IActionResult> GetByTag(string tag, int page, bool onlyWithoutTags = false, string[] tags = null, string[] types = null)
        {
            var query = new GetOrganisations(null, tags: MergeTags(tag, tags), page: page, onlyWithoutTags: onlyWithoutTags, types: GetOrganisationType(types));

            return await ViewWithState(query, (builder, result) => builder.WithOrganisations(result));
        }

        [HttpGet("~/organisation/country/{country}")]
       
        [HttpGet("~/retreat/country/{country}")]
        [HttpGet("~/clinic/country/{country}")]
        [HttpGet("~/research/country/{country}")]
        [HttpGet("~/community/country/{country}")]
        [HttpGet("~/shop/country/{country}")]
        [HttpGet("~/education/country/{country}")]
        [HttpGet("~/consumer/country/{country}")]
        [HttpGet("~/medical/country/{country}")]
        [HttpGet("~/consumer-products/country/{country}")]
        [HttpGet("~/medical-products/country/{country}")]
        [HttpGet("~/business-Services/country/{country}")]
        [HttpGet("~/media/country/{country}")]
        
        [HttpGet("~/shaman/country/{country}")]
        [HttpGet("~/guide/country/{country}")]
        [HttpGet("~/coach/country/{country}")]
        [HttpGet("~/therapist/country/{country}")]
        [HttpGet("~/integration/country/{country}")]
        [HttpGet("~/trip-sitter/country/{country}")]
        [HttpGet("~/facilitator/country/{country}")]
        [HttpGet("~/advocate/country/{country}")]
        [HttpGet("~/researcher/country/{country}")]
        [HttpGet("~/mycologist/country/{country}")]
        [HttpGet("~/anthropologist/country/{country}")]
        public async Task<IActionResult> GetByCountry(string country, bool onlyWithoutTags = false, string[] tags = null, string[] types = null)
        {
            var query = new GetOrganisations(null, country, tags: tags, onlyWithoutTags: onlyWithoutTags, types: GetOrganisationType(types));

            return await ViewWithState(query, (builder, result) => builder.WithOrganisations(result));
        }

        [HttpGet("~/organisation/country/{country}/page/{page}")]
     
        [HttpGet("~/retreat/country/{country}/page/{page}")]
        [HttpGet("~/clinic/country/{country}/page/{page}")]
        [HttpGet("~/research/country/{country}/page/{page}")]
        [HttpGet("~/community/country/{country}/page/{page}")]
        [HttpGet("~/shop/country/{country}/page/{page}")]
        [HttpGet("~/education/country/{country}/page/{page}")]
        [HttpGet("~/consumer/country/{country}/page/{page}")]
        [HttpGet("~/medical/country/{country}/page/{page}")]
        [HttpGet("~/consumer-products/country/{country}/page/{page}")]
        [HttpGet("~/medical-products/country/{country}/page/{page}")]
        [HttpGet("~/business-Services/country/{country}/page/{page}")]
        [HttpGet("~/media/country/{country}/page/{page}")]
        
        [HttpGet("~/shaman/country/{country}/page/{page}")]
        [HttpGet("~/guide/country/{country}/page/{page}")]
        [HttpGet("~/coach/country/{country}/page/{page}")]
        [HttpGet("~/therapist/country/{country}/page/{page}")]
        [HttpGet("~/integration/country/{country}/page/{page}")]
        [HttpGet("~/trip-sitter/country/{country}/page/{page}")]
        [HttpGet("~/facilitator/country/{country}/page/{page}")]
        [HttpGet("~/advocate/country/{country}/page/{page}")]
        [HttpGet("~/researcher/country/{country}/page/{page}")]
        [HttpGet("~/mycologist/country/{country}/page/{page}")]
        [HttpGet("~/anthropologist/country/{country}/page/{page}")]
        public async Task<IActionResult> GetByCountry(string country, int page, bool onlyWithoutTags = false, string[] tags = null, string[] types = null)
        {
            var query = new GetOrganisations(null, country, tags: tags, page: page, onlyWithoutTags: onlyWithoutTags, types: GetOrganisationType(types));

            return await ViewWithState(query, (builder, result) => builder.WithOrganisations(result));
        }

        [HttpGet("~/organisation/country/{country}/tag/{tag}")]
       
        [HttpGet("~/retreat/country/{country}/tag/{tag}")]
        [HttpGet("~/clinic/country/{country}/tag/{tag}")]
        [HttpGet("~/research/country/{country}/tag/{tag}")]
        [HttpGet("~/community/country/{country}/tag/{tag}")]
        [HttpGet("~/shop/country/{country}/tag/{tag}")]
        [HttpGet("~/education/country/{country}/tag/{tag}")]
        [HttpGet("~/consumer/country/{country}/tag/{tag}")]
        [HttpGet("~/medical/country/{country}/tag/{tag}")]
        [HttpGet("~/consumer-products/country/{country}/tag/{tag}")]
        [HttpGet("~/medical-products/country/{country}/tag/{tag}")]
        [HttpGet("~/business-Services/country/{country}/tag/{tag}")]
        [HttpGet("~/media/country/{country}/tag/{tag}")]
        
        [HttpGet("~/shaman/country/{country}/tag/{tag}")]
        [HttpGet("~/guide/country/{country}/tag/{tag}")]
        [HttpGet("~/coach/country/{country}/tag/{tag}")]
        [HttpGet("~/therapist/country/{country}/tag/{tag}")]
        [HttpGet("~/integration/country/{country}/tag/{tag}")]
        [HttpGet("~/trip-sitter/country/{country}/tag/{tag}")]
        [HttpGet("~/facilitator/country/{country}/tag/{tag}")]
        [HttpGet("~/advocate/country/{country}/tag/{tag}")]
        [HttpGet("~/researcher/country/{country}/tag/{tag}")]
        [HttpGet("~/mycologist/country/{country}/tag/{tag}")]
        [HttpGet("~/anthropologist/country/{country}/tag/{tag}")]
        public async Task<IActionResult> GetByCountryAndTag(string country, string tag, bool onlyWithoutTags = false, string[] tags = null, string[] types = null)
        {
            var query = new GetOrganisations(null, country, tags: MergeTags(tag, tags), onlyWithoutTags: onlyWithoutTags, types: GetOrganisationType(types));

            return await ViewWithState(query, (builder, result) => builder.WithOrganisations(result));
        }

        [HttpGet("~/organisation/country/{country}/tag/{tag}/page/{page}")]
        
        [HttpGet("~/retreat/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/clinic/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/research/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/community/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/shop/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/education/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/consumer/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/medical/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/consumer-products/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/medical-products/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/business-Services/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/media/country/{country}/tag/{tag}/page/{page}")]
        
        [HttpGet("~/shaman/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/guide/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/coach/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/therapist/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/integration/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/trip-sitter/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/facilitator/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/advocate/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/researcher/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/mycologist/country/{country}/tag/{tag}/page/{page}")]
        [HttpGet("~/anthropologist/country/{country}/tag/{tag}/page/{page}")]
        public async Task<IActionResult> GetByCountryAndTag(string country, string tag, int page, bool onlyWithoutTags = false, string[] tags = null, string[] types = null)
        {
            var query = new GetOrganisations(null, country, tags: MergeTags(tag, tags), page: page, onlyWithoutTags: onlyWithoutTags, types: GetOrganisationType(types));

            return await ViewWithState(query, (builder, result) => builder.WithOrganisations(result));
        }

        private string[] GetOrganisationType(string[] types)
        {
            var parts = Request.Path.Value.Split("/");
            var value = parts?.Length > 0 ? parts[1] : null;
            var values = value == "organisation" ? types : types.Concat(new [] { value }).ToArray();
            var result = values.Select(value =>
                string.Join(" ", value.Split("-")
                    .Select(word => $"{char.ToUpper(word[0])}{word.Substring(1)}"))).ToArray();
            return result;
        }

        private string[] MergeTags(string tag, string[] tags)
        {
            if (tag == null) return tags;

            return tags != null ? tags.Concat(new[] {tag}).ToArray() : new[] {tag};
        }

        [HttpGet("~/organisation/add")]
        public IActionResult Add()
        {
            return ViewWithState();
        }

        [HttpGet("~/organisation/{id}")]
        public async Task<IActionResult> GetById(OrganisationId id)
        {
            if (id == null)
            {
                throw new InvalidOperationException($"Id is null: {ControllerContext.HttpContext.Request.GetEncodedPathAndQuery()}");
            }

            var query = new GetOrganisation(null, id);

            return await ViewWithState(query,
                (builder, result) => builder.WithOrganisation(result),
                result => result.RedirectWhen(organisation => organisation != null, organisation => organisation.Url, Request.QueryString));
        }

        [HttpGet("~/organisation/{id}/photo/add")]
        public async Task<IActionResult> AddPhotos(OrganisationId id)
        {
            var query = new GetOrganisation(null, id);

            return await ViewWithState(query, 
                (builder, result) => builder.WithOrganisation(result),
                result => result.NotFoundWhenNull());
        }

        [HttpGet("~/organisation/{id}/edit")]
        public async Task<IActionResult> Edit(OrganisationId id)
        {
            var query = new GetOrganisation(null, id);

            return await ViewWithState(query, 
                (builder, result) => builder.WithOrganisation(result),
                result => result.NotFoundWhenNull());
        }

        [HttpGet("~/organisation/{id}/review/add")]
        public IActionResult AddReview()
        {
            return ViewWithState();
        }

        [HttpGet("~/organisation/{id}/review/{reviewId}")]
        public async Task<IActionResult> GetById(OrganisationId id, OrganisationReviewId reviewId)
        {
            var query = new GetOrganisationReview(null, id, reviewId);

            return await ViewWithState(query,
                (builder, result) => builder.WithOrganisationReview(result),
                result => result.RedirectWhen(data => data != null, data => data.Review.Url, Request.QueryString));
        }

        [HttpGet("~/organisation/{id}/review/{reviewId}/{slug}")]
        public async Task<IActionResult> GetReviewByIdAndSlug(OrganisationId id, OrganisationReviewId reviewId, string slug)
        {
            var query = new GetOrganisationReview(null, id, reviewId);

            return await ViewWithState(query,
                (builder, result) => builder.WithOrganisationReview(result),
                result => result.RedirectWhen(data => data != null && data.Review.Slug != slug, data => data.Review.Url, Request.QueryString));
        }

        [HttpGet("~/organisation/{id}/update")]
        public async Task<IActionResult> ListUpdates(OrganisationId id)
        {
            var query = new GetOrganisationUpdates(null, id, true);

            return await ViewWithState(query,
                (builder, result) => builder.WithOrganisationUpdates(result));
        }

        [HttpGet("~/organisation/{id}/update/add")]
        public IActionResult AddUpdate()
        {
            return ViewWithState();
        }

        [HttpGet("~/organisation/{id}/event/add")]
        public IActionResult AddEvent()
        {
            return ViewWithState();
        }

        [HttpGet("~/organisation/{id}/update/{updateId}")]
        public async Task<IActionResult> GetUpdate(OrganisationId id, OrganisationUpdateId updateId)
        {
            var query = new GetOrganisationUpdate(null, id, updateId, true);

            return await ViewWithState(query, (builder, result) => builder.WithOrganisationUpdate(result));
        }

        [HttpGet("~/organisation/{id}/update/{updateId}/edit")]
        public async Task<IActionResult> EditUpdate(OrganisationId id, OrganisationUpdateId updateId)
        {
            var query = new GetOrganisationUpdate(null, id, updateId, true);

            return await ViewWithState(query, (builder, result) => builder.WithOrganisationUpdate(result));
        }

        //[HttpGet("~/organisation/{id}/update/{updateId}/{slug}")]
        //public async Task<IActionResult> EditUpdate(OrganisationId id, OrganisationUpdateId updateId, string slug)
        //{
        //    var query = new GetOrganisationUpdate(null, id, updateId);

        //    return await ViewWithState(query,
        //        (builder, result) => builder.WithOrganisationUpdate(result),
        //        result => result.RedirectWhen(data => data != null && data.Slug != slug, data => data.Url));
        //}

        [HttpGet("~/organisation/{id}/{slug}")]
        public async Task<IActionResult> GetById(OrganisationId id, string slug)
        {
            var query = new GetOrganisation(null, id);

            return await ViewWithState(query,
                (builder, result) => builder.WithOrganisation(result),
                result => result.RedirectWhen(organisation => organisation != null && organisation.Slug != slug, organisation => organisation.Url, Request.QueryString)
            );
        }
        
        /// <summary>
        /// Get a specific photo from an organisation. Contenttype should be jpg/png.
        /// </summary>
        [HttpGet("~/organisation/{id}/photo/{photoId}")]
        [ProducesResponseType(typeof(File), 200)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        public async Task<IActionResult> Photo(OrganisationId id, PhotoId photoId, int height = 400, int width = 750)
        {
            if (id == null)
            {
                throw new InvalidOperationException($"Id is null: {ControllerContext.HttpContext.Request.GetEncodedPathAndQuery()}");
            }
            if (id == null)
            {
                throw new InvalidOperationException($"Photo is null: {ControllerContext.HttpContext.Request.GetEncodedPathAndQuery()}");
            }

            var query = new GetOrganisationPhoto(id, photoId);
            var result = await MessageDispatcher.Send(query);
            if (result == null)
            {
                return NotFound();
            }

            var folder = Configuration.PhotosFolder();
            var fileName = Path.GetFileName(result.FileName);

            var sourceFile = Path.Combine(folder, fileName);
            if (!System.IO.File.Exists(sourceFile))
            {
                return new NotFoundResult();
            }

            var resized = ImageSizes.GetResized(folder, sourceFile, height, width);
            return FileImage(resized ?? sourceFile);
        }

        private static IActionResult FileImage(string sourceFile)
        {
            var stream = System.IO.File.OpenRead(sourceFile);

            return new FileStreamResult(stream, "image/" + Path.GetExtension(sourceFile).TrimStart('.'));
        }
    }
}
